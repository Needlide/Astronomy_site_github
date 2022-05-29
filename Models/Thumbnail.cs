using System.ComponentModel.DataAnnotations.Schema;

namespace MVC_app_main.Models
{
    [Table("[dbo].thumbnails")]
    public class Thumbnail
    {
        public int Id { get; set; }
        public string Title {  get; set; }
        public string Url {  get; set; }
        public string ImageUrl {  get; set; }
        public string NewsSite {  get; set; }
        public string Summary {  get; set; }
        public DateTime PublishedAt {  get; set; }
        public DateTime UpdatedAt {  get; set; }
    }
}
