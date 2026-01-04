using Coworking.Core.Entities;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Coworking.Web.Services;

public class PdfService
{
    public byte[] GenerateFacturePdf(Facture facture)
    {
        QuestPDF.Settings.License = LicenseType.Community;

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);

                page.Header()
                    .Column(column =>
                    {
                        column.Item().Text("FACTURE").FontSize(24).Bold().FontColor(Colors.Blue.Medium);
                        column.Item().Text($"N° {facture.Numero}").FontSize(12);
                    });

                page.Content()
                    .PaddingVertical(1, Unit.Centimetre)
                    .Column(column =>
                    {
                        column.Spacing(1, Unit.Centimetre);

                        // Informations de la facture
                        column.Item().Row(row =>
                        {
                            row.RelativeItem().Column(col =>
                            {
                                col.Item().Text("Date d'émission :").FontSize(10).FontColor(Colors.Grey.Medium);
                                col.Item().Text(facture.DateEmission.ToString("dd/MM/yyyy")).FontSize(12).Bold();
                            });

                            row.RelativeItem().Column(col =>
                            {
                                col.Item().Text("Statut :").FontSize(10).FontColor(Colors.Grey.Medium);
                                col.Item().Text(facture.EstPayee ? "Payée" : "En attente").FontSize(12).Bold();
                            });
                        });

                        column.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);

                        // Informations du client
                        column.Item().Text("CLIENT").FontSize(14).Bold();
                        column.Item().Column(col =>
                        {
                            col.Item().Text($"Nom : {facture.Reservation.ClientNom}");
                            col.Item().Text($"Email : {facture.Reservation.ClientEmail}");
                            if (!string.IsNullOrEmpty(facture.Reservation.ClientTelephone))
                            {
                                col.Item().Text($"Téléphone : {facture.Reservation.ClientTelephone}");
                            }
                        });

                        column.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);

                        // Détails de la réservation
                        column.Item().Text("DÉTAILS DE LA RÉSERVATION").FontSize(14).Bold();
                        column.Item().Column(col =>
                        {
                            col.Item().Text($"Salle : {facture.Reservation.Salle.Nom}");
                            col.Item().Text($"Capacité : {facture.Reservation.Salle.Capacite} personnes");
                            col.Item().Text($"Date de début : {facture.Reservation.DateDebut:dd/MM/yyyy à HH:mm}");
                            col.Item().Text($"Date de fin : {facture.Reservation.DateFin:dd/MM/yyyy à HH:mm}");
                            var duree = (facture.Reservation.DateFin - facture.Reservation.DateDebut).TotalHours;
                            col.Item().Text($"Durée : {duree:F1} heures");
                        });

                        column.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);

                        // Montant
                        column.Item().AlignRight().Column(col =>
                        {
                            col.Item().Text("MONTANT TOTAL").FontSize(16).Bold();
                            col.Item().Text($"{facture.MontantTotal:F2} MAD").FontSize(20).Bold().FontColor(Colors.Blue.Medium);
                        });
                    });

                page.Footer()
                    .AlignCenter()
                    .Text(text =>
                    {
                        text.Span("Plateforme de Réservation de Salles - ");
                        text.Span(DateTime.Now.Year.ToString());
                    })
                    .FontSize(8)
                    .FontColor(Colors.Grey.Medium);
            });
        });

        return document.GeneratePdf();
    }
}

