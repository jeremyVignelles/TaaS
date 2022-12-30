using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using TaaS.DataAccess;
using TaaS.Models;

namespace TaaS.Controllers;

[Authorize]
public class TimelapseController : Controller
{
    [HttpGet("/Timelapse/{id}")]
    public async Task<IActionResult> ShowTimelapse(Guid id, [FromServices] TimelapseDataAccess timelapseDataAccess)
    {
        var timelapseImages = await timelapseDataAccess.GetTimelapseImages(id);
        return View(timelapseImages);
    }

    [HttpGet("/Timelapse/{id}/Record")]
    public async Task<IActionResult> RecordTimelapse(Guid id, [FromServices] TimelapseDataAccess timelapseDataAccess)
    {
        var timelapse = await timelapseDataAccess.GetTimelapseInfo(id);
        return View(new TimelapseViewModel(id, timelapse.Name, timelapse.LastNumber));
    }

    [HttpGet("/Timelapse/{id}/{number}.png")]
    public async Task<IActionResult> ShowTimelapseImage(Guid id, int number, [FromServices] TimelapseDataAccess timelapseDataAccess)
    {
        var timelapse = await timelapseDataAccess.GetTimelapseInfo(id);
        if (number > timelapse.LastNumber)
        {
            return NotFound();
        }
        var image = await timelapseDataAccess.GetTimelapseImage(id, number);
        var etagValue = $"\"{image.LastModified:O}\"";
        return File(image.Stream, "image/png", new DateTimeOffset(image.LastModified), new EntityTagHeaderValue(etagValue));
    }

    [HttpPost("/Timelapse/{id}/Record")]
    public async Task UploadTimelapseImage(Guid id, [FromServices] TimelapseDataAccess timelapseDataAccess)
    {
        await timelapseDataAccess.UploadTimelapseImage(id, Request.Body);
    }

    [HttpDelete("/Timelapse/{id}/{number}.png")]
    public async Task DeleteTimelapseImage(Guid id, int number, [FromServices] TimelapseDataAccess timelapseDataAccess)
    {
        await timelapseDataAccess.DeleteTimelapseImage(id, number);
    }
}