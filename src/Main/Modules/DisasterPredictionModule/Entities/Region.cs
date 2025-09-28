using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NetTopologySuite.Geometries;

namespace Main.Modules.DisasterPredictionModule.Entities;

public class Region
{
    public int Id { get; set; }
    public string Name { get; set; }
    public double Lat { get; set; }
    public double Lon { get; set; }
}
public class RegionConfiguration : IEntityTypeConfiguration<Region>
{
    public void Configure(EntityTypeBuilder<Region> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Name).IsRequired();

        builder.HasIndex(p => p.Name);
    }
}
