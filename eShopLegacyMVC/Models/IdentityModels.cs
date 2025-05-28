using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Net.Http;
using System.Threading.Tasks;

namespace eShopLegacyMVC.Models
{
    public class ApplicationUser : IdentityUser
    {
        private int? _zipCode = null;

        public int? ZipCode
        {
            get
            {
                if (_zipCode is null)
                {
                    using var httpClient = new HttpClient();
                    var uri = $"http://10.0.0.42/UserLookup.svc/zipCode?id={Id}";
                    var response = httpClient.GetStringAsync(uri).GetAwaiter().GetResult();
                    _zipCode = int.Parse(response);
                }
                return _zipCode;
            }
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}
