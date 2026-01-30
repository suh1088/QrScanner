namespace QRScreenScanner.Services;

using System.Diagnostics;

public class UrlLauncherService
{
    private static readonly string[] AllowedSchemes = { "http", "https" };

    public bool IsValidUrl(string content)
    {
        if (string.IsNullOrWhiteSpace(content))
            return false;

        return Uri.TryCreate(content, UriKind.Absolute, out var uri)
            && AllowedSchemes.Contains(uri.Scheme.ToLowerInvariant());
    }

    public void OpenInDefaultBrowser(string url)
    {
        if (!IsValidUrl(url))
            throw new ArgumentException("Invalid URL", nameof(url));

        Process.Start(new ProcessStartInfo
        {
            FileName = url,
            UseShellExecute = true
        });
    }
}
