
namespace BeerRateApi.DTOs
{
    /// <summary>
    /// Encapsulates the details of a single beer review, such as user comments, rating, and review date.
    /// </summary>
    public class GetBeerReviewResult
    {
        public int Id { get; set; }
        public string? Text { get; set; }
        public int? TasteRate { get; set; }
        public int? AromaRate { get; set; }
        public int? FoamRate { get; set; }
        public int? ColorRate { get; set; }
        public int BeerId { get; set; }
        public int UserId { get; set; }
        public string? UserName { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

