using MarketplaceAPI.Models;

namespace MarketplaceAPI
{
    public class DbPasswordUpdater
    {
        public static void UpdatePasswords(ApplicationDbContext context)
        {
            var users = context.Users.ToList();

            foreach (var user in users)
            {
                if (user.PasswordHash == "TEMP_HASH")
                {
                    user.PasswordHash = BCrypt.Net.BCrypt.HashPassword("123456");
                }
            }

            context.SaveChanges();
            Console.WriteLine("Пароли обновлены!");
        }
    }
}