using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace spigotmc.org
{
    class Program
    {
        class SpigotPlugin
        {
            public string Name { get; set; }
            public string Currency { get; set; }
            public float Price { get; set; }
            public int DownloadCount { get; set; }
        }

        static async Task Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;


            var cookieContainer = new CookieContainer();
            using var handler = new HttpClientHandler { CookieContainer = cookieContainer };
            using var client = new HttpClient(handler);
            cookieContainer.Add(new Cookie("wmt_secToken", "W731b7f60b5a7de43052038894a7145c1S", "/", "www.spigotmc.org"));
            cookieContainer.Add(new Cookie("xf_session", "855208b953911a981310ee2bcf72eeb7", "/", "www.spigotmc.org"));
            cookieContainer.Add(new Cookie("xf_user", "1066191%2C392ef11138859f16c354cba4d237225375f44060", "/", "www.spigotmc.org"));
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/85.0.4183.83 Safari/537.36");

            var plugins = new List<SpigotPlugin>();
            for (var i = 1; i <= 66; i++)
            {
                var url = "https://www.spigotmc.org/resources/categories/premium.20/?page=" + i;
                Console.WriteLine("GET " + url);

                var response = await client.GetAsync(url);
                var html = await response.Content.ReadAsStringAsync();

                var doc = new HtmlDocument();
                doc.LoadHtml(html);

                var nodes = doc.DocumentNode.SelectNodes("//*[@id=\"content\"]/div/div/div[2]/div/div[2]/div[2]/div/form/ol/li");

                foreach (var node in nodes)
                {
                    var nameNode = node.SelectSingleNode("div[2]/div/h3/a");
                    var priceNode = node.SelectSingleNode("div[2]/div/span");
                    var downloadCount = node.SelectSingleNode("div[3]/div/div[2]/dl[1]/dd");
                    var priceString = priceNode.InnerText.Split(" ");

                    plugins.Add(new SpigotPlugin
                    {
                        Name = nameNode.InnerText,
                        Currency = priceString[1],
                        Price = float.Parse(priceString[0], CultureInfo.GetCultureInfo("en-US")),
                        DownloadCount = int.Parse(downloadCount.InnerText.Replace(",", ""))
                    });
                }

                await Task.Delay(new Random().Next(1000, 2000));
            }

            Console.WriteLine($"Total: {plugins.Sum(d => d.Price * d.DownloadCount)}");
            Console.ReadKey();
        }
    }
}
