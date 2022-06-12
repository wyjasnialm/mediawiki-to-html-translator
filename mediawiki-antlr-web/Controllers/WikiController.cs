using Microsoft.AspNetCore.Mvc;

namespace mediawiki_antlr_web.Controllers
{
    [Route("/wiki")]
    public class WikiController : Controller
    {
        [Route("Main_page")]
        public IActionResult MainPage()
        {
            return View();
        }
        
        [Route("Contact_page")]
        public IActionResult ContactPage()
        {
            return View();
        }
    }
}