using System;
using Main.Modules.DisasterPredictionModule.Models;

namespace Main.Modules.DisasterPredictionModule.Entities;

public class Region
{
    public int Id { get; set; }
    public string Name { get; set; }
    public LocationCoordinate LocationCoordinate { get; set; }
}
