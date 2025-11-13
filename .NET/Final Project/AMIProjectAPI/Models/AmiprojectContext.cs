using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace AMIProjectAPI.Models;

public partial class AmiprojectContext : DbContext
{
    public AmiprojectContext()
    {
    }

    public AmiprojectContext(DbContextOptions<AmiprojectContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Bill> Bills { get; set; }

    public virtual DbSet<Consumer> Consumers { get; set; }

    public virtual DbSet<ConsumerLogin> ConsumerLogins { get; set; }

    public virtual DbSet<DailyConsumption> DailyConsumptions { get; set; }

    public virtual DbSet<Meter> Meters { get; set; }

    public virtual DbSet<MonthlyConsumption> MonthlyConsumptions { get; set; }

    public virtual DbSet<OrgUnit> OrgUnits { get; set; }

    public virtual DbSet<Tariff> Tariffs { get; set; }

    public virtual DbSet<TariffSlab> TariffSlabs { get; set; }

    public virtual DbSet<TodRule> TodRules { get; set; }

    public virtual DbSet<TriggerErrorLog> TriggerErrorLogs { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Bill>(entity =>
        {
            entity.HasKey(e => e.BillId).HasName("PK__Bill__11F2FC4A2840D5FB");

            entity.ToTable("Bill");

            entity.Property(e => e.BillId).HasColumnName("BillID");
            entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.BaseRate).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Category).HasMaxLength(50);
            entity.Property(e => e.GeneratedAt)
                .HasPrecision(3)
                .HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.MeterId)
                .HasMaxLength(50)
                .HasColumnName("MeterID");
            entity.Property(e => e.MonthlyConsumptionkWh).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.SlabRate).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Pending");
            entity.Property(e => e.TaxRate).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.Meter).WithMany(p => p.Bills)
                .HasForeignKey(d => d.MeterId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Bill__MeterID__40F9A68C");
        });

        modelBuilder.Entity<Consumer>(entity =>
        {
            entity.HasKey(e => e.ConsumerId).HasName("PK__Consumer__63BBE99A5C9FCA66");

            entity.ToTable("Consumer", tb => tb.HasTrigger("trg_Update_ConsumerTimestamp"));

            entity.Property(e => e.ConsumerId).HasColumnName("ConsumerID");
            entity.Property(e => e.Address).HasMaxLength(255);
            entity.Property(e => e.CreatedAt)
                .HasPrecision(3)
                .HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(100)
                .HasDefaultValue("admin");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("Active");
            entity.Property(e => e.UpdatedAt).HasPrecision(3);
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
        });

        modelBuilder.Entity<ConsumerLogin>(entity =>
        {
            entity.HasKey(e => e.ConsumerLoginId).HasName("PK__Consumer__E6D708D7B043FE72");

            entity.ToTable("ConsumerLogin");

            entity.HasIndex(e => e.Username, "UQ__Consumer__536C85E4C807DF73").IsUnique();

            entity.HasIndex(e => e.ConsumerId, "UQ__Consumer__63BBE99B92114195").IsUnique();

            entity.Property(e => e.ConsumerLoginId).HasColumnName("ConsumerLoginID");
            entity.Property(e => e.ConsumerId).HasColumnName("ConsumerID");
            entity.Property(e => e.IsVerified).HasDefaultValue(false);
            entity.Property(e => e.LastLogin).HasPrecision(3);
            entity.Property(e => e.Password).HasMaxLength(255);
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Active");
            entity.Property(e => e.Username).HasMaxLength(50);

            entity.HasOne(d => d.Consumer).WithOne(p => p.ConsumerLogin)
                .HasForeignKey<ConsumerLogin>(d => d.ConsumerId)
                .HasConstraintName("FK__ConsumerL__Consu__114A936A");
        });

        modelBuilder.Entity<DailyConsumption>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("DailyConsumption", tb => tb.HasTrigger("trg_UpdateMonthlyConsumption"));

            entity.Property(e => e.ConsumptionkWh).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.MeterId)
                .HasMaxLength(50)
                .HasColumnName("MeterID");

            entity.HasOne(d => d.Meter).WithMany()
                .HasForeignKey(d => d.MeterId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DailyCons__Meter__3B40CD36");
        });

        modelBuilder.Entity<Meter>(entity =>
        {
            entity.HasKey(e => e.MeterSerialNo).HasName("PK__Meter__5C498B0F46828ACB");

            entity.ToTable("Meter");

            entity.HasIndex(e => e.ConsumerId, "IX_Meter_ConsumerID");

            entity.HasIndex(e => e.OrgUnitId, "IX_Meter_OrgUnitId");

            entity.Property(e => e.MeterSerialNo).HasMaxLength(50);
            entity.Property(e => e.Category).HasMaxLength(50);
            entity.Property(e => e.ConsumerId).HasColumnName("ConsumerID");
            entity.Property(e => e.Firmware).HasMaxLength(20);
            entity.Property(e => e.Iccid)
                .HasMaxLength(50)
                .HasColumnName("ICCID");
            entity.Property(e => e.Imsi)
                .HasMaxLength(50)
                .HasColumnName("IMSI");
            entity.Property(e => e.InstallDate).HasPrecision(3);
            entity.Property(e => e.Ipaddress)
                .HasMaxLength(50)
                .HasColumnName("IPAddress");
            entity.Property(e => e.Manufacturer).HasMaxLength(100);
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("Active");

            entity.HasOne(d => d.Consumer).WithMany(p => p.Meters)
                .HasForeignKey(d => d.ConsumerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Meter__ConsumerI__7D439ABD");

            entity.HasOne(d => d.OrgUnit).WithMany(p => p.Meters)
                .HasForeignKey(d => d.OrgUnitId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Meter__OrgUnitId__7F2BE32F");
        });

        modelBuilder.Entity<MonthlyConsumption>(entity =>
        {
            entity.HasKey(e => new { e.MeterId, e.MonthStartDate });

            entity.ToTable("MonthlyConsumption", tb => tb.HasTrigger("trg_AutoGenerateBill"));

            entity.Property(e => e.MeterId)
                .HasMaxLength(50)
                .HasColumnName("MeterID");
            entity.Property(e => e.ConsumptionkWh).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.Meter).WithMany(p => p.MonthlyConsumptions)
                .HasForeignKey(d => d.MeterId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__MonthlyCo__Meter__3E1D39E1");
        });

        modelBuilder.Entity<OrgUnit>(entity =>
        {
            entity.HasKey(e => e.OrgUnitId).HasName("PK__OrgUnit__4A793B8EFD7ED6C3");

            entity.ToTable("OrgUnit");

            entity.Property(e => e.OrgUnitId).HasColumnName("OrgUnitID");
            entity.Property(e => e.Dtr)
                .HasMaxLength(100)
                .HasColumnName("DTR");
            entity.Property(e => e.Feeder).HasMaxLength(100);
            entity.Property(e => e.Substation).HasMaxLength(100);
            entity.Property(e => e.Zone).HasMaxLength(100);
        });

        modelBuilder.Entity<Tariff>(entity =>
        {
            entity.HasKey(e => e.TariffId).HasName("PK__Tariff__EBAF9D93007DC959");

            entity.ToTable("Tariff");

            entity.Property(e => e.TariffId).HasColumnName("TariffID");
            entity.Property(e => e.BaseRate).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.TariffName).HasMaxLength(100);
            entity.Property(e => e.TaxRate).HasColumnType("decimal(10, 2)");
        });

        modelBuilder.Entity<TariffSlab>(entity =>
        {
            entity.HasKey(e => e.SlabId).HasName("PK__TariffSl__D61699213DA804C9");

            entity.ToTable("TariffSlab");

            entity.HasIndex(e => e.TariffId, "IX_TariffSlab_TariffID");

            entity.Property(e => e.FromKwh).HasColumnType("decimal(18, 6)");
            entity.Property(e => e.RatePerKwh).HasColumnType("decimal(18, 6)");
            entity.Property(e => e.TariffId).HasColumnName("TariffID");
            entity.Property(e => e.ToKwh).HasColumnType("decimal(18, 6)");

            entity.HasOne(d => d.Tariff).WithMany(p => p.TariffSlabs)
                .HasForeignKey(d => d.TariffId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TariffSla__Tarif__72C60C4A");
        });

        modelBuilder.Entity<TodRule>(entity =>
        {
            entity.Property(e => e.Multiplier).HasColumnType("decimal(10, 4)");
        });

        modelBuilder.Entity<TriggerErrorLog>(entity =>
        {
            entity.HasKey(e => e.LogId).HasName("PK__TriggerE__5E548648A31C2E33");

            entity.ToTable("TriggerErrorLog");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.ErrorProcedure).HasMaxLength(200);
            entity.Property(e => e.TriggerName).HasMaxLength(200);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CCAC457CD7F8");

            entity.HasIndex(e => e.Username, "UQ__Users__536C85E441FF4D7C").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.DisplayName).HasMaxLength(100);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.LastLogin).HasColumnType("datetime");
            entity.Property(e => e.Password).HasMaxLength(255);
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Active");
            entity.Property(e => e.Username).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
