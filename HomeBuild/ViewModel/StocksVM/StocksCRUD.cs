namespace HomeBuild.ViewModel.StocksVM
{
	public class StocksCRUD
	{
		public int Id { get; set; }
		public string Product { get; set; }
		public string Type { get; set; }
		public string Color { get; set; }
		public string Size { get; set; }
		public int Quantity { get; set; }
		public decimal Price { get; set; }
		public IFormFile UrlImage { get; set; }
	}
}
