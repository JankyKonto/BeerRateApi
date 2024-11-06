using BeerRateApi.Models;
using System.ComponentModel.DataAnnotations;

namespace BeerRateApi.DTOs
{
    public class AddBeerReviewDTO
    {
        public string? Text { get; set; }
        public int? TasteRate { get; set; }
        public int? AromaRate { get; set; }
        public int? FoamRate { get; set; }
        public int? ColorRate { get; set; }
        public int BeerId { get; set; }
        public int UserId { get; set; }
    }
}
