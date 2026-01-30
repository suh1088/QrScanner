namespace QRScreenScanner.Services;

using System.Drawing;
using System.Windows.Forms;

public class ScreenCaptureService
{
    public Bitmap CaptureAllScreens()
    {
        var bounds = GetVirtualScreenBounds();

        var bitmap = new Bitmap(bounds.Width, bounds.Height);
        using var graphics = Graphics.FromImage(bitmap);

        graphics.CopyFromScreen(
            bounds.Left,
            bounds.Top,
            0,
            0,
            bounds.Size,
            CopyPixelOperation.SourceCopy
        );

        return bitmap;
    }

    public Bitmap CapturePrimaryScreen()
    {
        var screen = Screen.PrimaryScreen
            ?? throw new InvalidOperationException("No primary screen found");
        var bounds = screen.Bounds;

        var bitmap = new Bitmap(bounds.Width, bounds.Height);
        using var graphics = Graphics.FromImage(bitmap);

        graphics.CopyFromScreen(
            bounds.Left,
            bounds.Top,
            0,
            0,
            bounds.Size,
            CopyPixelOperation.SourceCopy
        );

        return bitmap;
    }

    private static Rectangle GetVirtualScreenBounds()
    {
        return new Rectangle(
            SystemInformation.VirtualScreen.Left,
            SystemInformation.VirtualScreen.Top,
            SystemInformation.VirtualScreen.Width,
            SystemInformation.VirtualScreen.Height
        );
    }
}
