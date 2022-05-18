using System;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Media.Imaging;

namespace StoreCard.Utils;

internal class Links
{
    private static readonly HttpClient s_httpClient = new();

    private static readonly Regex s_urlRegex =
        new(@"(http(s)?:\/\/)?(www\.)?([-a-zA-Z0-9@:%._\+~#=]{2,256}\.[a-z]{2,6})\b([-a-zA-Z0-9@:%_\+.~#?&//=]*)");

    private static readonly Regex s_titleRegex = new(@"<title>(.+)</title>");

    public static async Task<string> GetPageTitle(string url)
    {
        var title = string.Empty;

        var fullUrl = NormalizeUrl(url);
        if (!s_urlRegex.Match(fullUrl).Success) return title;

        try
        {
            var responseBody = await s_httpClient.GetStringAsync(fullUrl);
            var m = s_titleRegex.Match(responseBody);
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

    public static async Task<BitmapImage?> GetPageIcon(string url)
    {
        BitmapImage? image = null;

        var m = s_urlRegex.Match(url);
        if (!m.Success) return image;

        var domain = m.Groups[4].Captures[0].ToString();
        try
        {
            var bytes =
                await s_httpClient.GetByteArrayAsync(@"https://icons.duckduckgo.com/ip3/" + domain + ".ico");
            image = Images.BytesToBitmapImage(bytes);
            image.Freeze();
        }
        catch
        {
            // Ignored
        }

        return image;
    }

    public static string NormalizeUrl(string url)
    {
        var includesScheme = url.StartsWith("http://", StringComparison.OrdinalIgnoreCase) || 
                             url.StartsWith("https://", StringComparison.OrdinalIgnoreCase);
        return includesScheme ? url : $"http://{url}";
    }
}