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
        public bool IsConfirmed { get; set; }
        public virtual ICollection<Review> Reviews { get; set; }
        public int BeerImageId { get; set; }
        public virtual BeerImage? BeerImage {  get; set; }
    }
}
