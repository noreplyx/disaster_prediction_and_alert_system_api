using System;
using Main.Modules.DisasterPredictionModule.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Main.Modules.DisasterPredictionModule.Entities;

public class RegionDisasterConfiguration
{
    public int Id { get; set; }
    public int RegionId { get; set; }
    public Region Region { get; set; }
    public int DisasterTypeId { get; set; }
    public DisasterType DisasterType { get; set; }
    public double Threshold { get; set; } = 0;
}
public class RegionDisasterConfigurationConfiguration : IEntityTypeConfiguration<RegionDisasterConfiguration>
{
    public void Configure(EntityTypeBuilder<RegionDisasterConfiguration> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.RegionId).IsRequired();
        builder.Property(p => p.DisasterTypeId).IsRequired();
    }
}
