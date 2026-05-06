using FlowMind.Models;
using FlowMind.Data;
using FlowMind.Services;
using Microsoft.AspNetCore.Mvc;

namespace FlowMind.Controllers;

public class IntegrationController : Controller
{
    private readonly FlowMindDbContext _context;

    public IntegrationController(FlowMindDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        var logs = _context.IntegrationLogs.OrderByDescending(l => l.Zaman).ToList();
        return View(logs);
    }
}
