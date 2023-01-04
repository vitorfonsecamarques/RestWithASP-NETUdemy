using RestWithAspNetUdemy.Hypermedia;
using RestWithAspNetUdemy.Hypermedia.Abstract;

namespace RestWithAspNetUdemy.Data.VO
{
    public class BookVO : ISupportHyperMedia
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public decimal Price { get; set; }
        public DateTime LaunchDate { get; set; }

        public List<HyperMediaLink> Links { get; set; } = new List<HyperMediaLink>();
    }
}
