using FlowMind.Models;
using FlowMind.Services;
using Microsoft.AspNetCore.Mvc;

namespace FlowMind.Controllers;

public class IntegrationController : Controller
{
    private readonly InMemoryDataStore _store;

    public IntegrationController(InMemoryDataStore store)
    {
        _store = store;
    }

    public IActionResult Index()
    {
        var logs = _store.GetAll(_store.IntegrationLogs).OrderByDescending(l => l.Zaman).ToList();
        return View(logs);
    }
}
