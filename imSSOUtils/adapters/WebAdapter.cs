using System;
using System.Drawing;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using imSSOUtils.window.windows;
using Newtonsoft.Json.Linq;

namespace imSSOUtils.adapters
{
    /// <summary>
    /// Super basic Web Adapter.
    /// </summary>
    internal struct WebAdapter
    {
        #region Variables
        /// <summary>
        /// URL to read from.
        /// </summary>
        private static readonly string dice =
            "https://gist.githubusercontent.com/Optimusik/56abf4253ded0fb30f0609a21d29a935/raw/gistfile1.txt";

        /// <summary>
        /// Readonly single instance of <see cref="WebClient"/>.
        /// </summary>
        private static readonly WebClient client = new();

        /// <summary>
        /// Readonly single instance of <see cref="HttpClient"/>.
        /// </summary>
        private static readonly HttpClient http = new();

        /// <summary>
        /// Cached API content.
        /// </summary>
        private static JObject content;
        #endregion

        /// <summary>
        /// Download / Read a URL.
        /// </summary>
        /// <param name="url">The URL</param>
        /// <returns>The URLs content in a string.</returns>
        public static async Task<string> DownloadContent(string url)
        {
            try
            {
                // Hello Zuey! You can't code and you know it.
                return await http.GetStringAsync(new Uri(url));
            }
            catch (HttpRequestException e)
            {
                ConsoleWindow.send_input($"An HttpRequestException occured: {e.Message}", "[web]", Color.OrangeRed);
            }

            return string.Empty;
        }

        /// <summary>
        /// Dispose Web and Http Clients.
        /// </summary>
        public static void dispose_web()
        {
            http.Dispose();
            client.Dispose();
        }

        /// <summary>
        /// Cache the API data.
        /// </summary>
        public static async Task cache_api() => content = JObject.Parse(await DownloadContent(dice));

        /// <summary>
        /// Get a key's value from the API.
        /// </summary>
        /// <param name="key">The key's name.</param>
        /// <returns>The key's value, if present.</returns>
        public static object GetAPIValue(string key) => content?[key];
    }
}