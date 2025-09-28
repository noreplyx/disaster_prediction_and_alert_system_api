using System;
using Main.Common.Models;
using Main.Modules.DisasterPredictionModule.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Main.Common.Persistence.DatabaseContext;

public class PostgreSqlDbContext : DbContext
{
    private readonly IOptionsMonitor<AppSettingModel> _appSettingModel;
    public PostgreSqlDbContext(
        IOptionsMonitor<AppSettingModel> appSettingModel
    )
    {
        _appSettingModel = appSettingModel;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder
        .UseNpgsql(_appSettingModel.CurrentValue.ConnectionStrings.PostgreSQL);
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PostgreSqlDbContext).Assembly);
    }

    public DbSet<DisasterType> DisasterTypes { get; set; }
    public DbSet<Region> Regions { get; set; }
    public DbSet<RegionDisasterConfiguration> RegionDisasterConfigurations { get; set; }
    public DbSet<RegionAlertRecord> RegionAlertRecords { get; set; }
}
