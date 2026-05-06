using System.Diagnostics;
using FlowMind.Data;
using FlowMind.Models;

namespace FlowMind.Services;

/// <summary>
/// Gelişmiş YZ Karar Motoru.
/// İş akışı adımlarını analiz eder, güven skoru ve gerekçeyle karar verir.
/// Kural tabanlı basit heuristik kullanır (hackathon MVP).
/// 
/// AI TRACEABILITY: Bu sınıf tamamen YZ karar verme mantığını içerir.
/// Her karar AIDecision modeli olarak loglanır ve denetim izlenebilirliği sağlar.
/// </summary>
public class AIDecisionEngine
{
    private readonly FlowMindDbContext _context;
    private readonly AuditService _audit;
    private readonly ILogger<AIDecisionEngine> _logger;

    // Karar eşikleri (kolay ayarlanabilir) - Issue #13: YZ Karar Motoru Eşikleri
    private const decimal OtomatikOnayEsik = 5000m;     // ₺5.000 altı otomatik onay (Low Risk)
    private const decimal YuksekRiskEsik = 50000m;       // ₺50.000 üstü yüksek risk (High Risk)
    private const int AcilSaatEsik = 24;                 // 24 saat içinde son tarih = acil (Öncelik belirleme)

    public AIDecisionEngine(FlowMindDbContext context, AuditService audit, ILogger<AIDecisionEngine> logger)
    {
        _context = context;
        _audit = audit;
        _logger = logger;
    }

    /// <summary>
    /// Bir iş akışı adımı için YZ kararı verir.
    /// Güven skoru, önerilen eylem ve Türkçe gerekçe içerir.
    /// </summary>
    public AIDecision Degerlendir(WorkflowInstance instance, WorkflowStep step)
    {
        var sw = Stopwatch.StartNew();

        // Karar mantığını uygula
        var (aksiyon, guven, gerekce) = step.EylemTuru switch
        {
            StepActionType.Onay => OnayKarariVer(instance, step),
            StepActionType.Entegrasyon => EntegrasyonKarariVer(instance, step),
            StepActionType.YZKarar => AkisKarariVer(instance, step),
            _ => (AIAction.OtomatikOnayla, 0.80, "Standart adım, otomatik işleme alındı.")
        };

        sw.Stop();

        // AI karar nesnesini oluştur
        var karar = new AIDecision
        {
            WorkflowInstanceId = instance.Id,
            StepId = step.Id,
            SuggestedAction = aksiyon,
            ConfidenceScore = guven,
            Reason = gerekce,
            Baglan = BaglamOlustur(instance, step),
            IslemSuresiMs = (int)sw.ElapsedMilliseconds,
            KararZamani = DateTime.Now
        };

        // Depoya kaydet
        _context.AIDecisions.Add(karar);
        instance.YZKararIds.Add(karar.Id);
        _context.SaveChanges();

        // Denetim kaydı
        _audit.YZKaydi(
            $"YZ Kararı: {aksiyon} (Güven: %{guven * 100:F0})",
            "WorkflowInstance", instance.Id,
            gerekce);

        _logger.LogInformation("[AI] {Aksiyon} | Güven: {Guven:P0} | Gerekçe: {Gerekce}",
            aksiyon, guven, gerekce);

        return karar;
    }

