using System.ComponentModel.DataAnnotations;

namespace FlowMind.Models;

/// <summary>Uygulama içi bildirim</summary>
public class Notification
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString("N")[..8];

    /// <summary>Bildirimin gönderildiği kullanıcı</summary>
    [Required]
    public string KullaniciId { get; set; } = string.Empty;

    [Required, MaxLength(200)]
    public string Baslik { get; set; } = string.Empty;

    [MaxLength(500)]
    public string Mesaj { get; set; } = string.Empty;

    public NotificationType Tur { get; set; }

    /// <summary>İlişkili nesne ID'si (workflow, task vb.)</summary>
    public string? IliskiliId { get; set; }

    public bool Okundu { get; set; } = false;

    public DateTime OlusturmaTarihi { get; set; } = DateTime.Now;
}
