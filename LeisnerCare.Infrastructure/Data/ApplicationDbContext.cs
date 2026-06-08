using LeisnerCare.Core.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LeisnerCare.Infrastructure.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Patient> Patients => Set<Patient>();
    public DbSet<Symptom> Symptoms => Set<Symptom>();
    public DbSet<Medication> Medications => Set<Medication>();
    public DbSet<MedicationLog> MedicationLogs => Set<MedicationLog>();
    public DbSet<Observation> Observations => Set<Observation>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();


    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Patient konfiguration
        builder.Entity < Patient > (entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.CprNumber).IsUnique();
            entity.Property(e => e.CprNumber).HasMaxLength(10);

            entity.HasOne(e => e.User)
                  .WithOne()
                  .HasForeignKey < Patient > (e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne < ApplicationUser > ()
                .WithMany()
                .HasForeignKey(e => e.RelativeUserId)
                .OnDelete(DeleteBehavior.NoAction);
        });

        // Symptom konfiguration
        builder.Entity < Symptom > (entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Note).HasMaxLength(500);
            entity.HasOne(e => e.Patient)
                  .WithMany()
                  .HasForeignKey(e => e.PatientId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Medication konfiguration
        builder.Entity < Medication > (entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Dosage).HasMaxLength(50);
            entity.HasOne(e => e.Patient)
                  .WithMany()
                  .HasForeignKey(e => e.PatientId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // MedicationLog konfiguration
        builder.Entity < MedicationLog > (entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Medication)
                  .WithMany(m => m.Logs)
                  .HasForeignKey(e => e.MedicationId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Observation konfiguration
        builder.Entity < Observation > (entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Content).HasMaxLength(2000);
            entity.Property(e => e.AuthorName).HasMaxLength(100);
            entity.Property(e => e.AuthorRole).HasMaxLength(20);
            entity.HasOne(e => e.Patient)
                  .WithMany()
                  .HasForeignKey(e => e.PatientId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // AuditLog konfiguration
        builder.Entity < AuditLog > (entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Action).HasMaxLength(50);
            entity.Property(e => e.EntityType).HasMaxLength(50);
            entity.Property(e => e.UserName).HasMaxLength(100);
            entity.Property(e => e.Details).HasMaxLength(1000);
            entity.Property(e => e.PatientId);
        });

    }
}