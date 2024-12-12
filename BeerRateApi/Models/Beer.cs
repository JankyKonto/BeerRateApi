using System.ComponentModel.DataAnnotations.Schema;

namespace BeerRateApi.Models
{
    public class Beer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Producer { get ; set; }
        public int Kind {  get; set; }
        public string OriginCountry { get; set; }
        public decimal AlcoholAmount { get; set; }
        public int Ibu { get; set; }
        public bool IsConfirmed { get; set; }
        public virtual ICollection<Review> Reviews { get; set; }
        public int BeerImageId { get; set; }
        public virtual BeerImage BeerImage {  get; set; }
        public bool IsRemoved { get; set; }

        [NotMapped]
        public float AverageTasteRate
        {
            get
            {
                var collection = Reviews.Where(r => r.TasteRate.HasValue).Select(r => r.TasteRate.Value);
                if (collection.Any())
                    return (float)collection.Average();
                else return 0;
            }
        }

        [NotMapped]
        public float AverageAromaRate
        {
            get
            {
                var collection = Reviews.Where(r => r.TasteRate.HasValue).Select(r => r.TasteRate.Value);
                if (collection.Any())
                    return (float)collection.Average();
                else 
                    return 0;
            }
        }

        [NotMapped]
        public float AverageFoamRate
        {
            get
            {
                var collection = Reviews.Where(r => r.FoamRate.HasValue).Select(r => r.FoamRate.Value);
                if (collection.Any())
                    return (float)collection.Average();
                else 
                    return 0;
            }
        }

        [NotMapped]
        public float AverageColorRate
        {
            get
            {
                var collection = Reviews.Where(r => r.ColorRate.HasValue).Select(r => r.ColorRate.Value);
                if (collection.Any())
                    return (float)collection.Average();
                else 
                    return 0;
            }
        }
    }
}
