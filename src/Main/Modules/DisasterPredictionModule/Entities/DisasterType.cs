using System;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Main.Modules.DisasterPredictionModule.Entities;

public class DisasterType
{
    public int Id { get; set; }
    public string Name { get; set; }
}
public class DisasterTypeConfiguration : IEntityTypeConfiguration<DisasterType>
{
    public void Configure(EntityTypeBuilder<DisasterType> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Name).IsRequired();
        builder.HasIndex(p => p.Name);
    }
}