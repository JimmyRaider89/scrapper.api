using Scrapper.Helper;
using System.ComponentModel.DataAnnotations;

namespace Scrapper.Models
{
    public class Criteria
    {
        [Required(ErrorMessage ="Search criteria is required.")]
        [MaxLength(200)]
        public string KeyWords { get; set; }

        [Required]
        [EnumDataType(typeof(SearchEngine), ErrorMessage = "Search Engine does not exist")]
        public SearchEngine Engine { get; set; }
    }
}
