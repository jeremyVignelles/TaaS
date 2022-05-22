namespace TaaS.Models;

/// <summary>
/// The information stored in the index.json file.
/// </summary>
/// <param name="Name">The timelapse displayed name</param>
/// <param name="LastNumber">The last photo file index taken (-1 if no photo is taken)</param>
public record TimelapseInfo(string Name, int LastNumber);

/// <summary>
/// The model used in the view that represents a time lapse
/// </summary>
/// <param name="Id">The timelapse identifier</param>
/// <param name="Name">The timelapse displayed name</param>
/// <param name="LastNumber">The last photo file index taken (-1 if no photo is taken)</param>
public record TimelapseViewModel(Guid Id, string Name, int LastNumber);