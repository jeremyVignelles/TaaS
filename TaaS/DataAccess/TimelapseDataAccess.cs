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

    public Task<Guid[]> GetTimelapses()
    {
        return Task.FromResult(Directory.GetDirectories(this._dataFolder).Select(x => Guid.Parse(Path.GetFileName(x))).ToArray());
    }
}