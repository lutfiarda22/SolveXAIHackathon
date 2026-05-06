using System.ComponentModel.DataAnnotations;

namespace FlowMind.Models;

/// <summary>Sahte API entegrasyon çağrı kaydı</summary>
public class IntegrationLog
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString("N")[..8];

    /// <summary>Hangi iş akışı örneği için</summary>
    [Required]
    public string WorkflowInstanceId { get; set; } = string.Empty;

    /// <summary>Hedef sistem</summary>
    public IntegrationType HedefSistem { get; set; }

    /// <summary>Çağrılan işlem (örn: "KayıtOlustur", "BildirimGonder")</summary>
    [Required, MaxLength(100)]
    public string Islem { get; set; } = string.Empty;

    /// <summary>Gönderilen veri özeti</summary>
    [MaxLength(500)]
    public string GonderilenVeri { get; set; } = string.Empty;

    /// <summary>Dönen yanıt</summary>
    [MaxLength(500)]
    public string Yanit { get; set; } = string.Empty;

    public IntegrationStatus Durum { get; set; } = IntegrationStatus.Bekliyor;

    /// <summary>İşlem süresi (milisaniye)</summary>
    public int SureMs { get; set; }

    /// <summary>Hata mesajı (varsa)</summary>
    [MaxLength(300)]
    public string? HataMesaji { get; set; }

    public DateTime Zaman { get; set; } = DateTime.Now;
}
