using System.Text.Json;
using TaaS.Models;

namespace TaaS.DataAccess;

public class TimelapseDataAccess
{
    private readonly string _dataFolder;
    public TimelapseDataAccess(IConfiguration configuration)
    {
        this._dataFolder = configuration.GetValue<string>("DataFolder");
        if (!Directory.Exists(_dataFolder))
        {
            Directory.CreateDirectory(_dataFolder);
        }
    }

    public async Task CreateTimelapse(string name)
    {
        var id = Guid.NewGuid().ToString();
        Directory.CreateDirectory($"{this._dataFolder}/{id}");
        var info = new TimelapseInfo(name, -1);
        await File.WriteAllTextAsync($"{this._dataFolder}/{id}/index.json", JsonSerializer.Serialize(info));
    }

    public async Task<TimelapseInfo> GetTimelapseInfo(Guid id)
    {
        var info = await File.ReadAllTextAsync($"{this._dataFolder}/{id}/index.json");
        return JsonSerializer.Deserialize<TimelapseInfo>(info)!;
    }

    public async Task UploadTimelapseImage(Guid id, Stream content)
    {
        var info = await this.GetTimelapseInfo(id);
        var newInfo = info with { LastNumber = info.LastNumber + 1 };
        var fileName = $"{this._dataFolder}/{id}/{newInfo.LastNumber}.png";
        await using var fileStream = File.Create(fileName);
        await content.CopyToAsync(fileStream);
        await File.WriteAllTextAsync($"{this._dataFolder}/{id}/index.json", JsonSerializer.Serialize(newInfo));
    }

    public Task<Stream> GetTimelapseImage(Guid id, int number)
    {
        var fileName = $"{this._dataFolder}/{id}/{number}.png";
        return Task.FromResult((Stream)File.OpenRead(fileName));
    }

    public async Task<TimelapseImagesViewModel> GetTimelapseImages(Guid id)
    {
        var timelapse = await this.GetTimelapseInfo(id);
        var images = new List<TimelapseImage>();
        for (var i = 0; i <= timelapse.LastNumber; i++)
        {
            var creationTime = new FileInfo($"{this._dataFolder}/{id}/{i}.png").CreationTime;
            images.Add(new TimelapseImage(i, creationTime));
        }
        return new TimelapseImagesViewModel(id, timelapse.Name, images.ToArray());
    }

	public Task<Guid[]> GetTimelapses()
    {
        return Task.FromResult(Directory.GetDirectories(this._dataFolder).Select(x => Guid.Parse(Path.GetFileName(x))).ToArray());
    }

    public async Task DeleteTimelapseImage(Guid id, int number)
    {
        var info = await this.GetTimelapseInfo(id);
        var fileName = $"{this._dataFolder}/{id}/{number}.png";
        File.Move(fileName, fileName + ".old");
        for (var i = number + 1; i <= info.LastNumber; i++)
        {
            var oldName = $"{this._dataFolder}/{id}/{i}.png";
            var newName = $"{this._dataFolder}/{id}/{i-1}.png";
            File.Move(oldName, newName);
        }
        File.Delete(fileName + ".old");
        await File.WriteAllTextAsync($"{this._dataFolder}/{id}/index.json", JsonSerializer.Serialize(info with { LastNumber = info.LastNumber - 1 }));
    }
}