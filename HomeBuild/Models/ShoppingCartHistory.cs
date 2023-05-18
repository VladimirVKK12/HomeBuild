using System.ComponentModel.DataAnnotations;

namespace HomeBuild.Models
{
	public class ShoppingCartHistory
	{
		[Key]
		public int Id { get; set; }
		public int ProductId { get; set; }
		public string ProductName { get; set; }
		public decimal Price { get; set; }
		public int Quantity { get; set; }
		public string Color { get; set; }
		public string Size { get; set; }
		public string UserId { get; set; }
		public string Usernamne { get; set; }
		public decimal ProductTotal { get; set; }
		public decimal PriceTotal { get; set; }
		public DateTime PurchaseDate { get; set; }
		public bool Payed { get; set; }
		public ApplicationUser User { get; set; }
	}
}
