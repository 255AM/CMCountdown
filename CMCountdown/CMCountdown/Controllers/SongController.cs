using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using HtmlAgilityPack;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Net;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using CMCountdown.Models;

namespace ScrapingBeeScraper.Controllers
{
    public class SongController : Controller
    {
        private readonly ILogger<SongController> _logger;

        public SongController(ILogger<SongController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            string songsOfThatDateUrl = "https://american-country-countdown.fandom.com/wiki/September_12,_2020";
            var response = CallUrl(songsOfThatDateUrl).Result;
            var songList = ParseHtml(response);
            WriteToCsv(songList);

            return View();
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private static async Task<string> CallUrl(string fullUrl)
        {
            HttpClient client = new HttpClient();
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls13;
            client.DefaultRequestHeaders.Accept.Clear();
            var response = client.GetStringAsync(fullUrl);
            return await response;
        }

        private List<string> ParseHtml(string html)
        {
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);
            var songs = htmlDoc.DocumentNode
                            .SelectNodes("//b/a")
                    .Where(node => node.GetAttributeValue("title", "").Contains(":")).ToList();
            List<string> Songs = new List<string>();

            foreach (var song in songs)
            {
                Songs.Add(song.Attributes[1].Value);
                    

            }



            return Songs;

        }

        private void WriteToCsv(List<string> songs)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var song in songs)
            {
                sb.AppendLine(song);
            }

            System.IO.File.WriteAllText("songs.csv", sb.ToString());
        }
    }
}