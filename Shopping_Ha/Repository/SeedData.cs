using Microsoft.EntityFrameworkCore;
using Shopping_Ha.Models;

namespace Shopping_Ha.Repository
{
	public class SeedData
	{
		public static void SeedingData(DataContext _context)
		{
			_context.Database.Migrate();
			if (!_context.Products.Any())
			{
				CategoryModel mackBook = new CategoryModel { Name = "MackBook", Slug = "MackBook", Description= "MackBook is Large Brand in the word", Status=1} ;
				CategoryModel pc = new CategoryModel { Name = "pc", Slug = "pc", Description = "pc is Large Brand in the word", Status = 1 };
				BrandModel apple = new BrandModel { Name = "apple", Slug = "apple", Description = "apple is Large Brand in the word", Status = 1 };
				BrandModel samsung = new BrandModel { Name = "samsung", Slug = "samsung", Description = "samsung is Large Brand in the word", Status = 1 };
				_context.Products.AddRange(
					new ProductModel { Name = "MackBook", Slug = "MackBook", Description = "MackBook is Best", Image = "1.jpg", Category = mackBook, Brand = apple, Price = 1233 },
					new ProductModel { Name = "pc", Slug = "pc", Description = "pc is Best", Image = "1.jpg", Category = pc, Brand = samsung, Price = 1233 }
				);
				_context.SaveChanges();
			}
		}
	}
}
