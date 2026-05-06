using FlowMind.Services;
using Microsoft.AspNetCore.Mvc;

namespace FlowMind.Controllers;

/// <summary>
/// Denetim günlüğü kontrolcüsü.
/// Tüm sistem eylemlerini (kullanıcı, YZ, entegrasyon) görüntüler.
/// </summary>
public class AuditController : Controller
{
    private readonly AuditService _auditService;

    public AuditController(AuditService auditService)
    {
        _auditService = auditService;
    }

    /// <summary>Tüm denetim kayıtlarını listeler</summary>
    public IActionResult Index()
    {
        var kayitlar = _auditService.TumKayitlar();
        return View(kayitlar);
    }

    /// <summary>Sadece YZ tarafından tetiklenen kayıtları gösterir</summary>
    public IActionResult YZKayitlari()
    {
        var kayitlar = _auditService.YZKayitlari();
        return View("Index", kayitlar);
    }
}
