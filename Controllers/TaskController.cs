using FlowMind.Models;
using FlowMind.Services;
using Microsoft.AspNetCore.Mvc;
using TaskStatus = FlowMind.Models.TaskStatus;

namespace FlowMind.Controllers;

public class TaskController : Controller
{
    private readonly InMemoryDataStore _store;

    public TaskController(InMemoryDataStore store)
    {
        _store = store;
    }

    public IActionResult Index()
    {
        var tasks = _store.GetAll(_store.Tasks)
            .OrderByDescending(t => t.Durum == TaskStatus.Atandı)
            .ThenByDescending(t => t.Oncelik)
            .ToList();
        return View(tasks);
    }

    [HttpPost]
    public IActionResult Approve(string id, string? not)
    {
        var task = _store.Get(_store.Tasks, id);
        if (task != null)
        {
            task.Durum = TaskStatus.Onaylandı;
            task.TamamlanmaTarihi = DateTime.Now;
            task.KullaniciNotu = not;
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public IActionResult Reject(string id, string? not)
    {
        var task = _store.Get(_store.Tasks, id);
        if (task != null)
        {
            task.Durum = TaskStatus.Reddedildi;
            task.TamamlanmaTarihi = DateTime.Now;
            task.KullaniciNotu = not;
        }
        return RedirectToAction(nameof(Index));
    }
}
