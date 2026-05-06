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

    public IActionResult Create()
    {
        return View(new Workflow());
    }

    [HttpPost]
    public IActionResult Create(Workflow workflow)
    {
        if (ModelState.IsValid)
        {
            _isAkisiServisi.IsAkisiOlustur(workflow);
            return RedirectToAction(nameof(Index));
        }
        return View(workflow);
    }

    public IActionResult Details(string id)
    {
        var workflow = _isAkisiServisi.IsAkisiGetir(id);
        if (workflow == null) return NotFound();

        return View(workflow);
    }

    [HttpPost]
    public IActionResult Run(string id)
    {
        var formVerisi = new Dictionary<string, string>
        {
            { "Otomatik", "Demo" } // Hackathon için varsayılan form verisi
        };
        var instance = _isAkisiServisi.IsAkisiBaslat(id, "USR-004", formVerisi);

        // Akış başladığında ilk adımı otomatik işletmeyi deneriz
        _isAkisiServisi.AdimiIslet(instance.Id).Wait();

        return RedirectToAction(nameof(Index)); // veya Dashboard
    }
}
