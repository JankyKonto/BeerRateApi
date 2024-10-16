using System.ComponentModel.DataAnnotations;

namespace BeerRateApi.Models
{
    public class Review
    {
        public int Id { get; set; }
        public string? Text { get; set; }

        [Range(1, 10, ErrorMessage = "This rate must be value between 1 and 10")]
        public int TasteRate { get; set; }

        [Range(1, 10, ErrorMessage = "This rate must be value between 1 and 10")]
        public int AromaRate { get; set; }

        [Range(1, 10, ErrorMessage = "This rate must be value between 1 and 10")]
        public int FoamRate { get; set; }

        [Range(1, 10, ErrorMessage = "This rate must be value between 1 and 10")]
        public int ColourRate { get; set; }

        //navigation properties
        public virtual Beer Beer { get; set; }
        public int BeerId {  get; set; }

        public virtual User User { get; set; }
        public int UserId { get; set; }
    }
}
