using System;
using Main.Modules.DisasterPredictionModule.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Main.Modules.DisasterPredictionModule.Entities;

public class RegionAlertRecord
{
    public int Id { get; set; }
    public AlertType AlertType { get; set; }
    public int RegionId { get; set; }
    public Region Region { get; set; }
    public int DisasterTypeId { get; set; }
    public DisasterType DisasterType { get; set; }
    public RiskLevel RiskLevel { get; set; }
    public double RiskScore { get; set; }
    public double Threshold { get; set; }
    public string Content { get; set; }
    public DateTimeOffset CreateDate { get; set; }
    public DateTimeOffset UpdateDate { get; set; }
    public AlertStatus Status { get; set; }
}
public class RegionAlertRecordConfiguration : IEntityTypeConfiguration<RegionAlertRecord>
{
    public void Configure(EntityTypeBuilder<RegionAlertRecord> builder)
    {
        builder.HasKey(p => p.Id);
    }
}
