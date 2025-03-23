using System;
using System.Collections.Generic;
using DisasterPredictionAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace DisasterPredictionAPI.Contexts;

public partial class DBDEV : DbContext
{
    public DBDEV()
    {
    }

    public DBDEV(DbContextOptions<DBDEV> options)
        : base(options)
    {
    }

    public virtual DbSet<AlertSetting> AlertSettings { get; set; }

    public virtual DbSet<DisasterLog> DisasterLogs { get; set; }

    public virtual DbSet<Region> Regions { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AlertSetting>(entity =>
        {
            entity.HasKey(e => new { e.RegionId, e.AlertDisasterTypes });

            entity.ToTable("AlertSetting");

            entity.Property(e => e.RegionId)
                .HasMaxLength(10)
                .IsFixedLength()
                .HasColumnName("Region_ID");
            entity.Property(e => e.AlertDisasterTypes)
                .HasMaxLength(50)
                .HasColumnName("Alert_DisasterTypes");
            entity.Property(e => e.AlertThresholdScore).HasColumnName("Alert_ThresholdScore");
        });

        modelBuilder.Entity<DisasterLog>(entity =>
        {
            entity.HasKey(e => e.LogId);

            entity.ToTable("Disaster_LOG");

            entity.Property(e => e.LogId).HasColumnName("LOG_ID");
            entity.Property(e => e.CreateBy)
                .HasMaxLength(50)
                .HasColumnName("CREATE_BY");
            entity.Property(e => e.CreateDate)
                .HasColumnType("datetime")
                .HasColumnName("CREATE_DATE");
            entity.Property(e => e.LogDisasterType)
                .HasMaxLength(50)
                .HasColumnName("LOG_DISASTER_TYPE");
            entity.Property(e => e.LogLatitude)
                .HasColumnType("decimal(18, 4)")
                .HasColumnName("LOG_LATITUDE");
            entity.Property(e => e.LogLongitude)
                .HasColumnType("decimal(18, 4)")
                .HasColumnName("LOG_LONGITUDE");
            entity.Property(e => e.LogRegion)
                .HasMaxLength(50)
                .HasColumnName("LOG_REGION");
            entity.Property(e => e.LogRiskLevel)
                .HasMaxLength(50)
                .HasColumnName("LOG_RISK_LEVEL");
            entity.Property(e => e.LogRiskScore).HasColumnName("LOG_RISK_SCORE");
            entity.Property(e => e.Rev).HasColumnName("REV");
        });

        modelBuilder.Entity<Region>(entity =>
        {
            entity.HasKey(e => new { e.RegionId, e.RegionDisasterTypes }).HasName("PK_Table_1");

            entity.Property(e => e.RegionId)
                .HasMaxLength(10)
                .HasColumnName("Region_ID");
            entity.Property(e => e.RegionDisasterTypes)
                .HasMaxLength(50)
                .HasColumnName("Region_DisasterTypes");
            entity.Property(e => e.RegionLatitude)
                .HasColumnType("decimal(18, 4)")
                .HasColumnName("Region_Latitude");
            entity.Property(e => e.RegionLongitude)
                .HasColumnType("decimal(18, 4)")
                .HasColumnName("Region_Longitude");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
