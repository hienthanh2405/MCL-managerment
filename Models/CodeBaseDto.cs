using API.Infrastructure;
using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class CodeBaseDto : BaseDto
    {
        [Required(ErrorMessage = "You must provide Code")]
        [Sortable]
        [Filterable]
        public string Code { get; set; }
    }
}
