using Microsoft.AspNetCore.Routing.Constraints;

namespace BeerRateApi.Models
{
    public class BeerImage
    {
        public int Id { get; set; }
        public byte[] Data { get; set; }
        public string FileType { get; set; }
        public string Caption { get; set; }
    }
}
