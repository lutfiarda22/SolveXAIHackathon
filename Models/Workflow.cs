using System.ComponentModel.DataAnnotations;

namespace FlowMind.Models;

/// <summary>İş akışı şablonu — tekrar kullanılabilir süreç tanımı</summary>
public class Workflow
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N")[..8];

    [Required, MaxLength(150)]
    public string Ad { get; set; } = string.Empty;

    [MaxLength(500)]
    public string Aciklama { get; set; } = string.Empty;

    /// <summary>Bu iş akışının adımları</summary>
    public List<WorkflowStep> Adimlar { get; set; } = new();

    /// <summary>Tetikleyici açıklama (örn: "Satın alma talebi geldiğinde")</summary>
    [MaxLength(200)]
    public string Tetikleyici { get; set; } = string.Empty;

    public string OlusturanId { get; set; } = string.Empty;

    public DateTime OlusturmaTarihi { get; set; } = DateTime.Now;

    public bool Aktif { get; set; } = true;
}
