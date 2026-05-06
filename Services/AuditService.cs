using FlowMind.Models;

namespace FlowMind.Services;

/// <summary>
/// Denetim günlüğü servisi.
/// Sistemdeki tüm işlemleri (kullanıcı, YZ, entegrasyon) kayıt altına alır.
/// Not: Issue #7 gereksinimleri doğrultusunda loglama altyapısı tamamlanmıştır.
/// </summary>
public class AuditService
{
    private readonly InMemoryDataStore _store;
    private readonly ILogger<AuditService> _logger;

    public AuditService(InMemoryDataStore store, ILogger<AuditService> logger)
    {
        _store = store;
        _logger = logger;
    }

    /// <summary>Yeni denetim kaydı oluşturur</summary>
    public AuditLog Kaydet(string kullaniciId, string kullaniciAd, string eylem,
        string hedefTur, string hedefId, string? detay = null, bool yzTetikledi = false)
    {
        var log = new AuditLog
        {
            KullaniciId = kullaniciId,
            KullaniciAd = kullaniciAd,
            Eylem = eylem,
            HedefTur = hedefTur,
            HedefId = hedefId,
            Detay = detay,
            YZTetikledi = yzTetikledi,
            Zaman = DateTime.Now
        };

        _store.AuditLogs.TryAdd(log.Id, log);
        _logger.LogInformation("[AUDIT] {Eylem} | {HedefTur}:{HedefId} | Kullanıcı: {Kullanici} | YZ: {YZ}",
            eylem, hedefTur, hedefId, kullaniciAd, yzTetikledi);

        return log;
    }

    /// <summary>YZ tarafından tetiklenen kayıt oluşturur (kısa yol)</summary>
    public AuditLog YZKaydi(string eylem, string hedefTur, string hedefId, string? detay = null)
    {
        return Kaydet("YZ", "AI Karar Motoru", eylem, hedefTur, hedefId, detay, yzTetikledi: true);
    }

    /// <summary>Tüm denetim kayıtlarını getirir (en yeni önce)</summary>
    public List<AuditLog> TumKayitlar()
    {
        return _store.GetAll(_store.AuditLogs).OrderByDescending(a => a.Zaman).ToList();
    }

    /// <summary>Belirli bir hedefe ait kayıtları getirir</summary>
    public List<AuditLog> HedefKayitlari(string hedefId)
    {
        return _store.GetAll(_store.AuditLogs)
            .Where(a => a.HedefId == hedefId)
            .OrderByDescending(a => a.Zaman)
            .ToList();
    }

    /// <summary>YZ tarafından tetiklenen kayıtları getirir</summary>
    public List<AuditLog> YZKayitlari()
    {
        return _store.GetAll(_store.AuditLogs)
            .Where(a => a.YZTetikledi)
            .OrderByDescending(a => a.Zaman)
            .ToList();
    }
}
