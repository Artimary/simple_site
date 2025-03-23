using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Data;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using simple_site.Models;

namespace Controllers
{
public class HomeController : Controller
{
    private readonly IMouseService _mouseService;
    private readonly ILogger<HomeController> _logger;

    public HomeController(
        IMouseService mouseService,
        ILogger<HomeController> logger)
    {
        _mouseService = mouseService;
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Save([FromBody] List<MouseEvent> events)
    {
        _logger.LogInformation($"Received {events?.Count ?? 0} events");
            
            if (events != null && events.Any())
            {
                var firstEvent = events.First();
                _logger.LogInformation($"First event details: X={firstEvent.X}, Y={firstEvent.Y}, Time={firstEvent.T}");
                await _mouseService.SaveDataAsync(events);
            }

            return Ok();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
}
