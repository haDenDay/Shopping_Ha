using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Shopping_Ha.Models;
using Shopping_Ha.Repository;

namespace Shopping_Ha.Areas.Admin.Controllers
{
	[Area("Admin")]
    [Authorize(Roles = "Publisher,Author")]
    public class CategoryController : Controller
	{
		private readonly DataContext _dataContext;
		
		public CategoryController(DataContext context)
		{
			_dataContext = context;
			
		}
		public async Task<IActionResult> Index(int pg = 1)
		{
			List<CategoryModel> category = _dataContext.Categories.ToList();
			const int pageSize = 10; //10item/trag
			if (pg < 1) //page<1
			{
				pg=1;//page =1
			}
			int recsCount = category.Count();
			var pager = new Paginate(recsCount, pg, pageSize);
			int recSkip = (pg-1) * pageSize;//(3-1)*10
            //category.Skip(20).Take(10).ToList()
            var data = category.Skip(recSkip).Take(pager.PageSize).ToList();
			ViewBag.Pager = pager;	
			return View(data);
		}
		public async Task<IActionResult> Edit(int Id)
		{
			CategoryModel category = await _dataContext.Categories.FindAsync(Id);
			return View(category);
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(CategoryModel category)
		{

			if (ModelState.IsValid)
			{
				//code thêm dữ liệu
				category.Slug = category.Name.Replace(" ", "-");
				var slug = await _dataContext.Products.FirstOrDefaultAsync(p => p.Slug == category.Slug);
				if (slug != null)
				{
					ModelState.AddModelError("", "Danh mục đã có trong database");
					return View(category);
				}

				_dataContext.Update(category);
				await _dataContext.SaveChangesAsync();
				TempData["success"] = "Cập nhập danh mục  thành công";
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

		public IActionResult Create()
		{
			return View();
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(CategoryModel category)
		{
			
			if (ModelState.IsValid)
			{
				//code thêm dữ liệu
				category.Slug = category.Name.Replace(" ", "-");
				var slug = await _dataContext.Products.FirstOrDefaultAsync(p => p.Slug == category.Slug);
				if (slug != null)
				{
					ModelState.AddModelError("", "Danh mục đã có trong database");
					return View(category);
				}

				_dataContext.Add(category);
				await _dataContext.SaveChangesAsync();
				TempData["success"] = "Thêm danh mục  thành công";
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
		
		public async Task<IActionResult> Delete(int Id)
		{
			CategoryModel category = await _dataContext.Categories.FindAsync(Id);
			_dataContext.Categories.Remove(category);
			await _dataContext.SaveChangesAsync();
			TempData["success"] = "Danh mục đã xóa";
			return RedirectToAction("Index");
		}
	}
}
