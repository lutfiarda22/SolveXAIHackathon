using FlowMind.Models;
using FlowMind.Data;
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
    public IActionResult Create(Workflow workflow, string? Departman, string? Tutar, string? Konu, string? IzinSebebi, string? IadeSebebi)
    {
        if (ModelState.IsValid)
        {
            // Form verilerini workflow'a ekle (AI karar motoru için)
            workflow.FormVerisi = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(Departman)) workflow.FormVerisi["Departman"] = Departman;
            if (!string.IsNullOrEmpty(Tutar)) workflow.FormVerisi["Tutar"] = Tutar;
            if (!string.IsNullOrEmpty(Konu)) workflow.FormVerisi["Konu"] = Konu;
            if (!string.IsNullOrEmpty(IzinSebebi)) workflow.FormVerisi["IzinSebebi"] = IzinSebebi;
            if (!string.IsNullOrEmpty(IadeSebebi)) workflow.FormVerisi["IadeSebebi"] = IadeSebebi;

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

        var context = HttpContext.RequestServices.GetRequiredService<FlowMindDbContext>();
        var instance = context.WorkflowInstances.OrderByDescending(i => i.BaslangicTarihi).FirstOrDefault(i => i.WorkflowId == id);
        ViewBag.Instance = instance;

        return View(workflow);
    }

    /// <summary>Belirli bir iş akışını başlatır (Issue #8)</summary>
    [HttpPost]
    public IActionResult Run(string id)
    {
        var workflow = _isAkisiServisi.IsAkisiGetir(id);
        if (workflow == null) return NotFound();

        var context = HttpContext.RequestServices.GetRequiredService<FlowMindDbContext>();

        // Zaten çalışan/bekleyen/tamamlanmış bir instance varsa tekrar oluşturma
        var mevcutInstance = context.WorkflowInstances
            .FirstOrDefault(i => i.WorkflowId == id);
        if (mevcutInstance != null)
        {
            // Mevcut sonuç sayfasına yönlendir
            var mevcutDecision = context.AIDecisions
                .OrderByDescending(d => d.KararZamani)
                .FirstOrDefault(d => d.WorkflowInstanceId == mevcutInstance.Id);
            return RedirectToAction(nameof(RunResult), new { id = id, instanceId = mevcutInstance.Id, decisionId = mevcutDecision?.Id });
        }

        var formVerisi = workflow.FormVerisi ?? new Dictionary<string, string>();

        var currentUserId = Request.Cookies["CurrentUserId"] ?? "USR-004";
        var instance = _isAkisiServisi.IsAkisiBaslat(id, currentUserId, formVerisi);

        var decision = _isAkisiServisi.AdimiIslet(instance.Id).Result;

        return RedirectToAction(nameof(RunResult), new { id = id, instanceId = instance.Id, decisionId = decision?.Id });
    }

    /// <summary>AI karar sonucunu gösterir</summary>
    public IActionResult RunResult(string id, string instanceId, string? decisionId)
    {
        var workflow = _isAkisiServisi.IsAkisiGetir(id);
        if (workflow == null) return NotFound();

        ViewBag.Workflow = workflow;
        ViewBag.InstanceId = instanceId;

        // AI kararını bul
        FlowMind.Models.AIDecision? decision = null;
        if (!string.IsNullOrEmpty(decisionId))
        {
            var context = HttpContext.RequestServices.GetRequiredService<FlowMindDbContext>();
            decision = context.AIDecisions.FirstOrDefault(d => d.Id == decisionId);
        }
        ViewBag.Decision = decision;

        return View();
    }
    /// <summary>Tüm iş akışlarının durumlarını görevlere göre senkronize eder</summary>
    [HttpPost]
    public IActionResult Refresh()
    {
        var context = HttpContext.RequestServices.GetRequiredService<FlowMindDbContext>();
        var instances = context.WorkflowInstances.ToList();

        foreach (var instance in instances)
        {
            // Zaten tamamlanmış veya iptal edilmişse atla
            if (instance.Durum == WorkflowStatus.Tamamlandı || instance.Durum == WorkflowStatus.İptalEdildi)
                continue;

            // Bu instance'a ait bekleyen görev var mı?
            var bekleyenGorev = context.Tasks
                .Any(t => t.WorkflowInstanceId == instance.Id && t.Durum == FlowMind.Models.TaskStatus.Atandı);

            if (bekleyenGorev)
                continue; // Hâlâ bekleyen görev var, dokunma

            // Tüm görevler onaylanmış mı?
            var tumGorevler = context.Tasks
                .Where(t => t.WorkflowInstanceId == instance.Id)
                .ToList();

            if (tumGorevler.Any())
            {
                var hepsiOnaylandi = tumGorevler.All(t => t.Durum == FlowMind.Models.TaskStatus.Onaylandı);
                var birRedVar = tumGorevler.Any(t => t.Durum == FlowMind.Models.TaskStatus.Reddedildi);

                if (hepsiOnaylandi)
                {
                    instance.Durum = WorkflowStatus.Tamamlandı;
                    instance.BitisTarihi = DateTime.Now;
                }
                else if (birRedVar)
                {
                    instance.Durum = WorkflowStatus.İptalEdildi;
                    instance.BitisTarihi = DateTime.Now;
                }
            }
        }

        context.SaveChanges();
        return RedirectToAction(nameof(Index));
    }

}
