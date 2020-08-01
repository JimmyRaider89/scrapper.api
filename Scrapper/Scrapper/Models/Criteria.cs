using Scrapper.Helper;
using System.ComponentModel.DataAnnotations;

namespace Scrapper.Models
{
    public class Criteria
    {
        [Required]
        public string KeyWords { get; set; }

        [Required]
        [EnumDataType(typeof(SearchEngine), ErrorMessage = "Search Engine type does not exist")]
        public SearchEngine Engine { get; set; }
    }
}