    /// <summary>Onay adımları için karar verir (otomatik onay / manuel inceleme)</summary>
    private (AIAction, double, string) OnayKarariVer(WorkflowInstance instance, WorkflowStep step)
    {
        // Tutarı form verisinden al
        var tutarStr = instance.FormVerisi.GetValueOrDefault("Tutar", "0");
        if (!decimal.TryParse(tutarStr, out var tutar)) tutar = 0;

        var departman = instance.FormVerisi.GetValueOrDefault("Departman", "Bilinmiyor");
        var konu = instance.FormVerisi.GetValueOrDefault("Konu", "");
        var adimAdi = step.Ad;
        var izinSebebi = instance.FormVerisi.GetValueOrDefault("IzinSebebi", "");
        var iadeSebebi = instance.FormVerisi.GetValueOrDefault("IadeSebebi", "");

        // KURAL 0: İzin Süreçleri (Hastalık, Vefat vb.)
        if (!string.IsNullOrEmpty(izinSebebi) || konu.Contains("İzin", StringComparison.OrdinalIgnoreCase) || adimAdi.Contains("İzin", StringComparison.OrdinalIgnoreCase) || adimAdi.Contains("izin", StringComparison.OrdinalIgnoreCase))
        {
            if (izinSebebi == "Hastalık Raporu" || izinSebebi == "Vefat" || izinSebebi == "Düğün/Evlilik" || izinSebebi == "Doğum")
            {
                return (AIAction.OtomatikOnayla, 0.95,
                    $"İzin talebinde yasal/öncelikli mazeret ({izinSebebi}) tespit edildi. Güven skoru yüksek, otomatik onay verildi.");
            }
            else
            {
                return (AIAction.ManuelInceleme, 0.40,
                    $"İzin talebi ({izinSebebi}) standart veya inisiyatif gerektiren türde olduğu için manuel incelemeye yönlendirildi.");
            }
        }

        // KURAL 0.5: İade Süreçleri
        if (!string.IsNullOrEmpty(iadeSebebi) || konu.Contains("İade", StringComparison.OrdinalIgnoreCase) || adimAdi.Contains("İade", StringComparison.OrdinalIgnoreCase) || adimAdi.Contains("iade", StringComparison.OrdinalIgnoreCase))
        {
            if (iadeSebebi == "Kusurlu Ürün" || iadeSebebi == "Yanlış Ürün Gönderimi")
            {
                return (AIAction.OtomatikOnayla, 0.90,
                    $"İade sebebi olarak operasyonel hata ({iadeSebebi}) tespit edildiğinden iade otomatik onaylandı.");
            }
            else
            {
                return (AIAction.ManuelInceleme, 0.50,
                    $"İade sebebi ({iadeSebebi}) müşteri/ürün incelemesi gerektirdiğinden manuel onaya gönderildi.");
            }
        }

        // Kural 1: Düşük tutar → otomatik onay
        if (tutar > 0 && tutar <= OtomatikOnayEsik)
        {
            return (AIAction.OtomatikOnayla, 0.95,
                $"Tutar (₺{tutar:N0}) eşik değerin (₺{OtomatikOnayEsik:N0}) altında. " +
                $"Departman: {departman}. Otomatik onay verildi.");
        }

        // Kural 2: Çok yüksek tutar → kesinlikle manuel
        if (tutar > YuksekRiskEsik)
        {
            return (AIAction.ManuelInceleme, 0.30,
                $"Tutar (₺{tutar:N0}) yüksek risk eşiğinin (₺{YuksekRiskEsik:N0}) üzerinde. " +
                $"Yönetici onayı zorunlu. Manuel incelemeye yönlendirildi.");
        }

        // Kural 3: Orta tutar → güvene göre karar
        var guven = 1.0 - ((double)(tutar - OtomatikOnayEsik) / (double)(YuksekRiskEsik - OtomatikOnayEsik));
        guven = Math.Clamp(guven, 0.35, 0.75);

        if (guven >= 0.6)
        {
            return (AIAction.OtomatikOnayla, guven,
                $"Tutar (₺{tutar:N0}) orta seviyede. Departman: {departman}. " +
                $"Güven skoru yeterli, otomatik onay verildi.");
        }

        return (AIAction.ManuelInceleme, guven,
            $"Tutar (₺{tutar:N0}) dikkat gerektiriyor. Departman: {departman}. " +
            $"Güven skoru düşük, finans onayı gerekli.");
    }

