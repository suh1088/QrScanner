namespace QRScreenScanner.Services;

using System.Drawing;
using QRScreenScanner.Models;
using ZXing;
using ZXing.Common;
using ZXing.Windows.Compatibility;

public class QRCodeDetectionService
{
    private readonly BarcodeReader _reader;

    public QRCodeDetectionService()
    {
        _reader = new BarcodeReader
        {
            AutoRotate = true,
            Options = new DecodingOptions
            {
                TryHarder = true,
                PossibleFormats = new List<BarcodeFormat> { BarcodeFormat.QR_CODE },
                TryInverted = true
            }
        };
    }

    public IReadOnlyList<QRCodeResult> DetectQRCodes(Bitmap image)
    {
        var results = _reader.DecodeMultiple(image);

        if (results == null || results.Length == 0)
        {
            return Array.Empty<QRCodeResult>();
        }

        return results
            .Where(r => r != null && !string.IsNullOrWhiteSpace(r.Text))
            .Select(r => new QRCodeResult(
                Content: r.Text,
                BoundingBox: GetBoundingBox(r.ResultPoints),
                IsUrl: IsUrl(r.Text)
            ))
            .ToList();
    }

    private static Rectangle GetBoundingBox(ResultPoint[]? points)
    {
        if (points == null || points.Length == 0)
            return Rectangle.Empty;

        var minX = (int)points.Min(p => p.X);
        var minY = (int)points.Min(p => p.Y);
        var maxX = (int)points.Max(p => p.X);
        var maxY = (int)points.Max(p => p.Y);

        return new Rectangle(minX, minY, maxX - minX, maxY - minY);
    }

    private static bool IsUrl(string text)
    {
        return Uri.TryCreate(text, UriKind.Absolute, out var uri)
            && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
    }
}
