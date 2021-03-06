
using System.ComponentModel.DataAnnotations.Schema;

namespace MVC_app_main.Models
{
    [Table("[dbo].NASAImages")]
    public class ImagesGallery
    {
        public string? href { get; set; }
        public /*Data*/List<Data>? data { get; set; }
        public /*Links*/List<Links>? links { get; set; }
    }

    public class Data
    {
        public string? description { get; set; }
        public string? title { get; set; }
        public string? photographer { get; set; }
        public string? location { get; set; }
        public string? nasa_id { get; set; }
        public string? date_created { get; set; }
        /*public string[]? keywords { get; set; }*/
        public string? media_type { get; set; }
        public string? center { get; set; }
    }

    public class Links
    {
        public string? href { get; set; }
        public string? rel { get; set; }
        public string? render { get; set; }
    }
}
