using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using QRScreenScanner.Models;

namespace QRScreenScanner;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    public void ShowResult(ScanResult result)
    {
        var (icon, color, title) = result.Type switch
        {
            ScanResultType.Success => ("\u2713", "#4CAF50", "Success!"),
            ScanResultType.NoQRCodeFound => ("\u26A0", "#FF9800", "QR \ucf54\ub4dc\ub97c \ucc3e\uc744 \uc218 \uc5c6\uc74c"),
            ScanResultType.MultipleQRCodesFound => ("\u2713", "#4CAF50", "\uc5ec\ub7ec QR \ucf54\ub4dc \ubc1c\uacac"),
            ScanResultType.NotAUrl => ("\u26A0", "#FF9800", "URL\uc774 \uc544\ub2d8"),
            ScanResultType.Error => ("\u2717", "#F44336", "\uc624\ub958 \ubc1c\uc0dd"),
            _ => ("?", "#9E9E9E", "\uc54c \uc218 \uc5c6\uc74c")
        };

        StatusIcon.Text = icon;
        StatusIcon.Foreground = new SolidColorBrush(
            (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(color));
        StatusTitle.Text = title;
        MessageText.Text = result.Message;

        // Trigger fade-in animation
        var storyboard = (Storyboard)FindResource("FadeInStoryboard");
        BeginStoryboard(storyboard);
    }
}
