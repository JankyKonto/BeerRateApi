using BeerRateApi.Models;
using System.Text.Json;
using Microsoft.AspNetCore.StaticFiles;
using Castle.Components.DictionaryAdapter.Xml;
using Microsoft.EntityFrameworkCore;
namespace BeerRateApi
{
    public static class DbInitializer
    {
        public static async Task SeedAsync(AppDbContext context)
        {
            if (context.Database.EnsureCreated())
            {
                //Variables with test data paths declaration
                string currentPath = Directory.GetCurrentDirectory();
                string beersPath = $"{currentPath}\\TestData\\Beers.json";
                string beerImagePath = $"{currentPath}\\TestData\\BeerImage.png";
                string usersPath = $"{currentPath}\\TestData\\Users.json";
                string reviewsPath = $"{currentPath}\\TestData\\Reviews.json";

                //Json deserialization
                string beersJson = await File.ReadAllTextAsync(beersPath);
                string usersJson = await File.ReadAllTextAsync(usersPath);

                //Collections of objects which will be added to database
                IEnumerable<Beer> beers = JsonSerializer.Deserialize<IEnumerable<Beer>>(beersJson);
                IEnumerable<UserInitializer> usersData = JsonSerializer.Deserialize<IEnumerable<UserInitializer>>(usersJson);
                IEnumerable<Review> reviews = await GenerateReviewsAsync(3000, reviewsPath);
                IEnumerable<User> users;
                
                ICollection<BeerImage> beerImages = new List<BeerImage>();

                users = usersData.Select(
                    userData => new User()
                    {
                        Username = userData.Username,
                        Email = userData.Email,
                        UserType = userData.UserType,
                        PasswordHash = BCrypt.Net.BCrypt.EnhancedHashPassword(userData.Password)
                    });

                //Image collection fulfiling
                byte[] imageFile = await File.ReadAllBytesAsync(beerImagePath);
                string fileType;
                new FileExtensionContentTypeProvider().TryGetContentType(beerImagePath,out fileType);
                foreach (var beer in beers)
                {
                    BeerImage beerImage = new BeerImage()
                    {
                        Caption = "BeerImage",
                        FileType = fileType,
                        Data = imageFile
                    };
                    beerImages.Add(beerImage);
                }

                //Adding objects to database

                context.BeerImages.AddRangeAsync(beerImages);
                context.Beers.AddRangeAsync(beers);
                context.Users.AddRangeAsync(users);
                context.SaveChanges();
                context.Reviews.AddRangeAsync(reviews);
                context.SaveChanges();
                context.Database.CloseConnection();
            }
            
        }
        public static async Task<List<Review>> GenerateReviewsAsync(int n, string reviewPath)
        {
            var random = new Random();
            var reviewTexts = JsonSerializer.Deserialize<IList<string>>(File.ReadAllText(reviewPath)); 

            var uniquePairs = new HashSet<string>();
            var reviews = new List<Review>();
            int userId, beerId;

            while (reviews.Count < n)
            {
                // Generate UserId between 2 and 45
                userId = random.Next(2, 46);

                // Generate BeerId between 1 and 119, ensuring it's not divisible by 10
                do
                {
                    beerId = random.Next(1, 120);
                } while (beerId % 10 == 0);

                // Ensure uniqueness of UserId and BeerId pairs
                var pair = $"{userId}-{beerId}";
                if (!uniquePairs.Contains(pair))
                {
                    uniquePairs.Add(pair);

                    // Create a new Review object
                    var review = new Review
                    {
                        UserId = userId,
                        BeerId = beerId,
                        Text = reviewTexts[random.Next(reviewTexts.Count)],
                        TasteRate = random.Next(4, 11),
                        AromaRate = random.Next(4, 11),
                        FoamRate = random.Next(4, 11),
                        ColorRate = random.Next(4, 11),
                        CreatedAt = DateTime.UtcNow
                    };
                    if (!reviews.Where(x => x.UserId == review.UserId && x.BeerId == review.BeerId).Any())
                    {
                        reviews.Add(review);
                    }
                }
            }

            return reviews;
        }
        
    }
    

}