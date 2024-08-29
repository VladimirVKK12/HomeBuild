using HomeBuild.Data;
using HomeBuild.Models;
using HomeBuild.ViewModel.StocksVM;
using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeBuild.Repositories
{
	public class StocksRepository
	{
		private readonly ApplicationDbContext _db;

		public StocksRepository(ApplicationDbContext context)
		{
			_db = context;
		}

		// метод за връщане на всички артикули
		public async Task<IEnumerable<Stocks>> GetAll()
		{
			return await _db.Stocks.ToListAsync();
		}

		// метод за връщане на акция по  идентификатор
		public async Task<Stocks> GetById(int id)
		{
			return await _db.Stocks.FirstOrDefaultAsync(s => s.Id == id);
		}

		//метод за създаване на артикул
		public async Task Create(StocksCRUD entity)
		{
			var fileName = Photo(entity);
			var stocks = new Stocks
			{
				Type = entity.Type,
				Product = entity.Product,
				Quantity = entity.Quantity,
				Price = entity.Price,
				Color = string.Join(",", entity.Color),
				Size = string.Join(",", entity.Size),
				UrlImg = fileName,
			};
			_db.Stocks.Add(stocks);
			await _db.SaveChangesAsync();
		}

		public string Photo(StocksCRUD entity)
		{
			// Проверка дали има качена снимка
			if (entity.UrlImage != null)
			{
				// Генериране на уникално име на файла, който ще бъде запазен в wwwroot/My_Images
				var fileName = Path.GetFileNameWithoutExtension(entity.UrlImage.FileName);
				var extension = Path.GetExtension(entity.UrlImage.FileName);
				var uniqueFileName = $"{fileName}_{Guid.NewGuid().ToString()}{extension}";
				var filePath = Path.Combine("wwwroot/My_Images", uniqueFileName);

				// Запазване на качената снимка във файла
				using (var fileStream = new FileStream(filePath, FileMode.Create))
				{
					entity.UrlImage.CopyTo(fileStream);
				}

				// Връщане на пътя до запазената снимка
				return $"~/My_Images/{uniqueFileName}";
			}

			return null;
		}

		public async Task Update(StocksCRUD entity)
		{
			// Генериране на име на файла за снимката
			var fileName = Photo(entity);

			// Намиране на стоката в базата данни
			var stocks = await _db.Stocks.FirstOrDefaultAsync(s => s.Id == entity.Id);

			string container;

			if (stocks != null)
			{
				stocks.Product = entity.Product;
				stocks.Type = entity.Type;
				stocks.Quantity = entity.Quantity;
				stocks.Price = entity.Price;
				stocks.Color = entity.Color;
				stocks.Size = entity.Size;

				if (entity.UrlImage == null)
				{
					entity.UrlImage = new FormFile(new MemoryStream(Encoding.UTF8.GetBytes(stocks.UrlImg)),
					0, stocks.UrlImg.Length, "UrlImage", stocks.UrlImg);
				}
				else
				{
					stocks.UrlImg = fileName;
				}
			}

			await _db.SaveChangesAsync();
		}

		//метод за намиране на стоката в базата данни по id и да се премахне
		public async Task Delete(int id)
		{
			var stocks = await _db.Stocks.FindAsync(id);
			_db.Stocks.Remove(stocks);
			await _db.SaveChangesAsync();
		}

		public async Task<List<Stocks>> StocksSearch(string searchWord)
		{
			// Създаване на празен списък, в който да се запишат стоките, които отговарят на търсенето
			List<Stocks> searched = new List<Stocks>();

			if (searchWord != null)
			{
				// Извличане на всички стоки от базата данни
				IQueryable<Stocks> query = _db.Stocks;

				foreach (var item in query)
				{
					string name = string.Concat(item.Type, " ", item.Product).ToLower();
					if (name.Contains(searchWord.ToLower()))
					{
						searched.Add(item);
					}
				}
			}
			return searched;
		}
	}
}
