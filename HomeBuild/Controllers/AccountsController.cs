using HomeBuild.Data;
using HomeBuild.Models;
using HomeBuild.ViewModel.AccountsVM;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HomeBuild.Controllers
{
	public class AccountsController : Controller
	{
		// Инжектиране на зависимости от ApplicationDbContext, UserManager, SignInManager и RoleManager
		private readonly ApplicationDbContext _db;
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly SignInManager<ApplicationUser> _signInManager;
		private readonly RoleManager<IdentityRole> _roleManager;

		public AccountsController(ApplicationDbContext db, UserManager<ApplicationUser> userManager,
		SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager)
		{
			_db = db;
			_userManager = userManager;
			_roleManager = roleManager;
			_signInManager = signInManager;
		}

		public IActionResult Index()
		{
			return View();
		}

		// GET action метод за показване на формата за регистрация
		[HttpGet]
        public async Task<IActionResult> Register()
		{
			// Проверка дали ролите в системата съществуват и ако не, ги създава
			if (!_roleManager.RoleExistsAsync(Separator.Admin).GetAwaiter().GetResult())
			{
				await _roleManager.CreateAsync(new IdentityRole(Separator.Admin));
				await _roleManager.CreateAsync(new IdentityRole(Separator.Users));
			}
			return View();
		}

		// Използване на асинхронен метод за регистрация на потребител в системата
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Register(RegisterVM model)
		{
			if (ModelState.IsValid)
			{
				var userExists = await _userManager.FindByNameAsync(model.Username);
				var emailExist = await _userManager.FindByNameAsync(model.Email);
				if (userExists != null || emailExist != null)
				{
					TempData["errEmail"] = "Имейла е вече използван!";
					TempData["email"] = "email";
					TempData["errUser"] = "Потребителското име е вече използвано!";
					TempData["user"] = "user";
				}
				else
				{
					// Създаване на нов обект ApplicationUser и задаване на стойности за неговите свойства
					var user = new ApplicationUser
					{
						UserName = model.Username,
						Email = model.Email,
						FullName = model.Username + "," + model.Email
					};
					// Използване на асинхронен метод за създаване на потребител в базата данни
					var result = await _userManager.CreateAsync(user, model.Password);
					if (result.Succeeded)
					{
						// Добавяне на роля
						await _userManager.AddToRoleAsync(user, Separator.Users);
						await _signInManager.SignInAsync(user, isPersistent: false);
						return RedirectToAction("Index", "Home");
					}
				}
			}
			return View(model);
		}


		// POST и GET action метод за влизане в системата
		[HttpPost, HttpGet]
         public async Task<IActionResult> LogIn(LogInVM model)
		{
			if (!ModelState.IsValid)
				return View(model);

			// Търсене на потребителя по имейл
			var user = await _userManager.FindByEmailAsync(model.Email);

			if (user != null)
			{

				// Проверка на паролата на потребителя
				var password = await _userManager.CheckPasswordAsync(user, model.Password);

				if (password)
				{
					// Влизане в системата на потребителя с паролата му
					var final = await _signInManager.PasswordSignInAsync(user, model.Password, false, false);

					if (final.Succeeded)
					{
						return RedirectToAction("Index", "Home");
					}
				}
				else
				{
					TempData["errPass"] = "Грешна парола!";
					TempData["pass"] = "pass";
					return View(model);
				}
			}

			TempData["errEmail"] = "Няма такъв имейл!";
			TempData["email"] = "email";

			return View(model);
		 }

		// Изход от системата
		[HttpPost, HttpGet]
		public async Task<IActionResult> LogOff()
		{
			// Използване на асинхронен метод на _signInManager за излизане от системата
			await _signInManager.SignOutAsync();

			return RedirectToAction("Index", "Home");
		}

		// Изтриване на потребител след като го намери по id
		public async Task<IActionResult> Delete(string id)
		{
			var user = await _userManager.FindByIdAsync(id);
			await _userManager.DeleteAsync(user);

			return RedirectToAction("AccountsTable", "Accounts");
		}

		// Извличане на всички потребители
		[HttpGet]
		public async Task<IActionResult> AccountsTable()
		{
			// Извличане на всички потребители
			var users = await _db.Users.ToListAsync();

			// Списък от потребители
			var allAccountsVMs = new List<AllAccountsVM>();

			foreach (var user in users)
			{
				var roles = await _userManager.GetRolesAsync(user);

				var accountVM = new AllAccountsVM
				{
					Id = user.Id,
					UserName = user.UserName,
					Email = user.Email,
					FullName = user.FullName,
					Roles = roles.ToList()
				};

				allAccountsVMs.Add(accountVM);
			}

			return View(allAccountsVMs);
		}
	}
}