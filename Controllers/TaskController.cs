using FlowMind.Models;
using Microsoft.AspNetCore.Mvc;
using TaskStatus = FlowMind.Models.TaskStatus;

namespace FlowMind.Controllers;

public class TaskController : Controller
{
    // Mock veriler
    private static List<TaskItem> _tasks = new List<TaskItem>
    {
        new TaskItem 
        { 
            Id = "TSK-001", 
            Baslik = "Yüksek Tutarlı Satınalma Onayı", 
            Aciklama = "MacBook Pro M3 Max donanım talebi onayınızı bekliyor. Tutar: 120.000 TL",
            Durum = TaskStatus.Atandı,
            Oncelik = 5,
            YZTarafindan = true,
            OlusturmaTarihi = DateTime.Now.AddHours(-2),
            WorkflowInstanceId = "WFI-1234",
            AtananKisiAd = "Yönetici"
        },
        new TaskItem 
        { 
            Id = "TSK-002", 
            Baslik = "Sözleşme İncelemesi", 
            Aciklama = "Yeni tedarikçi sözleşmesinin hukuk departmanı tarafından incelenmesi.",
            Durum = TaskStatus.Atandı,
            Oncelik = 3,
            YZTarafindan = false,
            OlusturmaTarihi = DateTime.Now.AddDays(-1),
            SonTarih = DateTime.Now.AddHours(-5), // Süresi geçmiş
            WorkflowInstanceId = "WFI-5678",
            AtananKisiAd = "Hukuk"
        },
        new TaskItem 
        { 
            Id = "TSK-003", 
            Baslik = "İzin Talebi", 
            Aciklama = "Ahmet Yılmaz - Yıllık izin talebi (5 gün)",
            Durum = TaskStatus.Onaylandı,
            Oncelik = 2,
            YZTarafindan = true,
            OlusturmaTarihi = DateTime.Now.AddDays(-3),
            TamamlanmaTarihi = DateTime.Now.AddDays(-2),
            WorkflowInstanceId = "WFI-9012",
            AtananKisiAd = "İK Yöneticisi"
        }
    };

    public IActionResult Index()
    {
        return View(_tasks.OrderByDescending(t => t.Durum == TaskStatus.Atandı).ThenByDescending(t => t.Oncelik).ToList());
    }

    [HttpPost]
    public IActionResult Approve(string id, string? not)
    {
        var task = _tasks.FirstOrDefault(t => t.Id == id);
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
        var task = _tasks.FirstOrDefault(t => t.Id == id);
        if (task != null)
        {
            task.Durum = TaskStatus.Reddedildi;
            task.TamamlanmaTarihi = DateTime.Now;
            task.KullaniciNotu = not;
        }
        return RedirectToAction(nameof(Index));
    }
}
