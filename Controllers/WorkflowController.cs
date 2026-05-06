using FlowMind.Models;
using Microsoft.AspNetCore.Mvc;

namespace FlowMind.Controllers;

public class WorkflowController : Controller
{
    // Geçici olarak UI tarafını gösterebilmek için sahte veriler
    private static List<Workflow> _workflows = new List<Workflow>
    {
        new Workflow 
        { 
            Id = "WF-101", 
            Ad = "İşe Alım Süreci", 
            Aciklama = "Yeni personel işe alım ve onboarding adımları",
            Tetikleyici = "Aday Onaylandı",
            Aktif = true,
            OlusturmaTarihi = DateTime.Now.AddDays(-2),
            Adimlar = new List<WorkflowStep>
            {
                new WorkflowStep { Id = "S1", Ad = "Sözleşme Gönder", EylemTuru = StepActionType.Bildirim, Sira = 1 },
                new WorkflowStep { Id = "S2", Ad = "IT Ekipman Talebi", EylemTuru = StepActionType.Entegrasyon, EntegrasyonTuru = IntegrationType.ERP, Sira = 2 }
            }
        },
        new Workflow 
        { 
            Id = "WF-102", 
            Ad = "Satınalma Onayı", 
            Aciklama = "Belirli bir tutar üzerindeki satınalmalar için yöneticiden onay alma süreci",
            Tetikleyici = "Talep Oluşturuldu",
            Aktif = true,
            OlusturmaTarihi = DateTime.Now.AddDays(-5),
            Adimlar = new List<WorkflowStep>
            {
                new WorkflowStep { Id = "S1", Ad = "Yönetici Onayı", EylemTuru = StepActionType.Onay, Sira = 1 },
                new WorkflowStep { Id = "S2", Ad = "Sipariş Geç", EylemTuru = StepActionType.Entegrasyon, EntegrasyonTuru = IntegrationType.ERP, Sira = 2 }
            }
        }
    };

    public IActionResult Index()
    {
        return View(_workflows);
    }

    public IActionResult Create()
    {
        return View(new Workflow());
    }

    [HttpPost]
    public IActionResult Create(Workflow workflow)
    {
        if (ModelState.IsValid)
        {
            workflow.Id = "WF-" + new Random().Next(100, 999);
            workflow.OlusturmaTarihi = DateTime.Now;
            _workflows.Add(workflow);
            return RedirectToAction(nameof(Index));
        }
        return View(workflow);
    }

    public IActionResult Details(string id)
    {
        var workflow = _workflows.FirstOrDefault(w => w.Id == id);
        if (workflow == null) return NotFound();

        return View(workflow);
    }
}