    /// <summary>Entegrasyon adımları için karar verir (adım atlama / devam)</summary>
    private (AIAction, double, string) EntegrasyonKarariVer(WorkflowInstance instance, WorkflowStep step)
    {
        // YZ atlayabilir mi kontrol et
        if (step.YZAtlayabilir)
        {
            // Mevcut entegrasyon loglarını kontrol et
            var mevcutLog = _context.IntegrationLogs
                .Any(l => l.WorkflowInstanceId == instance.Id
                       && l.HedefSistem == step.EntegrasyonTuru
                       && l.Durum == IntegrationStatus.Basarili);

            if (mevcutLog)
            {
                return (AIAction.AdimAtla, 0.90,
                    $"Hedef sistemde ({step.EntegrasyonTuru}) başarılı kayıt zaten mevcut. Adım atlandı.");
            }
        }

        return (AIAction.OtomatikOnayla, 0.85,
            $"Entegrasyon adımı ({step.EntegrasyonTuru}) otomatik olarak tetikleniyor.");
    }

    /// <summary>Genel akış kararı verir (öncelik, yönlendirme)</summary>
    private (AIAction, double, string) AkisKarariVer(WorkflowInstance instance, WorkflowStep step)
    {
        // Yüksek riskli tutar kontrolü
        var tutarStr = instance.FormVerisi.GetValueOrDefault("Tutar", "0");
        if (decimal.TryParse(tutarStr, out var tutar) && tutar > YuksekRiskEsik)
        {
            return (AIAction.ManuelInceleme, 0.30,
                $"Tutar (₺{tutar:N0}) yüksek risk eşiğinin (₺{YuksekRiskEsik:N0}) üzerinde. " +
                $"Manuel incelemeye yönlendirildi.");
        }

        // Son tarih kontrolü
        var sonTarihStr = instance.FormVerisi.GetValueOrDefault("SonTarih", "");
        if (DateTime.TryParse(sonTarihStr, out var sonTarih))
        {
            var kalanSaat = (sonTarih - DateTime.Now).TotalHours;
            if (kalanSaat < AcilSaatEsik && kalanSaat > 0)
            {
                return (AIAction.OncelikliYonlendir, 0.92,
                    $"Son tarih {kalanSaat:F0} saat içinde. Üst onaylayıcıya acil yönlendirme yapıldı.");
            }
        }

        return (AIAction.OtomatikOnayla, 0.80,
            "Standart akış devam ediyor. Ek müdahale gerekmiyor.");
    }

    /// <summary>Karar bağlamı oluşturur</summary>
    private string BaglamOlustur(WorkflowInstance instance, WorkflowStep step)
    {
        var parts = new List<string>
        {
            $"İşAkışı: {instance.WorkflowAdi}",
            $"Adım: {step.Ad} (Sıra: {step.Sira})",
            $"Tür: {step.EylemTuru}"
        };

        if (instance.FormVerisi.TryGetValue("Tutar", out var tutar))
            parts.Add($"Tutar: ₺{tutar}");
        if (instance.FormVerisi.TryGetValue("Departman", out var dept))
            parts.Add($"Departman: {dept}");

        return string.Join(" | ", parts);
    }

    // ===== Sorgu Metotları =====

    /// <summary>Tüm YZ kararlarını getirir</summary>
    public List<AIDecision> TumKararlar()
    {
        return _context.AIDecisions.OrderByDescending(d => d.KararZamani).ToList();
    }

    /// <summary>Belirli iş akışı için kararları getirir</summary>
    public List<AIDecision> IsAkisiKararlari(string workflowInstanceId)
    {
        return _context.AIDecisions
            .Where(d => d.WorkflowInstanceId == workflowInstanceId)
            .OrderByDescending(d => d.KararZamani).ToList();
    }

    /// <summary>Otomasyonu oranını hesaplar</summary>
    public double OtomasyonOrani()
    {
        var kararlar = _context.AIDecisions.ToList();
        if (!kararlar.Any()) return 0;

        var otomatik = kararlar.Count(k =>
            k.SuggestedAction == AIAction.OtomatikOnayla ||
            k.SuggestedAction == AIAction.AdimAtla);

        return (double)otomatik / kararlar.Count;
    }

    /// <summary>Ortalama güven skorunu hesaplar</summary>
    public double OrtalamaGuvenSkoru()
    {
        var kararlar = _context.AIDecisions.ToList();
        return kararlar.Any() ? kararlar.Average(k => k.ConfidenceScore) : 0;
    }
}
