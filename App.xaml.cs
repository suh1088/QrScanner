using System.Windows;
using QRScreenScanner.Models;
using QRScreenScanner.Services;
using Application = System.Windows.Application;

namespace QRScreenScanner;

public partial class App : Application
{
    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var screenCapture = new ScreenCaptureService();
        var qrDetection = new QRCodeDetectionService();
        var urlLauncher = new UrlLauncherService();

        var mainWindow = new MainWindow();

        // Hide window initially to capture screen without it
        mainWindow.Opacity = 0;
        mainWindow.Show();

        // Small delay to ensure window is rendered but invisible
        await Task.Delay(150);

        // Execute scan
        var result = await ExecuteScanAsync(screenCapture, qrDetection, urlLauncher);

        // Show result
        mainWindow.Opacity = 1;
        mainWindow.ShowResult(result);

        // Auto-close after delay
        var delay = result.Type == ScanResultType.Success ? 1500 : 3000;
        await Task.Delay(delay);

        Shutdown();
    }

    private Task<ScanResult> ExecuteScanAsync(
        ScreenCaptureService screenCapture,
        QRCodeDetectionService qrDetection,
        UrlLauncherService urlLauncher)
    {
        return Task.Run(() =>
        {
            try
            {
                using var screenshot = screenCapture.CaptureAllScreens();
                var qrCodes = qrDetection.DetectQRCodes(screenshot);

                return qrCodes.Count switch
                {
                    0 => new ScanResult(
                        ScanResultType.NoQRCodeFound,
                        "화면에서 QR 코드를 찾을 수 없습니다."),

                    1 => ProcessSingleQRCode(qrCodes[0], urlLauncher),

                    _ => HandleMultipleQRCodes(qrCodes, urlLauncher)
                };
            }
            catch (Exception ex)
            {
                return new ScanResult(
                    ScanResultType.Error,
                    $"스캔 중 오류 발생: {ex.Message}",
                    Exception: ex);
            }
        });
    }

    private ScanResult ProcessSingleQRCode(QRCodeResult qrCode, UrlLauncherService urlLauncher)
    {
        if (qrCode.IsUrl && urlLauncher.IsValidUrl(qrCode.Content))
        {
            urlLauncher.OpenInDefaultBrowser(qrCode.Content);
            return new ScanResult(
                ScanResultType.Success,
                $"\ube0c\ub77c\uc6b0\uc800\uc5d0\uc11c \uc5f4\uae30: {qrCode.Content}",
                new[] { qrCode });
        }

        return new ScanResult(
            ScanResultType.NotAUrl,
            $"QR \ucf54\ub4dc \ubc1c\uacac (URL \uc544\ub2d8):\n{qrCode.Content}",
            new[] { qrCode });
    }

    private ScanResult HandleMultipleQRCodes(IReadOnlyList<QRCodeResult> qrCodes, UrlLauncherService urlLauncher)
    {
        var urlQRCode = qrCodes.FirstOrDefault(qr =>
            qr.IsUrl && urlLauncher.IsValidUrl(qr.Content));

        if (urlQRCode != null)
        {
            urlLauncher.OpenInDefaultBrowser(urlQRCode.Content);
            return new ScanResult(
                ScanResultType.MultipleQRCodesFound,
                $"{qrCodes.Count}\uac1c QR \ucf54\ub4dc \ubc1c\uacac. \uc5f4\uae30: {urlQRCode.Content}",
                qrCodes);
        }

        return new ScanResult(
            ScanResultType.NotAUrl,
            $"{qrCodes.Count}\uac1c QR \ucf54\ub4dc \ubc1c\uacac, URL \uc5c6\uc74c",
            qrCodes);
    }
}
