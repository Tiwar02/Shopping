
using Shopping.Data.Entities;
using Shopping.Enums;
using Shopping.Helpers;

namespace Shopping.Data
{
    public class SeedDb
    {
        private readonly DataContext _context;
        private readonly IUserHelper _userHelper;

        public SeedDb(DataContext context, IUserHelper userHelper)
        {
            _context = context;
            _userHelper = userHelper;
        }

        public async Task SeedAsync()
        {
            await _context.Database.EnsureCreatedAsync();
            await CheckCategoriesAsync();
            await CheckCountries();
            await CheckRolesAsync();
            await CheckUserAsync("1010", "Pablito", "Perez", "Pape@yopmail.com", "322 311 4620", "Calle Luna Calle Sol", UserType.Admin);
            await CheckUserAsync("3030", "Elkin", "Tico", "elkintico@yopmail.com", "322 311 4620", "Calle Luna Calle Sol", UserType.User);
        }

        private async Task<User> CheckUserAsync(
            string document,
            string firstName,
            string lastName,
            string email,
            string phone,
            string address,
            UserType userType)
                {
                    User user = await _userHelper.GetUserAsync(email);
                    if (user == null)
                    {
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
                _context.Categories.Add(new Category { Name = "Nutricíon" });
                _context.Categories.Add(new Category { Name = "Deportes" });
                _context.Categories.Add(new Category { Name = "Mascotas" });
                await _context.SaveChangesAsync();
            }    
        }
    }
}
