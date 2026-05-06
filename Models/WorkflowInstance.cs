using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FlowMind.Models;

/// <summary>Çalışan bir iş akışı örneği — şablondan türetilir</summary>
public class WorkflowInstance
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString("N")[..8];

    /// <summary>Hangi şablondan oluşturuldu</summary>
    [Required]
    public string WorkflowId { get; set; } = string.Empty;

    /// <summary>İş akışı şablonunun adı (hızlı erişim)</summary>
    public string WorkflowAdi { get; set; } = string.Empty;

    /// <summary>Mevcut durum</summary>
    public WorkflowStatus Durum { get; set; } = WorkflowStatus.Taslak;

    /// <summary>Şu an hangi adımda (sıra numarası)</summary>
    public int MevcutAdim { get; set; } = 1;

    /// <summary>İş akışını başlatan kullanıcı</summary>
    [Required]
    public string BaslatanId { get; set; } = string.Empty;

    /// <summary>İş akışıyla taşınan veri (örn: talep detayları)</summary>
    public Dictionary<string, string> FormVerisi { get; set; } = new();

    public DateTime BaslangicTarihi { get; set; } = DateTime.Now;

    public DateTime? BitisTarihi { get; set; }

    /// <summary>Bu örnek için YZ tarafından alınan kararlar</summary>
    public List<string> YZKararIds { get; set; } = new();

    /// <summary>YZ tarafından otomatik tamamlanan adım sayısı</summary>
    public int OtomatikTamamlanan { get; set; } = 0;

    /// <summary>Toplam geçen süre (dakika)</summary>
    [NotMapped]
    public double? ToplamSureDk => BitisTarihi.HasValue
        ? (BitisTarihi.Value - BaslangicTarihi).TotalMinutes
        : null;
}
