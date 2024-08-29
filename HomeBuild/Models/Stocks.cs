using System.ComponentModel.DataAnnotations;

namespace HomeBuild.Models
{
	public class Stocks
	{
		[Key]
		public int Id { get; set; }
		public string UrlImg { get; set; }
		public string Product { get; set; }
		public string Type { get; set; }
		public string Color { get; set; }
		public string Size { get; set; }
		public int Quantity { get; set; }
		public decimal Price { get; set; }
	}
}
