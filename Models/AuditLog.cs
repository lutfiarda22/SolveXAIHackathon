using System.ComponentModel.DataAnnotations;

namespace FlowMind.Models;

/// <summary>Denetim kaydı — kim, ne yaptı, ne zaman</summary>
public class AuditLog
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString("N")[..8];

    /// <summary>İşlemi yapan kullanıcı (veya "SISTEM" / "YZ")</summary>
    [Required, MaxLength(100)]
    public string KullaniciId { get; set; } = string.Empty;

    public string KullaniciAd { get; set; } = string.Empty;

    /// <summary>Yapılan eylem açıklaması</summary>
    [Required, MaxLength(300)]
    public string Eylem { get; set; } = string.Empty;

    /// <summary>Etkilenen nesne türü (Workflow, Task, vb.)</summary>
    [MaxLength(50)]
    public string HedefTur { get; set; } = string.Empty;

    /// <summary>Etkilenen nesnenin ID'si</summary>
    [MaxLength(50)]
    public string HedefId { get; set; } = string.Empty;

    /// <summary>Ek detay</summary>
    [MaxLength(500)]
    public string? Detay { get; set; }

    /// <summary>YZ tarafından mı tetiklendi</summary>
    public bool YZTetikledi { get; set; } = false;

    public DateTime Zaman { get; set; } = DateTime.Now;
}
