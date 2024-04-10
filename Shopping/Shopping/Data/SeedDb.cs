
using Microsoft.EntityFrameworkCore;
using Shopping.Data.Entities;
using Shopping.Enums;
using Shopping.Helpers;

namespace Shopping.Data
{
    public class SeedDb
    {
        private readonly DataContext _context;
        private readonly IUserHelper _userHelper;
        private readonly IBlobHelper _blobHelper;

        public SeedDb(DataContext context, IUserHelper userHelper, IBlobHelper blobHelper)
        {
            _context = context;
            _userHelper = userHelper;
            _blobHelper = blobHelper;
        }

        public async Task SeedAsync()
        {
            await _context.Database.EnsureCreatedAsync();
            await CheckCategoriesAsync();
            await CheckCountries();
            await CheckRolesAsync();
            await CheckUserAsync("001", "Admin", "Admin", "adminshop@yopmail.com", "322 311 4620", "Ed Principal", "admin.png", UserType.Admin);
            await CheckUserAsync("1010", "Lionel", "Messi", "messi@yopmail.com", "304 221 3640", "Miami", "MESSI.jpg", UserType.User);
            await CheckUserAsync("2020", "Cristiano", "Ronaldo", "cr7@yopmail.com", "344 255 4532", "Madrid Vallecas", "cr7.jpg", UserType.User);
            await CheckUserAsync("3030", "Miguel", "Dos Santos", "miguelito@yopmail.com", "322 311 4620", "Calle 10", "user3.png", UserType.User);
            await CheckProductsAsync();
        }

        private async Task CheckProductsAsync()
        {
            if (!_context.Products.Any())
            {
                await AddProductAsync("Camisa Hombre", 270000M, 12F, new List<string>() { "Ropa" }, new List<string>() { "camisa hombre.png" });
                await AddProductAsync("Camisa Atletico Nacional 2006", 450000M, 12F, new List<string>() { "Ropa", "Deportes" }, new List<string>() { "Camisanac-antique.png" });
                await AddProductAsync("Jordan 1 DIOR", 13000000M, 12F, new List<string>() { "Ropa", "Calzado" }, new List<string>() { "j1 dior.png", "j1 dior 2.jpg" });
                await AddProductAsync("Jordan 1 University Blue", 870000M, 12F, new List<string>() { "Ropa", "Calzado" }, new List<string>() { "j1 ublue.png" });
                await AddProductAsync("Mouse Gamer Kalley", 100000M, 12F, new List<string>() { "Tecnologia", "Gamer" }, new List<string>() { "mouse.png" });
                await AddProductAsync("Samsung S20", 2300000M, 12F, new List<string>() { "Tecnologia", "Celulares" }, new List<string>() { "s201.png", "s202.png", "s203.png" });
                await _context.SaveChangesAsync();
            }

        }
        private async Task AddProductAsync(string name, decimal price, float stock, List<string> categories, List<string> images)
        {
            Product prodcut = new()
            {
                Description = name,
                Name = name,
                Price = price,
                Stock = stock,
                ProductCategories = new List<ProductCategory>(),
                ProductImages = new List<ProductImage>()
            };

            foreach (string? category in categories)
            {
                prodcut.ProductCategories.Add(new ProductCategory { Category = await _context.Categories.FirstOrDefaultAsync(c => c.Name == category) });
            }


            foreach (string? image in images)
            {
                Guid imageId = await _blobHelper.UploadBlobAsync($"{Environment.CurrentDirectory}\\wwwroot\\images\\products\\{image}", "products");
                prodcut.ProductImages.Add(new ProductImage { ImageId = imageId });
            }

            _context.Products.Add(prodcut);
        }


        private async Task<User> CheckUserAsync(
            string document,
            string firstName,
            string lastName,
            string email,
            string phone,
            string address,
            string image,
            UserType userType)
                {
                    User user = await _userHelper.GetUserAsync(email);
                    if (user == null)
                    {
                        Guid imageId = await _blobHelper.UploadBlobAsync($"{Environment.CurrentDirectory}\\wwwroot\\images\\users\\{image}", "users");
                        user = new User
                        {
                            FirstName = firstName,
                            LastName = lastName,
                            Email = email,
                            UserName = email,
                            PhoneNumber = phone,
                            Address = address,
                            Document = document,
                            City = _context.Cities.FirstOrDefault(),
                            UserType = userType,
                            ImageId= imageId
                        };

                        await _userHelper.AddUserAsync(user, "123456");
                        await _userHelper.AddUserToRoleAsync(user, userType.ToString());

                        string token = await _userHelper.GenerateEmailConfirmationTokenAsync(user);
                        await _userHelper.ConfirmEmailAsync(user, token);

                    }

                    return user;
        }


        private async Task CheckRolesAsync()
        {
            await _userHelper.CheckRoleAsync(UserType.Admin.ToString());
            await _userHelper.CheckRoleAsync(UserType.User.ToString());
        }

        private async Task CheckCountries()
        {
            if (!_context.Countries.Any())
            {
                _context.Countries.Add(new Country 
                { 
                    Name = "Colombia",
                    States = new List<State>()
                    {
                        new State
                        {
                            Name = "Antioquia",
                            Cities = new List<City>()
                            {
                                new City { Name = "Medellín" },
                                new City { Name = "Envigado" },
                                new City { Name = "Bello" },
                                new City { Name = "Rionegro" },
                                new City { Name = "Barbosa" },
                            }
                        },
                        new State
                        {
                            Name = "Bogota",
                            Cities = new List<City>()
                            {
                                new City { Name = "Bosa" },
                                new City { Name = "Chapinero" },
                                new City { Name = "Usaquen" },
                                new City { Name = "Usme" },
                            }
                        }
                    }
                });

                _context.Countries.Add(new Country
                {
                    Name = "Estados Unidos",
                    States = new List<State>()
                    {
                        new State
                        {
                            Name = "Florida",
                            Cities = new List<City>()
                            {
                                new City { Name = "Orlando" },
                                new City { Name = "Miami" },
                                new City { Name = "Tampa" },
                                new City { Name = "Key West" }
                            }
                        },
                        new State
                        {
                            Name = "Texas",
                            Cities = new List<City>()
                            {
                                new City { Name = "Houston" },
                                new City { Name = "San Antonio" },
                                new City { Name = "Dallas" },
                                new City { Name = "Austin" },
                            }
                        }
                    }
                });

                await _context.SaveChangesAsync();
            }
        }

        private async Task CheckCategoriesAsync()
        {
            if (!_context.Categories.Any())
            {
                _context.Categories.Add(new Category { Name = "Tecnología" });
                _context.Categories.Add(new Category { Name = "Ropa" });
                _context.Categories.Add(new Category { Name = "Calzado" });
                _context.Categories.Add(new Category { Name = "Belleza" });
                _context.Categories.Add(new Category { Name = "Gamer" });
                _context.Categories.Add(new Category { Name = "Nutricíon" });
                _context.Categories.Add(new Category { Name = "Deportes" });
                _context.Categories.Add(new Category { Name = "Mascotas" });
                _context.Categories.Add(new Category { Name = "Celulares" });
                await _context.SaveChangesAsync();
            }    
        }
    }
}
