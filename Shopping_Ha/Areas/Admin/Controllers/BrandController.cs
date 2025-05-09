﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shopping_Ha.Models;
using Shopping_Ha.Repository;

namespace Shopping_Ha.Areas.Admin.Controllers
{
	[Area("Admin")]
    [Authorize(Roles ="Publisher,Author,Admin")]
    public class BrandController : Controller
    {
        private readonly DataContext _dataContext;

        public BrandController(DataContext context)
        {
            _dataContext = context;

        }
        //public async Task<IActionResult> Index()
        //{
        //    return View(await _dataContext.Brands.OrderByDescending(p => p.Id).ToListAsync());
        //}

        public async Task<IActionResult> Index(int pg = 1)
        {
            List<BrandModel> category = _dataContext.Brands.ToList();
            const int pageSize = 10; //10item/trag
            if (pg < 1) //page<1
            {
                pg = 1;//page =1
            }
            int recsCount = category.Count();
            var pager = new Paginate(recsCount, pg, pageSize);
            int recSkip = (pg - 1) * pageSize;//(3-1)*10
            //category.Skip(20).Take(10).ToList()
            var data = category.Skip(recSkip).Take(pager.PageSize).ToList();
            ViewBag.Pager = pager;
            return View(data);
        }
        public async Task<IActionResult> Create()
		{
			return View();
		}

		public async Task<IActionResult> Edit(int Id)
		{
			BrandModel brand = await _dataContext.Brands.FindAsync(Id);
			return View(brand);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(BrandModel brand)
		{

			if (ModelState.IsValid)
			{
				//code thêm dữ liệu
				brand.Slug = brand.Name.Replace(" ", "-");
				var slug = await _dataContext.Products.FirstOrDefaultAsync(p => p.Slug == brand.Slug);
				if (slug != null)
				{
					ModelState.AddModelError("", "Thương hiệu đã đã có trong database");
					return View(brand);
				}

				_dataContext.Update(brand);
				await _dataContext.SaveChangesAsync();
				TempData["success"] = "Cập nhập thương hiệu  thành công";
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

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(BrandModel brand)
		{

			if (ModelState.IsValid)
			{
				//code thêm dữ liệu
				brand.Slug = brand.Name.Replace(" ", "-");
				var slug = await _dataContext.Products.FirstOrDefaultAsync(p => p.Slug == brand.Slug);
				if (slug != null)
				{
					ModelState.AddModelError("", "Thương hiệu đã đã có trong database");
					return View(brand);
				}

				_dataContext.Add(brand);
				await _dataContext.SaveChangesAsync();
				TempData["success"] = "Thêm thương hiệu  thành công";
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
            BrandModel brand = await _dataContext.Brands.FindAsync(Id);
            _dataContext.Brands.Remove(brand);
            await _dataContext.SaveChangesAsync();
            TempData["success"] = "Thương hiệu đã xóa đã xóa";
            return RedirectToAction("Index");
        }
    }
}
