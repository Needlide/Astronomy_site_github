using System.ComponentModel.DataAnnotations.Schema;

namespace MVC_app_main.Models
{
    [Table("NASAImages")]
    public class ImagesGallery
    {
        public int Id { get; set; }
        public string? Center { get; set; }
        public string? Title { get; set; }
        public string? NasaId { get; set; }
        public string? MediaType { get; set; }
        public List<string>? Keywords { get; set; }
        public DateTime? DateCreated { get; set; }
        public string? Description508 { get; set; }
        public string? SecondaryCreator { get; set; }
        public string? Description { get; set; }
        public string? Href { get; set; }
    }
}
