using System.ComponentModel.DataAnnotations;

namespace FlowMind.Models;

/// <summary>İş akışı şablonundaki tek bir adım</summary>
public class WorkflowStep
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N")[..8];

    [Required, MaxLength(150)]
    public string Ad { get; set; } = string.Empty;

    [MaxLength(300)]
    public string Aciklama { get; set; } = string.Empty;

    /// <summary>Adımın sırası (1, 2, 3...)</summary>
    [Range(1, 100)]
    public int Sira { get; set; }

    /// <summary>Bu adımda ne yapılacak</summary>
    public StepActionType EylemTuru { get; set; }

    /// <summary>Atanan kullanıcı veya rol</summary>
    [MaxLength(100)]
    public string AtananKisi { get; set; } = string.Empty;

    /// <summary>Koşul açıklaması (örn: "Tutar > 5000 ise")</summary>
    [MaxLength(300)]
    public string Kosul { get; set; } = string.Empty;

    /// <summary>Entegrasyon hedefi (CRM, ERP vb.) — sadece Entegrasyon türü adımlar için</summary>
    public IntegrationType? EntegrasyonTuru { get; set; }

    /// <summary>YZ bu adımı atlayabilir mi?</summary>
    public bool YZAtlayabilir { get; set; } = false;
}
