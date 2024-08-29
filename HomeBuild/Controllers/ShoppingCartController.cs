using HomeBuild.Data;
using HomeBuild.Models;
using HomeBuild.Repositories;
using HomeBuild.ViewModel.ShoppingCartVM;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace HomeBuild.Controllers
{
	public class ShoppingCartController : Controller
	{
		private readonly ShoppingCartRepository _shoppingCartRepository;
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly ApplicationDbContext _db;

		public ShoppingCartController(ShoppingCartRepository shoppingCartRepository,
		UserManager<ApplicationUser> userManager, ApplicationDbContext db)
		{
			_shoppingCartRepository = shoppingCartRepository;
			_userManager = userManager;
			_db = db;
		}


		// Добавя продукт към количката на потребителя
		public async Task<IActionResult> AddToCart(int productId, string color, string size, int quantity = 1)
		{
			var user = await _userManager.GetUserAsync(User); // взема логнатия потребител
			var product = await _db.Stocks.FindAsync(productId);
			await _shoppingCartRepository.AddToCart(productId, color, quantity, user.Id, size);

			return RedirectToAction("Index", "Home");
		}

		// Премахва продукт от количката на потребителя
		[HttpPost]
		public async Task<IActionResult> RemoveFromCart(int id)
		{
			await _shoppingCartRepository.RemoveFromCart(id);

			return RedirectToAction("MyPurchases");
		}

		// Обновява количеството на продукта в количката на потребителя
		[HttpPost]
		public async Task<IActionResult> UpdateQuantity(int id, int quantity)
		{
			await _shoppingCartRepository.UpdateQuantity(id, quantity);

			return RedirectToAction("MyPurchases");
		}

		// Извеждане на продуктите в кошницата на потребителя
		public async Task<IActionResult> MyPurchases()
		{
			if (User.Identity.IsAuthenticated)
			{
				var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);//намира идентификатора на логнатия потребител
				var cartItems = await _shoppingCartRepository.UserShoppingCart(userId);//извлича количката на потребителя

				return View(cartItems);
			}
			else
			{
				return RedirectToAction("LogIn", "Accounts");
			}
		}

		// Извеждане на историята на покупките на потребителя
		public async Task<IActionResult> PurchasesHistory(bool payment)
		{
			if (User.Identity.IsAuthenticated)
			{
				var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
				var historyItems = await _shoppingCartRepository.OrderHistory(userId,payment);//извлича историята на потребителя
				return View(historyItems);
			}
			else
			{
				return RedirectToAction("LogIn", "Accounts");
			}
		}

		[HttpGet,HttpPost]
        public async Task<IActionResult> EveryPurchase(List<int> paidIds)
        {
            var orderItems = await _db.ShoppingCartHistories.ToListAsync();

            if (ModelState.IsValid)
            {
                foreach (var item in orderItems)
                {
                    item.Payed = paidIds.Contains(item.Id);
                }

                await _db.SaveChangesAsync();
            }

            return View(orderItems);
        }


        public async Task<IActionResult> Checkout()
		{
			var userId = User.Identity.Name;
			bool paymentResult = await _shoppingCartRepository.Payment(userId);
			return View();
		}
	}
}
