using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace SQLRepository.Models;

public partial class DatariskContext : DbContext
{
    public DatariskContext()
    {
    }

    public DatariskContext(DbContextOptions<DatariskContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ExecutionScript> ExecutionScripts { get; set; }

    public virtual DbSet<Script> Scripts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        
        modelBuilder.Entity<ExecutionScript>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("execution_script_pkey");

            entity.ToTable("execution_script");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.EndTime).HasColumnName("end_time");
            entity.Property(e => e.Request).HasColumnName("request");
            entity.Property(e => e.Response).HasColumnName("response");
            entity.Property(e => e.ScriptId).HasColumnName("script_id");
            entity.Property(e => e.StartTime).HasColumnName("start_time");
            entity.Property(e => e.Status).HasColumnName("status");

            entity.HasOne(d => d.Script).WithMany(p => p.ExecutionScripts)
                .HasForeignKey(d => d.ScriptId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("execution_script_script_id_fkey");
        });

        modelBuilder.Entity<Script>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("script_pkey");

            entity.ToTable("script");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.DateAdd).HasColumnName("date_add");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.ScriptName)
                .HasMaxLength(100)
                .HasColumnName("script_name");
            entity.Property(e => e.Accepted)
                .HasColumnName("accepted");
            entity.Property(e => e.Errors)
                .HasColumnName("errors");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
