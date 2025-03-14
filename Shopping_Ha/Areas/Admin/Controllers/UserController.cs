using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Shopping_Ha.Models;
using Shopping_Ha.Repository;
using System.Security.Claims;

namespace Shopping_Ha.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        private readonly UserManager<AppUserModel> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly DataContext _dataContext;

        public UserController(DataContext context ,UserManager<AppUserModel> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _dataContext = context;
        }
        [HttpGet]
        [Route("Index")]
        public async Task<IActionResult> Index()
        {
            var usersWithRoles = await (from u  in _dataContext.Users
                                        join ur in _dataContext.UserRoles on u.Id equals ur.UserId
                                        join r in _dataContext.Roles on ur.RoleId equals r.Id
                                        select new {User = u , RoleName = r.Name })
                                        .ToListAsync();
            var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            ViewBag.LoggedInUserId = loggedInUserId;
                    
            return View(usersWithRoles);
        }
        [HttpGet]
        [Route("Create")]
        public async Task<IActionResult> Create()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            ViewBag.Roles = new SelectList(roles, "Id", "Name");
            return View(new AppUserModel());
        }

		[HttpGet]
		[Route("Edit")]
		public async Task<IActionResult> Edit(string id)
		{

			if (string.IsNullOrEmpty(id))
			{
				return NotFound();
			}
			var user = await _userManager.FindByIdAsync(id);
			if (user == null)
			{   
				return NotFound();
			}

			var roles = await _roleManager.Roles.ToListAsync();
			ViewBag.Roles = new SelectList(roles, "Id", "Name");

			return View(user);
		}

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Edit")]
        public async Task<IActionResult> Edit(string id,AppUserModel user)
        {
            var existingUser = await _userManager.FindByIdAsync(id);//lấy user dựa vào id
            if(existingUser == null)
            {
                return NotFound();
            }
            if(ModelState.IsValid)
            {
                //update
                existingUser.UserName = user.UserName;
                existingUser.Email = user.Email;
                existingUser.PhoneNumber = user.PhoneNumber;
                existingUser.RoleId = user.RoleId;

                var updateUserResult = await _userManager.UpdateAsync(existingUser);
                if (updateUserResult.Succeeded)
                {
                    return RedirectToAction("Index", "User");
                }
                else
                {
                    AddidentityErrors(updateUserResult);
                    return View(existingUser);
                }

            }
            var roles = await _roleManager.Roles.ToListAsync();
            ViewBag.Roles = new SelectList(roles, "Id", "Name");
            ////

            TempData["error"] = "Model Validation failed";
            var errors = ModelState.Values.SelectMany(v=>v.Errors.Select(e => e.ErrorMessage)).ToList();
            string errorMessage = string.Join("\n",errors);
            return View(existingUser);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
		[Route("Create")]
		public async Task<IActionResult> Create(AppUserModel user)
		{
			if(ModelState.IsValid)
            {
                var createUserResult = await _userManager.CreateAsync(user,user.PasswordHash);//tạo user

                if(createUserResult.Succeeded)
                {
                    var createUser = await _userManager.FindByEmailAsync(user.Email);//Tìm user dựa vào email
                    var userId = createUser.Id;//Lấy  user id
                    var role = _roleManager.FindByIdAsync(user.RoleId);//lấy roleId
                    //gán quyền
                    var addToRoleResult = await _userManager.AddToRoleAsync(createUser, role.Result.Name);
                    if(!addToRoleResult.Succeeded)
                    {
                        foreach (var error in createUserResult.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }
                    return RedirectToAction("Index", "User");
                }
                else
                {
					foreach (var error in createUserResult.Errors)
					{
						ModelState.AddModelError(string.Empty, error.Description);
					}
					
                    return View(user);
                }
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
			var roles = await _roleManager.Roles.ToListAsync();
			ViewBag.Roles = new SelectList(roles, "Id", "Name");
			return View(user);

		}

		[HttpGet]
		[Route("Delete")]
		public async Task<IActionResult> Delete(string id)
		{
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }
            var user = await _userManager.FindByIdAsync(id);
            if(user == null)
            {
                return NotFound();
            }
            var deleteResult = await _userManager.DeleteAsync(user);
            if(!deleteResult.Succeeded)
            {
                return View("Error");
            }
			TempData["success"] = "User đã xóa thành công";
			return RedirectToAction("Index");
		}
        
        private void AddidentityErrors(IdentityResult identityResult)
        {
            foreach(var error in identityResult.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }
	}
}
