using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using QuestPDF.Helpers;
using LeisnerCare.Core.Entities;
using LeisnerCare.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LeisnerCare.Web.Services;

public class PdfReportService
{
    private readonly ApplicationDbContext _context;

    public PdfReportService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<byte[]> GeneratePatientReportAsync(int patientId)
    {
        var patient = await _context.Patients
            .Include(p => p.User)
            .FirstOrDefaultAsync(p => p.Id == patientId);

        if (patient == null) throw new Exception("Patient ikke fundet");

        var symptoms = await _context.Symptoms
            .Where(s => s.PatientId == patientId)
            .OrderByDescending(s => s.RecordedAt)
            .Take(30)
            .ToListAsync();

        var medications = await _context.Medications
            .Where(m => m.PatientId == patientId)
            .Include(m => m.Logs)
            .ToListAsync();

        var observations = await _context.Observations
            .Where(o => o.PatientId == patientId)
            .OrderByDescending(o => o.CreatedAt)
            .Take(20)
            .ToListAsync();

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.DefaultTextStyle(x => x.FontSize(11));

                // Header
                page.Header().Row(row =>
                {
                    row.RelativeItem().Column(col =>
                    {
                        col.Item().Text("LeisnerCare").FontSize(20).Bold().FontColor(Colors.Blue.Medium);
                        col.Item().Text("Patient Ugeoversigt").FontSize(14).SemiBold();
                    });
                    row.ConstantItem(100).Text(DateTime.Now.ToString("dd-MM-yyyy")).AlignRight();
                });

                // Indhold
                page.Content().PaddingVertical(1, Unit.Centimetre).Column(col =>
                {
                    // Patient info
                    col.Item().Background(Colors.Grey.Lighten3).Padding(10).Column(info =>
                    {
                        info.Item().Text("Patient Information").FontSize(14).Bold();
                        info.Item().Text($"Navn: {patient.User?.FirstName} {patient.User?.LastName}");
                        info.Item().Text($"CPR: {patient.CprNumber}");
                        info.Item().Text($"Fødselsdato: {patient.DateOfBirth:dd-MM-yyyy}");
                    });

                    col.Item().PaddingVertical(10);

                    // Symptomer tabel
                    col.Item().Text("Symptomer (seneste 30)").FontSize(14).Bold();
                    col.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(2); // Dato
                            columns.RelativeColumn(2); // Type
                            columns.RelativeColumn(1); // Værdi
                            columns.RelativeColumn(3); // Note
                        });

                        table.Header(header =>
                        {
                            header.Cell().Text("Dato").Bold();
                            header.Cell().Text("Type").Bold();
                            header.Cell().Text("Værdi").Bold();
                            header.Cell().Text("Note").Bold();
                        });

                        foreach (var s in symptoms)
                        {
                            var valueText = s.Type == SymptomType.OnOff
                                ? (s.Value == 0 ? "ON" : s.Value == 1 ? "OFF" : "DYSKINESI")
                                : s.Value.ToString();

                            table.Cell().Text(s.RecordedAt.ToString("dd-MM HH:mm"));
                            table.Cell().Text(s.Type.ToString());
                            table.Cell().Text(valueText);
                            table.Cell().Text(s.Note ?? "-");
                        }
                    });

                    col.Item().PaddingVertical(10);

                    // Medicin
                    col.Item().Text("Medicin Oversigt").FontSize(14).Bold();
                    foreach (var med in medications)
                    {
                        col.Item().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5).Row(row =>
                        {
                            row.RelativeItem().Column(c =>
                            {
                                c.Item().Text($"{med.Name} - {med.Dosage}").Bold();
                                c.Item().Text($"Frekvens: {med.Frequency}");
                                var todayLog = med.Logs.FirstOrDefault(l => l.TakenAt.Date == DateTime.Today);
                                c.Item().Text($"Taget i dag: {(todayLog != null ? "Ja" : "Nej")}")
                                    .FontColor(todayLog != null ? Colors.Green.Darken2 : Colors.Red.Darken2);
                            });
                        });
                    }

                    col.Item().PaddingVertical(10);

                    // Observationer
                    col.Item().Text("Observationer").FontSize(14).Bold();
                    foreach (var obs in observations)
                    {
                        var badgeColor = obs.IsClinical ? Colors.Red.Medium : Colors.Blue.Medium;
                        col.Item().Background(Colors.Grey.Lighten4).Padding(8).Column(c =>
                        {
                            c.Item().Row(r =>
                            {
                                r.RelativeItem().Text($"{obs.AuthorName} ({obs.AuthorRole})").SemiBold();
                                r.ConstantItem(120).Text(obs.CreatedAt.ToString("dd-MM-yyyy")).AlignRight();
                            });
                            c.Item().Text(obs.Content);
                            if (obs.IsClinical)
                                c.Item().Text("Klinisk note").FontSize(9).FontColor(Colors.Red.Medium).Italic();
                        });
                    }
                });

                // Footer
                page.Footer().AlignCenter().Text(text =>
                {
                    text.Span("Side ").FontSize(9);
                    text.CurrentPageNumber().FontSize(9);
                    text.Span(" af ").FontSize(9);
                    text.TotalPages().FontSize(9);
                });
            });
        });

        return document.GeneratePdf();
    }
}