namespace MVC_app_main.Models
{
    public class Photos
    {
        private string Id { get; set; }
        private string Sol { get; set; }
        private IDictionary<string, string> Camera { get; set; }
        private string Img_src { get; set; }
        private string Earth_date { get; set; }
        private IDictionary<string, string> Rover { get; set; }
    }
}