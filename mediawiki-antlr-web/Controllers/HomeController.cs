using System;
using System.Diagnostics;
using System.Text;
using Antlr4.Runtime;
using mediawiki_antlr_web;
using mediawiki_antlr_web.Gen;
using Microsoft.AspNetCore.Mvc;
using mediawiki_antlr_web.Models;
using Microsoft.Extensions.Logging;

namespace mediawiki_antlr_web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var defaultMediaWikiText = System.IO.File.ReadAllText(@"example-mediawiki-text.txt");
            var model = new IndexViewModel() {MediaWikiText = defaultMediaWikiText};
            return View(model);
        }

        [HttpPost]
        public IActionResult Index(string mediaWikiText)
        {
            var html = TranslateMediaWiki(mediaWikiText);
            var model = new IndexViewModel() {MediaWikiText = mediaWikiText, Html = html};
            return View(model);
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }

        private static string TranslateMediaWiki(string input)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                var inputStream = new AntlrInputStream(input);
                var mediawikiLexer = new MediawikiLexer(inputStream);
                var commonTokenStream = new CommonTokenStream(mediawikiLexer);
                var mediawikiParser = new MediawikiParser(commonTokenStream);

                MediawikiParser.DocumentContext documentContext = mediawikiParser.document();
                var visitor = new BasicMediawikiVisitor();
                visitor.Visit(documentContext);

                foreach (var line in visitor.Lines)
                {
                    sb.Append(line.Content);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex);
            }

            return sb.ToString();
        }
    }
}