using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using TaaS.DataAccess;
using TaaS.Models;

namespace TaaS.Controllers;
[Authorize]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public async Task<IActionResult> Index([FromServices] TimelapseDataAccess timelapseDataAccess)
    {
        var timelapses = await timelapseDataAccess.GetTimelapses();
        var mapping = new Dictionary<Guid, TimelapseInfo>(
            await Task.WhenAll(timelapses
                .Select(async timelapse => new KeyValuePair<Guid,TimelapseInfo>(timelapse, await timelapseDataAccess.GetTimelapseInfo(timelapse))))
        );
        return View(mapping);
    }

    public IActionResult Create()
    {
        return View(new TimelapseCreationFormModel());
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromServices]TimelapseDataAccess timelapseDataAccess, TimelapseCreationFormModel model)
    {
        if (ModelState.IsValid)
        {
            await timelapseDataAccess.CreateTimelapse(model.Name);
            return RedirectToAction(nameof(Index));
        }
        return View(model);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}