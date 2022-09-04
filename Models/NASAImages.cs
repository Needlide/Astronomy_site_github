using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVC_app_main.Models
{
    [Table("NASAImages")]
    public class ImagesGallery
    {
        public int Id { get; set; }
        public string? center { get; set; }
        public string? title { get; set; }
        public string? nasa_id { get; set; }
        public string? media_type { get; set; }
        public List<string>? keywords { get; set; }
        public DateTime? date_created { get; set; }
        public string? description_508 { get; set; }
        public string? secondary_creator { get; set; }
        public string? description { get; set; }
        public string? href { get; set; }
    }
}
