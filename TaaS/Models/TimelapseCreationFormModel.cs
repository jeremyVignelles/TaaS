using System.ComponentModel.DataAnnotations;

namespace TaaS.Models;

public class TimelapseCreationFormModel
{
    [Display(Name = "Nom du Timelapse")]
    [Required]
    public string Name { get; set; } = "";
}