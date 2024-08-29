using Azure;
using HomeBuild.Data;
using HomeBuild.Models;
using HomeBuild.ViewModel.ShoppingCartVM;
using HomeBuild.ViewModel.StocksVM;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using System.Drawing;

namespace HomeBuild.Repositories
{
	public class ShoppingCartRepository
	{
		private readonly ApplicationDbContext _db;

		public ShoppingCartRepository(ApplicationDbContext db)
		{
			_db = db;
		}

		// Вземане на всички артикули в количката на даден потребител
		public async Task<List<ShoppingCart>> ShoppingCartItems(string userId)
		{
			return await _db.ShoppingCarts.Include(x => x.User).Where(x => x.UserId == userId).ToListAsync();
		}

		// Добавяне на артикул в количката
		public async Task AddToCart(int productId, string color, int quantity, string userId, string size)
		{
			// Проверка дали артикулът вече съществува в количката на потребителя
			var shoppingCartItem = await _db.ShoppingCarts
			.SingleOrDefaultAsync(s => s.ProductId == productId && s.UserId == userId);

			if (shoppingCartItem != null && shoppingCartItem.Color == color && shoppingCartItem.Size == size)
			{
				// Ако съществува, увеличаваме количеството на артикула в количката
				shoppingCartItem.Quantity += quantity;
				shoppingCartItem.Price += shoppingCartItem.Price;
				await _db.SaveChangesAsync();
				return;
			}
			
			// Иначе, артикулът все още не е добавен в количката и го добавяме
			var stock = await _db.Stocks.FindAsync(productId);
			shoppingCartItem = new ShoppingCart
			{
                ProductId = productId,
                UserId = userId,
				ProductName = stock.Product,
				Price = stock.Price,
				Quantity = quantity,
				Color = color,
				Size = size,
				UrlImg = stock.UrlImg
			};
			_db.ShoppingCarts.Add(shoppingCartItem);
			await _db.SaveChangesAsync();
		}
		// Премахване на артикул от количката
		public async Task RemoveFromCart(int id)
		{
			var shoppingCartItem = await _db.ShoppingCarts.FindAsync(id);
			if (shoppingCartItem != null)
			{
				_db.ShoppingCarts.Remove(shoppingCartItem);
				await _db.SaveChangesAsync();
			}
		}
		// Изчистване на цялата количка на потребителя
		public async Task ClearShoppingCart(string userId)
		{
			var cartItems = await _db.ShoppingCarts.Where(c => c.UserId == userId).ToListAsync();
			_db.ShoppingCarts.RemoveRange(cartItems);
			await _db.SaveChangesAsync();
		}
		// Промяна на количеството на даден артикул в количката
		public async Task UpdateQuantity(int id, int quantity)
		{
			var cartItem = await _db.ShoppingCarts.FindAsync(id);
			if (cartItem != null)
			{
				cartItem.Quantity = quantity;
				await _db.SaveChangesAsync();
			}
		}
		//Метод за обновяване на продуктите в количката
		public async Task UpdateCartItems(Stocks stock)
		{
			//Намиране на продукт по името му
			var cartItems = await _db.ShoppingCarts.Where(x => x.ProductName == stock.Product).ToListAsync();

			foreach (var cartItem in cartItems)
			{
				cartItem.ProductName = stock.Product;
				cartItem.Price = stock.Price;
				cartItem.UrlImg = stock.UrlImg;
			}
			await _db.SaveChangesAsync();
		}

		//Метод за вземане на информацията за количката на потребителя:
		public async Task<ShoppingCartCRUD> UserShoppingCart(string userId)
		{
			//взимане на всички преемти от количката на потребителят
			var items = await ShoppingCartItems(userId);

			//групиране по име,цвят и размер
			var productGroups = items.GroupBy(i => new { i.ProductName, i.Color, i.Size })
				.ToDictionary(g => $"{g.Key.ProductName} ({g.Key.Color}) ({g.Key.Size})", g => g.Sum(i => i.Price * i.Quantity));

			var totalPrice = items.Sum(i => i.Price * i.Quantity);

			var cartItems = new ShoppingCartCRUD
			{
				ShoppingCarts = items.Select(x => new ShoppingCart
				{
					Id = x.Id,
					ProductName = x.ProductName,
					Quantity = x.Quantity,
					Price = x.Price,
					UrlImg = x.UrlImg,
					Color = x.Color,
					Size = x.Size,
				}).ToList(),
				PriceTotal = totalPrice,
				ProductTotal = productGroups
			};
			return cartItems;
		}


		//Метод за вземане на историята на поръчките на потребителя:
		public async Task<List<ShoppingCartHistory>> OrderHistory(string userId, bool payment)
		{
			// Взема всички продукти от кошницата на потребителя.
			var cartItems = await _db.ShoppingCarts.Where(c => c.UserId == userId).ToListAsync();
			var orderHistory = new List<ShoppingCartHistory>();
			var cartCRUD = await UserShoppingCart(userId);
			decimal totalPrice = cartCRUD.PriceTotal;

            foreach (var item in cartItems)
			{
				var orderItem = new ShoppingCartHistory
				{
					ProductId = item.ProductId,
					ProductName = item.ProductName,
					Color = item.Color,
					Size = item.Size,
					Price = item.Price,
					Quantity = item.Quantity,
					UserId = userId,
					Payed = false,
					Usernamne = item.User.FullName,
					PurchaseDate = DateTime.UtcNow,
					ProductTotal = item.Quantity * item.Price,
					PriceTotal = totalPrice
				};

				orderHistory.Add(orderItem);

				// Намалява количеството на продукта в склада.
				var stock = await _db.Stocks.FindAsync(item.ProductId);
				if (stock != null)
				{
					if (stock.Quantity >= item.Quantity)
					{
						stock.Quantity -= item.Quantity;
						_db.SaveChanges();
					}
					else
					{
						throw new Exception($"Нямаме толкова налични '{item.ProductName}'. Останали са ни само {stock.Quantity} Извиняваме се!");
					}
				}
				else
				{
					throw new Exception($"В момента нямаме налична");
				}

                if (payment == true)
                {
                    foreach (var orderItemsas in orderHistory)
                    {
                        orderItemsas.Id = 0; 
                    }
                    await _db.ShoppingCartHistories.AddRangeAsync(orderHistory);
                    _db.ShoppingCarts.Remove(item);
                }
            }

            await _db.SaveChangesAsync();

			// Връща историята на поръчките на потребителите.
			var orderItems = await _db.ShoppingCartHistories
			.Where(c => c.UserId == userId)
			.ToListAsync();

			return orderItems;
		}


		public async Task<bool> Payment(string userId)
		{
			var cartItems = await _db.ShoppingCarts.Where(c => c.UserId == userId).ToListAsync();
			decimal totalPrice = 0;
			foreach (var item in cartItems)
			{
				totalPrice += item.Quantity * item.Price;
			}

			if (totalPrice > 0)
			{ 
				return true; 
			}
			else 
			{ 
				return false; 
			}
		}
	}
}
