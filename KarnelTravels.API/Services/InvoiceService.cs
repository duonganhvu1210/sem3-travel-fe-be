using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using KarnelTravels.API.DTOs;

namespace KarnelTravels.API.Services;

public interface IInvoiceService
{
    byte[] GenerateInvoicePdf(InvoiceDto invoice);
}

public class InvoiceService : IInvoiceService
{
    public InvoiceService()
    {
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public byte[] GenerateInvoicePdf(InvoiceDto invoice)
    {
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(40);
                page.DefaultTextStyle(x => x.FontSize(11).FontFamily("Arial"));

                page.Header().Element(c => ComposeHeader(c, invoice));
                page.Content().Element(c => ComposeContent(c, invoice));
                page.Footer().Element(c => ComposeFooter(c, invoice));
            });
        });

        return document.GeneratePdf();
    }

    private void ComposeHeader(IContainer container, InvoiceDto invoice)
    {
        container.Column(column =>
        {
            column.Item().Row(row =>
            {
                row.RelativeItem().Column(col =>
                {
                    col.Item().Text("KARNEL TRAVELS")
                        .Bold().FontSize(24).FontColor(Colors.Blue.Darken2);
                    col.Item().Text(invoice.CompanyAddress).FontSize(10);
                    col.Item().Text($"DT: {invoice.CompanyPhone} | Email: {invoice.CompanyEmail}").FontSize(10);
                });

                row.RelativeItem().AlignRight().Column(col =>
                {
                    col.Item().Text("HOA DON THANH TOAN")
                        .Bold().FontSize(18).FontColor(Colors.Blue.Darken2);
                    col.Item().Text($"So: {invoice.InvoiceNumber}").FontSize(11);
                    col.Item().Text($"Ngay: {invoice.InvoiceDate:dd/MM/yyyy HH:mm}").FontSize(11);
                });
            });

            column.Item().PaddingVertical(10).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
        });
    }

    private void ComposeContent(IContainer container, InvoiceDto invoice)
    {
        container.PaddingVertical(10).Column(column =>
        {
            // Customer Info
            column.Item().Row(row =>
            {
                row.RelativeItem().Column(col =>
                {
                    col.Item().Text("THONG TIN KHACH HANG").Bold().FontColor(Colors.Blue.Darken1);
                    col.Item().PaddingTop(5).Text($"Ho ten: {invoice.CustomerName}");
                    col.Item().Text($"Email: {invoice.CustomerEmail}");
                    col.Item().Text($"Dien thoai: {invoice.CustomerPhone}");
                });

                row.RelativeItem().Column(col =>
                {
                    col.Item().Text("THONG TIN DON HANG").Bold().FontColor(Colors.Blue.Darken1);
                    col.Item().PaddingTop(5).Text($"Ma don: {invoice.OrderCode}");
                    col.Item().Text($"Ngay dat: {invoice.BookingDate:dd/MM/yyyy HH:mm}");
                    col.Item().Text($"Ngay su dung: {invoice.ServiceDate:dd/MM/yyyy}");
                    if (invoice.EndDate.HasValue)
                        col.Item().Text($"Ngay ket thuc: {invoice.EndDate.Value:dd/MM/yyyy}");
                });
            });

            column.Item().PaddingVertical(15).Text("CHI TIET DICH VU").Bold().FontColor(Colors.Blue.Darken1);

            // Service Table
            column.Item().Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(3);
                    columns.RelativeColumn(2);
                    columns.RelativeColumn(3);
                    columns.RelativeColumn(1);
                    columns.RelativeColumn(2);
                });

                // Header
                table.Header(header =>
                {
                    header.Cell().Background(Colors.Blue.Darken1).Padding(8)
                        .Text("Ten dich vu").FontColor(Colors.White).Bold();
                    header.Cell().Background(Colors.Blue.Darken1).Padding(8)
                        .Text("Loai").FontColor(Colors.White).Bold();
                    header.Cell().Background(Colors.Blue.Darken1).Padding(8)
                        .Text("Thong tin").FontColor(Colors.White).Bold();
                    header.Cell().Background(Colors.Blue.Darken1).Padding(8)
                        .Text("SL").FontColor(Colors.White).Bold();
                    header.Cell().Background(Colors.Blue.Darken1).Padding(8).AlignRight()
                        .Text("Thanh tien").FontColor(Colors.White).Bold();
                });

                // Data
                table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(8)
                    .Text(invoice.ServiceName);
                table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(8)
                    .Text(invoice.ServiceType);
                table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(8)
                    .Text(invoice.ServiceDetails ?? invoice.ServiceAddress ?? "-");
                table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(8)
                    .Text(invoice.Quantity.ToString());
                table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(8).AlignRight()
                    .Text($"{invoice.TotalAmount:N0} VND");
            });

            // Pricing Summary - use Column instead of Row
            column.Item().PaddingTop(15).AlignRight().Column(priceCol =>
            {
                priceCol.Item().Row(r =>
                {
                    r.RelativeItem().Text("Tong tien:");
                    r.AutoItem().Text($"{invoice.TotalAmount:N0} VND");
                });
                
                if (invoice.DiscountAmount > 0)
                {
                    priceCol.Item().Row(r =>
                    {
                        r.RelativeItem().Text("Giam gia:");
                        r.AutoItem().Text($"-{invoice.DiscountAmount:N0} VND").FontColor(Colors.Green.Darken1);
                    });
                    
                    if (!string.IsNullOrEmpty(invoice.PromotionCode))
                    {
                        priceCol.Item().Text($"(Ma: {invoice.PromotionCode})").FontSize(9).FontColor(Colors.Grey.Medium);
                    }
                }
                
                priceCol.Item().PaddingTop(5).LineHorizontal(1).LineColor(Colors.Black);
                priceCol.Item().Row(r =>
                {
                    r.RelativeItem().Text("THANH TOAN:").Bold();
                    r.AutoItem().Text($"{invoice.FinalAmount:N0} VND").Bold().FontSize(14).FontColor(Colors.Blue.Darken2);
                });
            });

            // Payment Status
            column.Item().PaddingTop(20).Row(row =>
            {
                row.RelativeItem().Column(col =>
                {
                    col.Item().Text("THONG TIN THANH TOAN").Bold().FontColor(Colors.Blue.Darken1);
                    col.Item().PaddingTop(5).Text($"Trang thai: {invoice.PaymentStatus}");
                    col.Item().Text($"Phuong thuc: {invoice.PaymentMethod ?? "Chua thanh toan"}");
                    if (invoice.PaidAt.HasValue)
                        col.Item().Text($"Ngay thanh toan: {invoice.PaidAt.Value:dd/MM/yyyy HH:mm}");
                });
            });
        });
    }

    private void ComposeFooter(IContainer container, InvoiceDto invoice)
    {
        container.Column(column =>
        {
            column.Item().PaddingTop(20).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
            
            column.Item().PaddingTop(10).AlignCenter().Column(col =>
            {
                col.Item().Text(invoice.TermsAndConditions).FontSize(9).FontColor(Colors.Grey.Medium);
                col.Item().PaddingTop(5).Text(invoice.ThankYouMessage).FontSize(11).Bold().FontColor(Colors.Blue.Darken1);
            });
        });
    }
}
