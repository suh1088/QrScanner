namespace QRScreenScanner.Models;

public enum ScanResultType
{
    Success,
    NoQRCodeFound,
    MultipleQRCodesFound,
    NotAUrl,
    Error
}

public record ScanResult(
    ScanResultType Type,
    string Message,
    IReadOnlyList<QRCodeResult>? QRCodes = null,
    Exception? Exception = null
);
