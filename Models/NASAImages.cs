using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVC_app_main.Models
{
    [Table("NASAImages")]
    public class ImagesGallery
    {
        public int Id { get; set; }
        public string? href { get; set; }
        public/* List<Data>*/string? data { get; set; }
        public /*List<Links>*/string? links { get; set; }
    }

    //[Keyless]
    //[NotMapped]
    //public class Data
    //{
    //    public string? description { get; set; }
    //    public string? title { get; set; }
    //    public string? photographer { get; set; }
    //    public string? location { get; set; }
    //    public string? nasa_id { get; set; }
    //    public string? date_created { get; set; }
    //    /*public string[]? keywords { get; set; }*/
    //    public string? media_type { get; set; }
    //    public string? center { get; set; }
    //}

    //[Keyless]
    //[NotMapped]
    //public class Links
    //{
    //    public string? href { get; set; }
    //    public string? rel { get; set; }
    //    public string? render { get; set; }
    //}
}
