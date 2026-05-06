using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FlowMind.Models;

/// <summary>Kullanıcıya atanan görev — onay, inceleme veya veri girişi</summary>
public class TaskItem
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString("N")[..8];

    [Required, MaxLength(200)]
    public string Baslik { get; set; } = string.Empty;

    [MaxLength(500)]
    public string Aciklama { get; set; } = string.Empty;

    /// <summary>Hangi iş akışı örneğine ait</summary>
    [Required]
    public string WorkflowInstanceId { get; set; } = string.Empty;

    /// <summary>Hangi adımdan oluştu</summary>
    public string StepId { get; set; } = string.Empty;

    /// <summary>Görevi kime atandı</summary>
    [Required]
    public string AtananKisiId { get; set; } = string.Empty;

    public string AtananKisiAd { get; set; } = string.Empty;

    public Models.TaskStatus Durum { get; set; } = Models.TaskStatus.Atandı;

    /// <summary>Öncelik seviyesi (1=düşük, 5=kritik)</summary>
    [Range(1, 5)]
    public int Oncelik { get; set; } = 3;

    /// <summary>YZ tarafından mı oluşturuldu</summary>
    public bool YZTarafindan { get; set; } = false;

    /// <summary>İlişkili YZ kararı</summary>
    public string? YZKararId { get; set; }

    /// <summary>Kullanıcı notu (onay/ret sebebi)</summary>
    [MaxLength(500)]
    public string? KullaniciNotu { get; set; }

    public DateTime OlusturmaTarihi { get; set; } = DateTime.Now;

    public DateTime? TamamlanmaTarihi { get; set; }

    public DateTime? SonTarih { get; set; }

    /// <summary>Süre aşımı var mı</summary>
    [NotMapped]
    public bool SuresiGecmis => SonTarih.HasValue && DateTime.Now > SonTarih.Value
                                && Durum != Models.TaskStatus.Tamamlandı
                                && Durum != Models.TaskStatus.Onaylandı;
}
