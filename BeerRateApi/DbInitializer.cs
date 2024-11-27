using BeerRateApi.Models;
using System.Text.Json;

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
                string beerImagePath = $"{currentPath}\\TestData\\BeerImage.json";
                string usersPath = $"{currentPath}\\TestData\\Users.json";
                string reviewsPath = $"{currentPath}\\TestData\\Reviews.json";

                //Json deserialization
                string beersJson = await File.ReadAllTextAsync(beersPath);
                string usersJson = await File.ReadAllTextAsync(usersPath);
                string reviewsJson = await File.ReadAllTextAsync(reviewsPath);

                //Collections of objects which will be added to database
                IEnumerable<Beer> beers = JsonSerializer.Deserialize<IEnumerable<Beer>>(beersJson);
                IEnumerable<UserInitializer> usersData = JsonSerializer.Deserialize<IEnumerable<UserInitializer>>(usersJson);
                IEnumerable<Review> reviews = JsonSerializer.Deserialize<IEnumerable<Review>>(reviewsJson);
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
                byte[] imageFile = await File.ReadAllBytesAsync(beersPath);
                string fileType = File.GetAttributes(beersPath).GetType().FullName;
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
                //context.Reviews.AddRangeAsync(reviews);

                context.SaveChanges();
            }
            
        }
    }
}