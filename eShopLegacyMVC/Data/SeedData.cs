using eShopLegacyMVC.Models;
using eShopLegacyMVC.Models.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace eShopLegacyMVC.Data
{
    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new CatalogDBContext(
                serviceProvider.GetRequiredService<DbContextOptions<CatalogDBContext>>()))
            {
                if (context.CatalogItems.Any())
                {
                    return;   // DB has been seeded
                }

                var useCustomizationData = serviceProvider.GetRequiredService<IConfiguration>()
                    .GetValue<bool>("AppSettings:UseCustomizationData");

                if (useCustomizationData)
                {
                    PreconfiguredData.GetPreconfiguredCatalogItems().ForEach(i => context.CatalogItems.Add(i));
                    PreconfiguredData.GetPreconfiguredCatalogBrands().ForEach(i => context.CatalogBrands.Add(i));
                    PreconfiguredData.GetPreconfiguredCatalogTypes().ForEach(i => context.CatalogTypes.Add(i));
                }

                context.SaveChanges();
            }
        }
    }
}
