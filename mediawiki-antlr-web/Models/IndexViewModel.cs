using System.ComponentModel.DataAnnotations;

namespace mediawiki_antlr_web.Models
{
    public class IndexViewModel
    {
        [Display(Name = "Tekst Mediawiki")]
        public string? MediaWikiText { get; set; }
        
        [Display(Name = "HTML")]
        public string? Html { get; set; }
    }
}