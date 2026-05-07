using FlowMind.Models;
using FlowMind.Data;
using FlowMind.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskStatus = FlowMind.Models.TaskStatus;

namespace FlowMind.Controllers;

public class TaskController : Controller
{
    private readonly FlowMindDbContext _context;
    private readonly AuditService _auditService;

    public TaskController(FlowMindDbContext context, AuditService auditService)
    {
        _context = context;
        _auditService = auditService;
    }

    public IActionResult Index()
    {
        var currentUserId = Request.Cookies["CurrentUserId"];

        var tasksQuery = _context.Tasks.AsQueryable();

        if (!string.IsNullOrEmpty(currentUserId))
        {
            tasksQuery = tasksQuery.Where(t => t.AtananKisiId == currentUserId);
        }

        var tasks = tasksQuery
            .OrderByDescending(t => t.Durum == TaskStatus.Atandı)
            .ThenByDescending(t => t.Oncelik)
            .ToList();

        return View(tasks);
    }

    [HttpPost]
    public IActionResult Approve(string id, string? not)
    {
        var task = _context.Tasks.FirstOrDefault(t => t.Id == id);
        if (task != null)
        {
            task.Durum = TaskStatus.Onaylandı;
            task.TamamlanmaTarihi = DateTime.Now;
            task.KullaniciNotu = not;

            // İlgili WorkflowInstance'ı güncelle
            if (!string.IsNullOrEmpty(task.WorkflowInstanceId))
            {
                var instance = _context.WorkflowInstances.FirstOrDefault(i => i.Id == task.WorkflowInstanceId);
                if (instance != null)
                {
                    var workflow = _context.Workflows
                        .Include(w => w.Adimlar)
                        .FirstOrDefault(w => w.Id == instance.WorkflowId);

                    instance.MevcutAdim++;

                    // Kalan adım var mı kontrol et
                    var kalanAdim = workflow?.Adimlar?.Any(a => a.Sira >= instance.MevcutAdim) ?? false;
                    if (!kalanAdim)
                    {
                        // Tüm adımlar tamamlandı
                        instance.Durum = FlowMind.Models.WorkflowStatus.Tamamlandı;
                        instance.BitisTarihi = DateTime.Now;
                    }
                    else
                    {
                        // Bir sonraki adıma geçiş için tekrar çalışıyor durumuna al
                        instance.Durum = FlowMind.Models.WorkflowStatus.Calisıyor;
                    }
                }
            }

            _auditService.Kaydet(
                task.AtananKisiId ?? "USR-000",
                task.AtananKisiAd ?? "Kullanıcı",
                "Görev onaylandı",
                "TaskItem", task.Id,
                $"Görev: {task.Baslik} | Akış: {task.WorkflowInstanceId}" +
                    (string.IsNullOrEmpty(not) ? "" : $" | Not: {not}"));
            _context.SaveChanges();
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public IActionResult Reject(string id, string? not)
    {
        var task = _context.Tasks.FirstOrDefault(t => t.Id == id);
        if (task != null)
        {
            task.Durum = TaskStatus.Reddedildi;
            task.TamamlanmaTarihi = DateTime.Now;
            task.KullaniciNotu = not;

            // İlgili WorkflowInstance'ı iptal et
            if (!string.IsNullOrEmpty(task.WorkflowInstanceId))
            {
                var instance = _context.WorkflowInstances.FirstOrDefault(i => i.Id == task.WorkflowInstanceId);
                if (instance != null)
                {
                    instance.Durum = FlowMind.Models.WorkflowStatus.İptalEdildi;
                    instance.BitisTarihi = DateTime.Now;
                }
            }

            _auditService.Kaydet(
                task.AtananKisiId ?? "USR-000",
                task.AtananKisiAd ?? "Kullanıcı",
                "Görev reddedildi",
                "TaskItem", task.Id,
                $"Görev: {task.Baslik} | Akış: {task.WorkflowInstanceId}" +
                    (string.IsNullOrEmpty(not) ? "" : $" | Not: {not}"));
            _context.SaveChanges();
        }
        return RedirectToAction(nameof(Index));
    }
}
