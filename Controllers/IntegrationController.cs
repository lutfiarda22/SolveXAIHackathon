using FlowMind.Models;
using Microsoft.AspNetCore.Mvc;

namespace FlowMind.Controllers;

public class IntegrationController : Controller
{
    // Mock API logs
    private static List<IntegrationLog> _logs = new List<IntegrationLog>
    {
        new IntegrationLog
        {
            Id = "INT-101",
            WorkflowInstanceId = "WFI-1234",
            HedefSistem = IntegrationType.CRM,
            Islem = "MüşteriKaydıOluştur",
            GonderilenVeri = "{ \"Ad\": \"Ahmet\", \"Soyad\": \"Yılmaz\" }",
            Yanit = "{ \"CrmId\": \"C-98765\", \"Status\": \"Created\" }",
            Durum = IntegrationStatus.Basarili,
            SureMs = 145,
            Zaman = DateTime.Now.AddMinutes(-15)
        },
        new IntegrationLog
        {
            Id = "INT-102",
            WorkflowInstanceId = "WFI-5678",
            HedefSistem = IntegrationType.ERP,
            Islem = "StokKontrol",
            GonderilenVeri = "{ \"UrunKodu\": \"PRD-001\" }",
            Yanit = "Timeout",
            Durum = IntegrationStatus.Basarisiz,
            SureMs = 5000,
            HataMesaji = "Sunucu yanıt vermedi (Timeout).",
            Zaman = DateTime.Now.AddHours(-1)
        },
        new IntegrationLog
        {
            Id = "INT-103",
            WorkflowInstanceId = "WFI-9012",
            HedefSistem = IntegrationType.Email,
            Islem = "EpostaGonder",
            GonderilenVeri = "{ \"To\": \"ahmet@example.com\", \"Subject\": \"Onayınız Bekleniyor\" }",
            Yanit = "{\"Status\": \"Queued\", \"MessageId\": \"msg_01\"}",
            Durum = IntegrationStatus.Basarili,
            SureMs = 80,
            Zaman = DateTime.Now.AddMinutes(-5)
        }
    };

    public IActionResult Index()
    {
        return View(_logs.OrderByDescending(l => l.Zaman).ToList());
    }
}
