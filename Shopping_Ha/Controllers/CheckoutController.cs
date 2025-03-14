using Microsoft.AspNetCore.Mvc;
using Shopping_Ha.Areas.Admin.Repository;
using Shopping_Ha.Models;
using Shopping_Ha.Repository;
using System.Security.Claims;

namespace Shopping_Ha.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly DataContext _dataContext;
		private readonly IEmailSender _emailSender;
		public CheckoutController(DataContext context, IEmailSender emailSender)
        {
            _dataContext = context;
            _emailSender = emailSender; 
        }
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> Checkout()
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (userEmail == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                var ordercode = Guid.NewGuid().ToString();
                var orderItem = new OrderModel();
                orderItem.OrderCode = ordercode;
                orderItem.UserName = userEmail;
                orderItem.Status = 1;
                orderItem.CreateDate = DateTime.Now;
                _dataContext.Add(orderItem); 
                _dataContext.SaveChanges();
				List<CartItemModel> cartItems = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
                foreach(var cart in cartItems)
                {
                    var orderdatails = new OrderDetails();
					orderdatails.UserName = userEmail;
					orderdatails.OrderCode = ordercode;
					orderdatails.ProductId = cart.ProductId;
					orderdatails.Price = cart.Price;
                    orderdatails.Quantity = cart.Quantity;
					_dataContext.Add(orderdatails);
					_dataContext.SaveChanges();
				}
				HttpContext.Session.Remove("Cart");
                //Send mail order when success
                var receiver = userEmail;
                var subject = "Đặt hàng thành công";
                var message = "Đặt hàng thành công, trải nghiệm dịch vụ nhé.";

                await _emailSender.SendEmailAsync(receiver, subject, message);

                TempData["success"] = "Checkout thành công, vui lòng chờ duyệt đơn hàng";
				return RedirectToAction("Index", "Cart");
			}
			
		}
    }
}
