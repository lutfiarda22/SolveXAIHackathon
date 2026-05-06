using System.ComponentModel.DataAnnotations;

namespace FlowMind.Models;

/// <summary>YZ Karar Motoru tarafından alınan bir karar — tam izlenebilirlik</summary>
public class AIDecision
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N")[..8];

    /// <summary>Hangi iş akışı örneği için</summary>
    [Required]
    public string WorkflowInstanceId { get; set; } = string.Empty;

    /// <summary>Hangi adım için karar verildi</summary>
    public string StepId { get; set; } = string.Empty;

    /// <summary>YZ'nin önerdiği eylem</summary>
    [Required]
    public AIAction SuggestedAction { get; set; }

    /// <summary>Güven skoru (0.0 = düşük güven, 1.0 = tam güven)</summary>
    [Range(0.0, 1.0)]
    public double ConfidenceScore { get; set; }

    /// <summary>Kararın Türkçe açıklaması — neden bu karar verildi</summary>
    [Required, MaxLength(1000)]
    public string Reason { get; set; } = string.Empty;

    /// <summary>Karar bağlamı — değerlendirilen veriler</summary>
    [MaxLength(500)]
    public string Baglan { get; set; } = string.Empty;

    /// <summary>Kararın verildiği zaman</summary>
    public DateTime KararZamani { get; set; } = DateTime.Now;

    /// <summary>Kullanıcı bu kararı geçersiz kıldı mı?</summary>
    public bool GecersizKilindi { get; set; } = false;

    /// <summary>Geçersiz kılan kullanıcının ID'si</summary>
    public string? GecersizKilanId { get; set; }

    /// <summary>Geçersiz kılma sebebi</summary>
    [MaxLength(500)]
    public string? GecersizKilmaSebebi { get; set; }

    /// <summary>İşlem süresi (milisaniye)</summary>
    public int IslemSuresiMs { get; set; }
}
