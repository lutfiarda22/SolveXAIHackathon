using FlowMind.Models;
using FlowMind.Data;

namespace FlowMind.Services;

/// <summary>
/// Sahte (Mock) Entegrasyon Servisi.
/// CRM, ERP, E-Posta, Slack ve Drive sistemlerine yapılan API çağrılarını simüle eder.
/// Task.Delay ile gerçekçi gecikme sağlar ve her çağrıyı IntegrationLog olarak kaydeder.
/// </summary>
public class MockIntegrationService
{
    private readonly FlowMindDbContext _context;
    private readonly ILogger<MockIntegrationService> _logger;
    private readonly Random _random = new();

    public MockIntegrationService(FlowMindDbContext context, ILogger<MockIntegrationService> logger)
    {
        _context = context;
        _logger = logger;
    }

    // ===== CRM Mock Bağlayıcı =====

    /// <summary>CRM'de yeni kayıt oluşturur</summary>
    public async Task<IntegrationLog> CrmKayitOlustur(string workflowInstanceId, string ad, string detay)
    {
        return await SimuleEt(workflowInstanceId, IntegrationType.CRM, "KayıtOluştur",
            $"{{\"Ad\":\"{ad}\",\"Detay\":\"{detay}\"}}",
            () => $"{{\"CrmId\":\"CRM-{_random.Next(1000, 9999)}\",\"Status\":\"Created\"}}");
    }

    /// <summary>CRM'den kayıt sorgular</summary>
    public async Task<IntegrationLog> CrmSorgula(string workflowInstanceId, string sorguKriteri)
    {
        return await SimuleEt(workflowInstanceId, IntegrationType.CRM, "Sorgula",
            $"{{\"Kriter\":\"{sorguKriteri}\"}}",
            () => $"{{\"BulunanSayisi\":{_random.Next(0, 10)},\"Status\":\"OK\"}}");
    }

    // ===== ERP Mock Bağlayıcı =====

    /// <summary>ERP'de satınalma emri oluşturur</summary>
    public async Task<IntegrationLog> ErpSatinalmaEmri(string workflowInstanceId, string urun, decimal tutar)
    {
        return await SimuleEt(workflowInstanceId, IntegrationType.ERP, "SatınAlmaEmri",
            $"{{\"Ürün\":\"{urun}\",\"Tutar\":{tutar}}}",
            () => $"{{\"EmirNo\":\"PO-2026-{_random.Next(100, 999)}\",\"Status\":\"Created\"}}");
    }

    /// <summary>ERP'de envanter günceller</summary>
    public async Task<IntegrationLog> ErpEnvanterGuncelle(string workflowInstanceId, string urunKodu, int miktar)
    {
        return await SimuleEt(workflowInstanceId, IntegrationType.ERP, "EnvanterGüncelle",
            $"{{\"ÜrünKodu\":\"{urunKodu}\",\"Miktar\":{miktar}}}",
            () => $"{{\"GüncelStok\":{_random.Next(10, 500)},\"Status\":\"Updated\"}}");
    }

    // ===== E-Posta Mock Bağlayıcı =====

    /// <summary>E-posta gönderir</summary>
    public async Task<IntegrationLog> EpostaGonder(string workflowInstanceId, string alici, string konu, string icerik)
    {
        return await SimuleEt(workflowInstanceId, IntegrationType.EPosta, "EpostaGönder",
            $"{{\"To\":\"{alici}\",\"Subject\":\"{konu}\"}}",
            () => $"{{\"MessageId\":\"msg_{Guid.NewGuid().ToString("N")[..6]}\",\"Status\":\"Sent\"}}");
    }

    // ===== Slack Mock Bağlayıcı =====

    /// <summary>Slack kanalına bildirim gönderir</summary>
    public async Task<IntegrationLog> SlackBildirimGonder(string workflowInstanceId, string kanal, string mesaj)
    {
        return await SimuleEt(workflowInstanceId, IntegrationType.Slack, "KanalBildirim",
            $"{{\"Channel\":\"#{kanal}\",\"Text\":\"{mesaj}\"}}",
            () => $"{{\"Ts\":\"{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}\",\"Status\":\"Posted\"}}");
    }

    // ===== Drive Mock Bağlayıcı =====

    /// <summary>Drive'a dosya yükler</summary>
    public async Task<IntegrationLog> DriveDosyaYukle(string workflowInstanceId, string dosyaAdi)
    {
        return await SimuleEt(workflowInstanceId, IntegrationType.Drive, "DosyaYükle",
            $"{{\"FileName\":\"{dosyaAdi}\"}}",
            () => $"{{\"FileId\":\"file_{Guid.NewGuid().ToString("N")[..8]}\",\"Url\":\"https://drive.mock/{dosyaAdi}\",\"Status\":\"Uploaded\"}}");
    }

    // ===== Simülasyon Altyapısı =====

    /// <summary>
    /// Tüm mock çağrıların ortak simülasyon altyapısı.
    /// Gecikme simüle eder, %90 başarı oranıyla çalışır ve IntegrationLog kaydeder.
    /// </summary>
    private async Task<IntegrationLog> SimuleEt(string workflowInstanceId, IntegrationType hedef,
        string islem, string gonderilenVeri, Func<string> yanitOlustur)
    {
        var sw = System.Diagnostics.Stopwatch.StartNew();

        // Gerçekçi gecikme simüle et (50-250ms)
        await Task.Delay(_random.Next(50, 250));

        // %90 başarı oranı
        var basarili = _random.Next(1, 11) > 1; // %10 hata şansı

        sw.Stop();

        var log = new IntegrationLog
        {
            WorkflowInstanceId = workflowInstanceId,
            HedefSistem = hedef,
            Islem = islem,
            GonderilenVeri = gonderilenVeri,
            Yanit = basarili ? yanitOlustur() : "Timeout",
            Durum = basarili ? IntegrationStatus.Basarili : IntegrationStatus.Basarisiz,
            SureMs = (int)sw.ElapsedMilliseconds,
            HataMesaji = basarili ? null : "Simüle edilmiş hata: Sunucu yanıt vermedi.",
            Zaman = DateTime.Now
        };

        _context.IntegrationLogs.Add(log);
        await _context.SaveChangesAsync();

        _logger.LogInformation("[INTEGRATION] {Hedef}.{Islem} | Durum: {Durum} | Süre: {Sure}ms",
            hedef, islem, log.Durum, log.SureMs);

        return log;
    }

    /// <summary>Tüm entegrasyon loglarını getirir</summary>
    public List<IntegrationLog> TumLoglar()
    {
        return _context.IntegrationLogs.OrderByDescending(l => l.Zaman).ToList();
    }

    /// <summary>Belirli bir iş akışı için logları getirir</summary>
    public List<IntegrationLog> IsAkisiLoglari(string workflowInstanceId)
    {
        return _context.IntegrationLogs
            .Where(l => l.WorkflowInstanceId == workflowInstanceId)
            .OrderByDescending(l => l.Zaman)
            .ToList();
    }
}
