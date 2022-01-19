namespace MVC_app_main.Models
{
    public class Photos
    {
        public string Id { get; set; }
        public string Sol { get; set; }
        public IDictionary<string, string> Camera { get; set; }
        public string Img_src { get; set; }
        public string Earth_date { get; set; }
        public IDictionary<string, string> Rover { get; set; }
    }
}
