namespace MVC_app_main.Models
{
    //public class ItemsSon
    //{
    //    private ICollection<ItemsSon> JSON { get; set; }
    //}

    public class Main
    {
        public Data collection {get;set;}
    }

    public class Data
    {
        public string version { get; set; }
        public string href { get; set; }
        public ICollection<Items> items { get; set; }
    }

    public class Items
    {
        public ItemsSon item { get; set; }
    }

    public class ItemsSon
    {
        public string href { get; set; }
        public ICollection<object>? data { get; set; }
        public ICollection<object>? links { get; set; }
    }
    public class Prikol
    {
        public SecondPrikol item { get; set; }
    }

    public class Links
    {
        public LinksPrikol item { get; set; }
    }

    public class SecondPrikol
    {
        public string description { get; set; }
        public string title { get; set; }
        public string photographer { get; set; }
        public string location { get; set; }
        public string nasa_id { get; set; }
        public string date_created { get; set; }
        public string[] keywords { get; set; }
        public string media_type { get; set; }
        public string center { get; set; }
    }

    public class LinksPrikol
    {
        public string href { get; set; }
        public string rel { get; set; }
        public string render { get; set; }
    }
}
