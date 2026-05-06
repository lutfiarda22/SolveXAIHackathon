using FlowMind.Models;
using FlowMind.Services;
using Microsoft.AspNetCore.Mvc;

namespace FlowMind.Controllers;

public class WorkflowController : Controller
{
    private readonly IIsAkisiServisi _isAkisiServisi;

    public WorkflowController(IIsAkisiServisi isAkisiServisi)
    {
        _isAkisiServisi = isAkisiServisi;
    }

    public IActionResult Index()
    {
        var workflows = _isAkisiServisi.TumIsAkislari();
        return View(workflows);
    }

    /// <summary>Yeni iş akışı oluşturma formu (Issue #8)</summary>
    public IActionResult Create()
    {
        return View(new Workflow());
    }

    [HttpPost]
    public IActionResult Create(Workflow workflow, string? Departman, string? Tutar, string? Konu)
    {
        if (ModelState.IsValid)
        {
            // Form verilerini workflow'a ekle (AI karar motoru için)
            workflow.FormVerisi = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(Departman)) workflow.FormVerisi["Departman"] = Departman;
            if (!string.IsNullOrEmpty(Tutar)) workflow.FormVerisi["Tutar"] = Tutar;
            if (!string.IsNullOrEmpty(Konu)) workflow.FormVerisi["Konu"] = Konu;

            _isAkisiServisi.IsAkisiOlustur(workflow);
            return RedirectToAction(nameof(Details), new { id = workflow.Id });
        }
        return View(workflow);
    }

    /// <summary>İş akışı detaylarını gösterir (Issue #8)</summary>
    public IActionResult Details(string id)
    {
        var workflow = _isAkisiServisi.IsAkisiGetir(id);
        if (workflow == null) return NotFound();

        return View(workflow);
    }

    /// <summary>Belirli bir iş akışını başlatır (Issue #8)</summary>
    [HttpPost]
    public IActionResult Run(string id)
    {
        var workflow = _isAkisiServisi.IsAkisiGetir(id);
        if (workflow == null) return NotFound();

        // Workflow'dan gelen gerçek form verisini kullan (Tutar, Departman, Konu)
        var formVerisi = workflow.FormVerisi ?? new Dictionary<string, string>();

        var instance = _isAkisiServisi.IsAkisiBaslat(id, "USR-004", formVerisi);

        // Akış başladığında ilk adımı otomatik işletmeyi deneriz
        _isAkisiServisi.AdimiIslet(instance.Id).Wait();

        return RedirectToAction(nameof(Details), new { id = id });
    }

    [HttpGet]
    public IActionResult AddStep(string id)
    {
        var workflow = _isAkisiServisi.IsAkisiGetir(id);
        if (workflow == null) return NotFound();

        ViewBag.WorkflowId = id;
        ViewBag.WorkflowName = workflow.Ad;
        ViewBag.Konu = workflow.FormVerisi?.GetValueOrDefault("Konu", "Diğer") ?? "Diğer";
        return View(new WorkflowStep());
    }

    [HttpPost]
    public IActionResult AddStep(string id, WorkflowStep adim)
    {
        if (!string.IsNullOrEmpty(adim.Ad))
        {
            _isAkisiServisi.AdimEkle(id, adim);
            return RedirectToAction(nameof(Details), new { id = id });
        }

        var workflow = _isAkisiServisi.IsAkisiGetir(id);
        ViewBag.WorkflowId = id;
        ViewBag.WorkflowName = workflow?.Ad ?? "";
        return View(adim);
    }
}
