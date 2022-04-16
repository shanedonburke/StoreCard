using System;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace StoreCard
{
    internal class HttpUtils
    {
        private static readonly HttpClient _httpClient = new();

        private static readonly Regex _urlRegex =
            new(@"(http(s)?:\/\/)?(www\.)?([-a-zA-Z0-9@:%._\+~#=]{2,256}\.[a-z]{2,6})\b([-a-zA-Z0-9@:%_\+.~#?&//=]*)");

        private static readonly Regex _titleRegex = new(@"<title>(.+)</title>");

        public static async Task<string> GetWebsiteTitle(string url)
        {
            var includesScheme = url.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
                                 url.StartsWith("https://", StringComparison.OrdinalIgnoreCase);
            var fullUrl = includesScheme ? url : $"http://{url}";
            var title = string.Empty;
            if (!_urlRegex.Match(fullUrl).Success) return title;
            try
            {
                var responseBody = await _httpClient.GetStringAsync(fullUrl);
                var m = _titleRegex.Match(responseBody);
                if (m.Success)
                {
                    title = m.Groups[1].Captures[0].ToString();
                }
            }
            catch
            {
                // Ignored
            }
            return title;
        }

        public static async Task<BitmapImage?> GetWebsiteIcon(string url)
        {
            BitmapImage? image = null;
            var m = _urlRegex.Match(url);
            if (!m.Success) return image;
            var domain = m.Groups[4].Captures[0].ToString();
            try
            {
                var bytes =
                    await _httpClient.GetByteArrayAsync(@"https://icons.duckduckgo.com/ip3/" + domain + ".ico");
                image = ImageUtils.BytesToBitmapImage(bytes);
                image.Freeze();
            }
            catch
            {
                // Ignored
            }
            return image;
        }
    }
}