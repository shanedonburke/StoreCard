using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace StoreCard
{
    internal class HttpUtils
    {
        private static readonly HttpClient _httpClient = new HttpClient();

        private static readonly Regex _titleRegex = new Regex(@"<title>(.+)</title>");

        public static async Task<string> GetWebsiteTitle(string url)
        {
            var title = string.Empty;
            try
            {
                var responseBody = await _httpClient.GetStringAsync(url);
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
    }
}