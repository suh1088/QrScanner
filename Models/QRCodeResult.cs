namespace QRScreenScanner.Models;

using System.Drawing;

public record QRCodeResult(
    string Content,
    Rectangle BoundingBox,
    bool IsUrl
);
