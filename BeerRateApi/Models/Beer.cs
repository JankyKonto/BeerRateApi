namespace BeerRateApi.Models
{
    public class Beer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Producer { get ; set; }
        public string Kind {  get; set; }
        public string OriginCountry { get; set; }
        public decimal AlcoholAmount { get; set; }
        public int Ibu { get; set; }
        public bool isComitted { get; set; }
        public virtual ICollection<Review> Reviews { get; set; }

    }
}
