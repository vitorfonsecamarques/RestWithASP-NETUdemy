using RestWithAspNetUdemy.Hypermedia;
using RestWithAspNetUdemy.Hypermedia.Abstract;

namespace RestWithAspNetUdemy.Data.VO
{
    public class PersonVO : ISupportHyperMedia
    {
        //[JsonPropertyName("code")]
        public long Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Adress { get; set; }
        public string Gender { get; set; }

        public List<HyperMediaLink> Links { get; set; } = new List<HyperMediaLink>();
    }
}
