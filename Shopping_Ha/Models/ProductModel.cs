using Shopping_Ha.Repository.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shopping_Ha.Models
{
	public class ProductModel
	{
		[Key]
		public int Id { get; set; }
		[Required, MinLength(4, ErrorMessage = "Yêu cầu nhập tên Sản phẩm")]
		public string Name { get; set; }
		public string Slug { get; set; }
		[Required, MinLength(4, ErrorMessage = "Yêu cầu nhập mô tả Sản phẩm	")]
		public string Description { get; set; }
		[Required, Range(0, double.MaxValue, ErrorMessage = "Giá sản phẩm phải lớn hơn hoặc bằng 0")]
		public decimal Price { get; set; }
		public int BrandId { get; set; }
		public int CategoryId { get; set; }
		public CategoryModel Category { get; set; }
		public BrandModel Brand { get; set; }
		public string Image { get; set; }

		[NotMapped]
		[FileExtension]
		public IFormFile? ImageUpload { get; set; }
	}
}
