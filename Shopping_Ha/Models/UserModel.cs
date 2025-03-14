using System.ComponentModel.DataAnnotations;

namespace Shopping_Ha.Models
{
	public class UserModel
	{
		public int Id { get; set; }
		[Required(ErrorMessage ="làm ơn nhập UserName")]	
		
		public string Username { get; set; }
		[Required(ErrorMessage = "làm ơn nhập Email"),EmailAddress]
		public string Email { get; set; }
		[DataType(DataType.Password),Required(ErrorMessage ="Làm ơn nhập Password")]
		public string Password { get; set; }

	}
}
