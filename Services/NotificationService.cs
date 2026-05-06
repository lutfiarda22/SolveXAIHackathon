using FlowMind.Models;
using FlowMind.Data;

namespace FlowMind.Services;

/// <summary>
/// Uygulama içi bildirim servisi.
/// Kullanıcılara bildirim oluşturur, okunma durumunu yönetir.
/// Not: Issue #7 gereksinimleri doğrultusunda bildirim mekanizması aktiftir.
/// </summary>
public class NotificationService
{
    private readonly FlowMindDbContext _context;
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(FlowMindDbContext context, ILogger<NotificationService> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>Yeni bildirim oluşturur</summary>
    public Notification Olustur(string kullaniciId, string baslik, string mesaj,
        NotificationType tur, string? iliskiliId = null)
    {
        var bildirim = new Notification
        {
            KullaniciId = kullaniciId,
            Baslik = baslik,
            Mesaj = mesaj,
            Tur = tur,
            IliskiliId = iliskiliId,
            OlusturmaTarihi = DateTime.Now
        };

        _context.Notifications.Add(bildirim);
        _context.SaveChanges();
        _logger.LogInformation("[NOTIFICATION] {Baslik} → Kullanıcı: {KullaniciId}", baslik, kullaniciId);

        return bildirim;
    }

    /// <summary>Bir kullanıcının tüm bildirimlerini getirir (en yeni önce)</summary>
    public List<Notification> KullaniciBildirimleri(string kullaniciId)
    {
        return _context.Notifications
            .Where(n => n.KullaniciId == kullaniciId)
            .OrderByDescending(n => n.OlusturmaTarihi)
            .ToList();
    }

    /// <summary>Okunmamış bildirim sayısını döndürür</summary>
    public int OkunmamisSayisi(string kullaniciId)
    {
        return _context.Notifications
            .Count(n => n.KullaniciId == kullaniciId && !n.Okundu);
    }

    /// <summary>Bildirimi okundu olarak işaretler</summary>
    public bool OkunduIsaretle(string bildirimId)
    {
        var bildirim = _context.Notifications.FirstOrDefault(n => n.Id == bildirimId);
        if (bildirim != null)
        {
            bildirim.Okundu = true;
            _context.SaveChanges();
            return true;
        }
        return false;
    }

    /// <summary>Tüm bildirimleri okundu olarak işaretler</summary>
    public void TumunuOkunduIsaretle(string kullaniciId)
    {
        var bildirimler = KullaniciBildirimleri(kullaniciId);
        foreach (var b in bildirimler) b.Okundu = true;
        _context.SaveChanges();
    }

    /// <summary>Tüm bildirimleri getirir</summary>
    public List<Notification> TumBildirimler()
    {
        return _context.Notifications.OrderByDescending(n => n.OlusturmaTarihi).ToList();
    }
}
