using System.ComponentModel.DataAnnotations;

namespace FlowMind.Models;

/// <summary>Sistem kullanıcısı — çalışan, yönetici, finans vb.</summary>
public class User
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N")[..8];

    [Required, MaxLength(100)]
    public string Ad { get; set; } = string.Empty;

    [Required, MaxLength(100)]
    public string Soyad { get; set; } = string.Empty;

    [Required, EmailAddress]
    public string EPosta { get; set; } = string.Empty;

    public UserRole Rol { get; set; } = UserRole.Calisan;

    [MaxLength(50)]
    public string Departman { get; set; } = string.Empty;

    public DateTime KayitTarihi { get; set; } = DateTime.Now;

    // Hesaplanmış alan
    public string TamAd => $"{Ad} {Soyad}";
}
