using FlowMind.Models;

namespace FlowMind.Services;

/// <summary>
/// Uygulama içi bildirim servisi.
/// Kullanıcılara bildirim oluşturur, okunma durumunu yönetir.
/// Not: Issue #7 gereksinimleri doğrultusunda bildirim mekanizması aktiftir.
/// </summary>
public class NotificationService
{
    private readonly InMemoryDataStore _store;
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(InMemoryDataStore store, ILogger<NotificationService> logger)
    {
        _store = store;
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

        _store.Notifications.TryAdd(bildirim.Id, bildirim);
        _logger.LogInformation("[NOTIFICATION] {Baslik} → Kullanıcı: {KullaniciId}", baslik, kullaniciId);

        return bildirim;
    }

    /// <summary>Bir kullanıcının tüm bildirimlerini getirir (en yeni önce)</summary>
    public List<Notification> KullaniciBildirimleri(string kullaniciId)
    {
        return _store.GetAll(_store.Notifications)
            .Where(n => n.KullaniciId == kullaniciId)
            .OrderByDescending(n => n.OlusturmaTarihi)
            .ToList();
    }

    /// <summary>Okunmamış bildirim sayısını döndürür</summary>
    public int OkunmamisSayisi(string kullaniciId)
    {
        return _store.GetAll(_store.Notifications)
            .Count(n => n.KullaniciId == kullaniciId && !n.Okundu);
    }

    /// <summary>Bildirimi okundu olarak işaretler</summary>
    public bool OkunduIsaretle(string bildirimId)
    {
        var bildirim = _store.Get(_store.Notifications, bildirimId);
        if (bildirim != null)
        {
            bildirim.Okundu = true;
            return true;
        }
        return false;
    }

    /// <summary>Tüm bildirimleri okundu olarak işaretler</summary>
    public void TumunuOkunduIsaretle(string kullaniciId)
    {
        var bildirimler = KullaniciBildirimleri(kullaniciId);
        foreach (var b in bildirimler) b.Okundu = true;
    }

    /// <summary>Tüm bildirimleri getirir</summary>
    public List<Notification> TumBildirimler()
    {
        return _store.GetAll(_store.Notifications).OrderByDescending(n => n.OlusturmaTarihi).ToList();
    }
}
