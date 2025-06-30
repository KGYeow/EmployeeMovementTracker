using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace EmpMovementTracker.Models;

public partial class TurnstileDbContext : DbContext
{
    public TurnstileDbContext()
    {
    }

    public TurnstileDbContext(DbContextOptions<TurnstileDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<EmployeeMovement> EmployeeMovements { get; set; }

    public virtual DbSet<EmployeeTimeTracking> EmployeeTimeTrackings { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=awase1pensql81;Database=TurnstileDB;User Id=sradmin;Password=sr;Encrypt=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<EmployeeMovement>(entity =>
        {
            entity.ToTable("EmployeeMovement");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.BuildingId).HasMaxLength(100);
            entity.Property(e => e.DateTime).HasColumnType("datetime");
            entity.Property(e => e.DepartmentCode).HasMaxLength(100);
            entity.Property(e => e.ShiftGroupId).HasMaxLength(100);
        });

        modelBuilder.Entity<EmployeeTimeTracking>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Employee__3214EC279FA3B657");

            entity.ToTable("EmployeeTimeTracking");

            entity.Property(e => e.Id).HasColumnName("ID");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
