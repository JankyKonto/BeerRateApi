using Microsoft.AspNetCore.Routing.Constraints;

namespace BeerRateApi.Models
{
    /// <summary>
    /// Represents an image associated with a beer, including its data and metadata.
    /// </summary>
    public class BeerImage
    {
        public int Id { get; set; }
        public byte[] Data { get; set; }
        public string FileType { get; set; }
        public string Caption { get; set; }
    }
}
