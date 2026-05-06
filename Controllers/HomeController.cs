using System.Diagnostics;
using FlowMind.Models;
using FlowMind.Services;
using FlowMind.Data;
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
        private readonly AIDecisionEngine _kararMotoru;
        private readonly FlowMindDbContext _context;
        private readonly AuditService _auditService;

        public HomeController(
            ILogger<HomeController> logger,
            IIsAkisiServisi isAkisiServisi,
            AIDecisionEngine kararMotoru,
            FlowMindDbContext context,
            AuditService auditService)
        {
            _logger = logger;
            _isAkisiServisi = isAkisiServisi;
            _kararMotoru = kararMotoru;
            _context = context;
            _auditService = auditService;
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
                AIOtomasyonOrani = (int)(_kararMotoru.OtomasyonOrani() * 100),
                ToplamIslemSayisi = _context.Tasks.Count() + _context.WorkflowInstances.Count(),
                BekleyenOnaySayisi = _context.Tasks.Count(t => t.Durum == FlowMind.Models.TaskStatus.Atandı),
                AIKararSayisi = _context.AIDecisions.Count(),
                ZamanKazanciSaat = Math.Round(_context.AIDecisions.Count() * 3.0 / 60.0, 1),
                Aktiviteler = _auditService.TumKayitlar().Take(5).Select(a => new ActivityItem
                {
                    Baslik = a.KullaniciAd + " - " + a.Eylem,
                    Aciklama = string.IsNullOrEmpty(a.Detay) ? a.HedefTur : a.Detay,
                    Tarih = a.Zaman,
                    Ikon = a.YZTetikledi ? "🤖" : "📝",
                    Tur = a.YZTetikledi ? "success" : "info"
                }).ToList()
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
