namespace TgBotPlay.WebAPI.Utils;

public static class UrlUtils
{
    public static string ObfuscateWebHookUrl(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return url;

        var safeLength = url.LastIndexOf('/') + 1;
        return $"{url[..safeLength]}{new string('*', url.Length - safeLength)}";
    }
}
