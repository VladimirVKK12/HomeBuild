using HomeBuild.Models;
using System.ComponentModel.DataAnnotations;

namespace HomeBuild.ViewModel.ShoppingCartVM
{
	public class ShoppingCartCRUD
	{
		public int Id { get; set; }
		public int ProductId { get; set; }
		public IFormFile UrlImg { get; set; }
		public string ProductName { get; set; }
		public decimal Price { get; set; }
		public int Quantity { get; set; }
		public decimal PriceTotal { get; set; }
		public string Color { get; set; }
		public string Size { get; set; }
		public string UserId { get; set; }
		public bool Payed { get; set; }
		public Dictionary<string, decimal> ProductTotal { get; set; }
		public ApplicationUser User { get; set; }
		public DateTime PurchaseDate { get; set; }
		public IEnumerable<ShoppingCart> ShoppingCarts { get; set; }
		public IEnumerable<ShoppingCartHistory> ShoppingCartHistories { get; set; }
	}
}
