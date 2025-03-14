using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Shopping_Ha.Models;
using Shopping_Ha.Repository;

namespace Shopping_Ha.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize(Roles ="Admin")]
	public class ProductController : Controller
	{
		private readonly DataContext _dataContext;
		private readonly IWebHostEnvironment _webHosEnviroment;
		public ProductController(DataContext context, IWebHostEnvironment webHosEnviroment)
		{
			_dataContext = context;
			_webHosEnviroment = webHosEnviroment;
		}
		//public async Task<IActionResult> Index()
		//{
		//	return View(await _dataContext.Products.OrderByDescending(p => p.Id).Include(p => p.Category).Include(p => p.Brand).ToListAsync());
		//}

        public async Task<IActionResult> Index(int pg = 1)
        {
            const int pageSize = 10;
            if (pg < 1)
            {
                pg = 1;
            }

            int recsCount = await _dataContext.Products.CountAsync(); // Đếm tổng số sản phẩm

            var pager = new Paginate(recsCount, pg, pageSize);
            int recSkip = (pg - 1) * pageSize;

            var data = await _dataContext.Products
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .OrderByDescending(p => p.Id)
                .Skip(recSkip)
                .Take(pager.PageSize)
                .ToListAsync();

            ViewBag.Pager = pager;
            return View(data);
        }

        
        [HttpGet]
		public IActionResult Create()
		{
			ViewBag.Categories = new SelectList(_dataContext.Categories, "Id", "Name");
			ViewBag.Brands = new SelectList(_dataContext.Brands, "Id", "Name");

			return View();
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(ProductModel product)
		{
			ViewBag.Categories = new SelectList(_dataContext.Categories, "Id", "Name", product.CategoryId);
			ViewBag.Brands = new SelectList(_dataContext.Brands, "Id", "Name", product.BrandId);
			if (ModelState.IsValid)
			{
				//code thêm dữ liệu
				product.Slug = product.Name.Replace(" ", "-");
				var slug = await _dataContext.Products.FirstOrDefaultAsync(p => p.Slug == product.Slug);
				if (slug != null)
				{
					ModelState.AddModelError("", "Sản phẩm đã có trong database");
					return View(product);
				}


				if (product.ImageUpload != null)
				{
					string uploadsDir = Path.Combine(_webHosEnviroment.WebRootPath, "media/products");
					string imageName = Guid.NewGuid().ToString() + "_" + product.ImageUpload.FileName;
					string filePath = Path.Combine(uploadsDir, imageName);

					FileStream fs = new FileStream(filePath, FileMode.Create);
					await product.ImageUpload.CopyToAsync(fs);
					fs.Close();
					product.Image = imageName;
				}

				_dataContext.Add(product);
				await _dataContext.SaveChangesAsync();
				TempData["success"] = "Thêm sản phẩm thành công";
				return RedirectToAction("Index");
			}
			else
			{
				TempData["error"] = "Model có 1 vài thứ đang bị lỗi";
				List<string> errors = new List<string>();
				foreach (var value in ModelState.Values)
				{
					foreach (var error in value.Errors)
					{
						errors.Add(error.ErrorMessage);
					}
				}
				string errorMesssage = string.Join("\n", errors);
				return BadRequest(errorMesssage);
			}
		}

		public async Task<IActionResult> Edit(int Id)
		{
			ProductModel product = await _dataContext.Products.FindAsync(Id);
			ViewBag.Categories = new SelectList(_dataContext.Categories, "Id", "Name", product.CategoryId);
			ViewBag.Brands = new SelectList(_dataContext.Brands, "Id", "Name", product.BrandId);
			return View(product);
		}
		
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(int Id, ProductModel product)
		{
			// Tìm sản phẩm trong cơ sở dữ liệu
			var existedProduct = await _dataContext.Products.FindAsync(Id);
			if (existedProduct == null)
			{
				return NotFound();
			}

			// Đảm bảo dữ liệu hợp lệ
			if (ModelState.IsValid)
			{
				// Cập nhật tên nếu có thay đổi, nếu không giữ nguyên
				if (!string.IsNullOrEmpty(product.Name))
				{
					existedProduct.Name = product.Name;
				}

				// Cập nhật mô tả nếu có thay đổi, nếu không giữ nguyên
				if (!string.IsNullOrEmpty(product.Description))
				{
					existedProduct.Description = product.Description;
				}

				// Cập nhật giá nếu có thay đổi và giá hợp lệ (không phải 0)
				if (product.Price > 0)
				{
					existedProduct.Price = product.Price;
				}

				// Cập nhật CategoryId nếu có thay đổi
				if (product.CategoryId != 0)
				{
					existedProduct.CategoryId = product.CategoryId;
				}

				// Cập nhật BrandId nếu có thay đổi
				if (product.BrandId != 0)
				{
					existedProduct.BrandId = product.BrandId;
				}

				// Kiểm tra và xử lý ảnh upload
				if (product.ImageUpload != null)
				{
					string uploadsDir = Path.Combine(_webHosEnviroment.WebRootPath, "media/products");
					string imageName = Guid.NewGuid().ToString() + "_" + product.ImageUpload.FileName;
					string filePath = Path.Combine(uploadsDir, imageName);

					// Xóa ảnh cũ nếu có
					string oldFileImage = Path.Combine(uploadsDir, existedProduct.Image);
					if (System.IO.File.Exists(oldFileImage))
					{
						System.IO.File.Delete(oldFileImage);
					}

					// Lưu ảnh mới
					using (FileStream fs = new FileStream(filePath, FileMode.Create))
					{
						await product.ImageUpload.CopyToAsync(fs);
					}

					existedProduct.Image = imageName;  // Cập nhật tên ảnh
				}

				// Cập nhật sản phẩm trong cơ sở dữ liệu
				_dataContext.Update(existedProduct);
				await _dataContext.SaveChangesAsync();

				TempData["success"] = "Cập nhật sản phẩm thành công";
				return RedirectToAction("Index");
			}

			TempData["error"] = "Model có một vài lỗi";
			return View(product);
		}



		//public async Task<IActionResult> Delete(int Id)
		//{
		//	ProductModel product = await _dataContext.Products.FindAsync(Id);

		//	// Kiểm tra nếu ảnh sản phẩm không phải là ảnh mặc định "noname.jpg"

		//	if (!string.Equals(product.Image, "noname.jpg"))
		//	{
		//		string uploadsDir = Path.Combine(_webHosEnviroment.WebRootPath, "media/products");
		//		string oldFileImagePath = Path.Combine(uploadsDir, product.Image);

		//		// Kiểm tra xem tệp tin ảnh có tồn tại không
		//		if (System.IO.File.Exists(oldFileImagePath))
		//		{
		//			try
		//			{
		//				// Xóa ảnh nếu tồn tại
		//				System.IO.File.Delete(oldFileImagePath);
		//			}
		//			catch (Exception ex)
		//			{
		//				// Ghi log nếu có lỗi khi xóa ảnh
		//				TempData["error"] = "Lỗi khi xóa ảnh: " + ex.Message;
		//				return RedirectToAction("Index");
		//			}
		//		}
		//		else
		//		{
		//			// Thông báo nếu ảnh không tồn tại
		//			TempData["error"] = "Ảnh không tồn tại!";
		//			return RedirectToAction("Index");
		//		}
		//	}

		//	// Xóa sản phẩm
		//	_dataContext.Products.Remove(product);
		//	await _dataContext.SaveChangesAsync();

		//	TempData["success"] = "Sản phẩm đã xóa thành công";
		//	return RedirectToAction("Index");
		//}

		public async Task<IActionResult> Delete(int Id)
		{
			// Tìm sản phẩm theo Id
			ProductModel product = await _dataContext.Products.FindAsync(Id);

			// Kiểm tra nếu sản phẩm không tồn tại
			if (product == null)
			{
				TempData["error"] = "Sản phẩm không tồn tại!";
				return RedirectToAction("Index");
			}

			// Kiểm tra nếu Image bị null hoặc rỗng, gán giá trị mặc định
			if (string.IsNullOrEmpty(product.Image))
			{
				product.Image = "noname.jpg"; // Ảnh mặc định
			}

			// Kiểm tra nếu ảnh sản phẩm không phải là ảnh mặc định
			if (!string.Equals(product.Image, "noname.jpg"))
			{
				string uploadsDir = Path.Combine(_webHosEnviroment.WebRootPath, "media/products");
				string oldFileImagePath = Path.Combine(uploadsDir, product.Image);

				// Kiểm tra xem tệp tin ảnh có tồn tại không
				if (System.IO.File.Exists(oldFileImagePath))
				{
					try
					{
						System.IO.File.Delete(oldFileImagePath);
					}
					catch (Exception ex)
					{
						TempData["error"] = "Lỗi khi xóa ảnh: " + ex.Message;
						return RedirectToAction("Index");
					}
				}
				else
				{
					TempData["error"] = "Ảnh không tồn tại!";
					return RedirectToAction("Index");
				}
			}

			// Xóa sản phẩm
			_dataContext.Products.Remove(product);
			await _dataContext.SaveChangesAsync();

			TempData["success"] = "Sản phẩm đã xóa thành công";
			return RedirectToAction("Index");
		}


	}
}