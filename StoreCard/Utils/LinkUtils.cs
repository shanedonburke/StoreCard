#region

using System;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Media.Imaging;

#endregion

namespace StoreCard.Utils;

/// <summary>
/// Utilities for working with links.
/// </summary>
public static class LinkUtils
{
    private static readonly HttpClient s_httpClient = new();

    /// <summary>
    /// Expression that matches any valid URL.
    /// </summary>
    private static readonly Regex s_urlRegex =
        new(@"(http(s)?:\/\/)?(www\.)?([-a-zA-Z0-9@:%._\+~#=]{2,256}\.[a-z]{2,6})\b([-a-zA-Z0-9@:%_\+.~#?&//=]*)");

    /// <summary>
    /// Expression that matches the <c>title</c> HTML tag, capturing its value.
    /// </summary>
    private static readonly Regex s_titleRegex = new(@"<title>(.+)</title>");

    /// <summary>
    /// Get the title of a page, as it would be displayed in a browser.
    /// </summary>
    /// <param name="url"></param>
    /// <returns>Page title</returns>
    public static async Task<string> GetPageTitle(string url)
    {
        string fullUrl = NormalizeUrl(url);
        string title = string.Empty;

        if (!s_urlRegex.Match(fullUrl).Success)
        {
            return title;
        }

        try
        {
            // Get the HTML response, and grab the <title> tag's value.
            string responseBody = await s_httpClient.GetStringAsync(fullUrl);
            Match m = s_titleRegex.Match(responseBody);

            if (m.Success)
            {
                title = m.Groups[1].Captures[0].ToString();
            }
        }
        catch
        {
            return title;
        }

        return HttpUtility.HtmlDecode(title);
    }

    /// <summary>
    /// Get the favicon for the given URL, as it would be displayed in a browser.
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    public static async Task<BitmapImage?> GetPageIcon(string url)
    {
        BitmapImage? image = null;
        Match m = s_urlRegex.Match(url);

        if (!m.Success)
        {
            // Not a valid URL
            return image;
        }

        string domain = m.Groups[4].Captures[0].ToString();

        try
        {
            // DuckDuckGo provides a service for getting site favicons by domain
            byte[] bytes = await s_httpClient.GetByteArrayAsync($"https://icons.duckduckgo.com/ip3/{domain}.ico");
            image = ImageUtils.BytesToBitmapImage(bytes);
            // Must freeeze to access in other threads
            image.Freeze();
        }
        catch
        {
            // Ignored
        }

        return image;
    }

    /// <summary>
    /// Normalize the given URL by prepending it with <c>http://</c> if it is absent.
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    public static string NormalizeUrl(string url)
    {
        bool includesScheme = url.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
                              url.StartsWith("https://", StringComparison.OrdinalIgnoreCase);
        return includesScheme ? url : $"http://{url}";
    }
}
