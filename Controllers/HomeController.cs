using System.Diagnostics;
using FlowMind.Models;
using FlowMind.Services;
using Microsoft.AspNetCore.Mvc;

namespace FlowMind.Controllers
{
    /// <summary>
    /// Ana sayfa ve dashboard kontrolcüsü.
    /// Servisler üzerinden veri çekerek görünümlere aktarır.
    /// </summary>
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IIsAkisiServisi _isAkisiServisi;
        private readonly IKararMotoru _kararMotoru;

        public HomeController(
            ILogger<HomeController> logger,
            IIsAkisiServisi isAkisiServisi,
            IKararMotoru kararMotoru)
        {
            _logger = logger;
            _isAkisiServisi = isAkisiServisi;
            _kararMotoru = kararMotoru;
        }

        /// <summary>
        /// Ana panel (Dashboard) sayfası
        /// </summary>
        public IActionResult Index()
        {
            var model = new PanelViewModel
            {
                ToplamIsAkisi = _isAkisiServisi.ToplamIsAkisiSayisi(),
                AktifIsAkisi = _isAkisiServisi.AktifIsAkisiSayisi(),
                TamamlananIsAkisi = _isAkisiServisi.TamamlananIsAkisiSayisi(),
                SistemDurumu = _isAkisiServisi.SistemDurumuOzeti(),
                AIOtomasyonOrani = 78, // Mock data
                ToplamIslemSayisi = 1250, // Mock data
                Aktiviteler = new List<ActivityItem>
                {
                    new ActivityItem { Baslik = "AI Kararı: Onaylandı", Aciklama = "Sipariş #1042 otomatik olarak onaylandı.", Tarih = DateTime.Now.AddMinutes(-5), Ikon = "🤖", Tur = "success" },
                    new ActivityItem { Baslik = "Yeni İş Akışı", Aciklama = "İnsan Kaynakları işe alım süreci başlatıldı.", Tarih = DateTime.Now.AddMinutes(-25), Ikon = "🔄", Tur = "info" },
                    new ActivityItem { Baslik = "Risk Uyarısı", Aciklama = "Ödeme işleminde anomali tespit edildi, manuel inceleme bekleniyor.", Tarih = DateTime.Now.AddHours(-1), Ikon = "⚠️", Tur = "warning" },
                    new ActivityItem { Baslik = "Entegrasyon Hatası", Aciklama = "CRM servisine bağlanılamadı.", Tarih = DateTime.Now.AddHours(-2), Ikon = "❌", Tur = "danger" }
                }
            };

            _logger.LogInformation("Ana panel yüklendi. Toplam: {Toplam}, Aktif: {Aktif}",
                model.ToplamIsAkisi, model.AktifIsAkisi);

            return View(model);
        }

        /// <summary>
        /// Hakkında sayfası
        /// </summary>
        public IActionResult Hakkinda()
        {
            return View();
        }

        /// <summary>
        /// Hata sayfası
        /// </summary>
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
