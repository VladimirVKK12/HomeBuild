using HomeBuild.Models;
using HomeBuild.Repositories;
using HomeBuild.ViewModel.StocksVM;
using Microsoft.AspNetCore.Mvc;
using System.Drawing;

namespace HomeBuild.Controllers
{
	public class StocksController : Controller
	{
		private readonly StocksRepository _stocksRepository;
		private readonly ShoppingCartRepository _shoppingCartRepository;

		public StocksController(StocksRepository stocksRepository, ShoppingCartRepository shoppingCartRepository)
		{
			_stocksRepository = stocksRepository;
			_shoppingCartRepository = shoppingCartRepository;
		}

		// Извличане на всички продукти и предаване на данните към изгледа
		public async Task<IActionResult> Table()
		{
			var stocks = await _stocksRepository.GetAll(); // извличане на всички продукти
			return View(stocks);
		}

		// Извличане на изгледа за създаване на нов продукт
		[HttpGet]
		public IActionResult Create()
		{
			return View();
		}

		// Създаване на нов продукт
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(StocksCRUD model)
		{
			if (ModelState.IsValid)
			{
				await _stocksRepository.Create(model); // създаване на нов продукт в базата данни

				return RedirectToAction("Table");
			}
			return View(model);
		}

		// Извличане на изгледа за редактиране на продукт
		[HttpGet]
		public async Task<IActionResult> Edit(StocksCRUD model, int id)
		{
			var stocks = await _stocksRepository.GetById(id);
			var fileName = _stocksRepository.Photo(model); // вземане на името на снимката на продукта

			model = new StocksCRUD
			{
				Id = stocks.Id,
				Product = stocks.Product,
				Type = stocks.Type,
				Color = stocks.Color,
				Size = stocks.Size,
				Quantity = stocks.Quantity,
				Price = stocks.Price,
			};

			return View(model);
		}

		// Редактиране на съществуващ продукт, след като бъде извлечен по id
		[HttpPost]
		public async Task<IActionResult> Edit(int id, StocksCRUD model)
		{
			ModelState.Remove("UrlImage");
			if (ModelState.IsValid)
			{
				var stocks = await _stocksRepository.GetById(id);

				await _stocksRepository.Update(model);

				await _shoppingCartRepository.UpdateCartItems(stocks);

				return RedirectToAction("Table");
			}

			return View(model);
		}

		// Изтриване на продукт
		public async Task<IActionResult> Delete(int id)
		{
			await _stocksRepository.Delete(id);
			return RedirectToAction("Table");
		}

		// Извличане на изгледа за детайли на продукт или търсене на продукт по ключова дума
		public async Task<IActionResult> GetProductDetails(int id, string searchWord)
		{
			var searchResults = await _stocksRepository.StocksSearch(searchWord);
			if (searchResults == null || searchResults.Count == 0)
			{
				var stocks = await _stocksRepository.GetById(id);
				return View("GetProductDetails", stocks);
			}
			else
			{
				var model = searchResults.FirstOrDefault();
				return View(model);
			}
		}
	}
}
