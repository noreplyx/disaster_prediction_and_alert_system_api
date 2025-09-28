using System;
using System.ComponentModel.DataAnnotations;

namespace Main.Modules.DisasterPredictionModule.Models.Requests;

public class AddOrUpdateRegionRequest
{
    [Required]
    public string RegionId { get; set; }
    [Required]
    public LocationCoordinate LocationCoordinates { get; set; }
    [Required]
    public IEnumerable<string> DisasterTypes { get; set; }
}
