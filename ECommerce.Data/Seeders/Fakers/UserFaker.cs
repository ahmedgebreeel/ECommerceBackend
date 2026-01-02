using Bogus;
using ECommerce.Core.Entities;

namespace ECommerce.Data.Seeders.Fakers
{
    public static class UserFaker
    {
        public static Faker<ApplicationUser> GetUser()
        {
            return new Faker<ApplicationUser>()
                .RuleFor(u => u.FirstName, f => f.Name.FirstName())
                .RuleFor(u => u.LastName, f => f.Name.LastName())
                .RuleFor(u => u.UserName, (f, u) => f.Internet.UserName(u.FirstName, u.LastName))
                .RuleFor(u => u.Email, (f, u) => f.Internet.Email(u.FirstName, u.LastName, "test.com"))
                .RuleFor(u => u.PhoneNumber, f => f.Phone.PhoneNumber("##########"))
                .RuleFor(u => u.Created, f => DateTime.UtcNow)
                .RuleFor(u => u.Created, f => f.Date.Past(2))
                .RuleFor(u => u.Updated, (f, u) => f.Date.Between(u.Created, DateTime.UtcNow))
                .RuleFor(u => u.Addresses, _ => AddressFaker.GetAddress().Generate(3));
        }
    }
}