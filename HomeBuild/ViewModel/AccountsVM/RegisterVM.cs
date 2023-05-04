using System.ComponentModel.DataAnnotations;

namespace HomeBuild.ViewModel.AccountsVM
{
	public class RegisterVM
	{
		[StringLength(25, MinimumLength = 4, ErrorMessage = "Трябва да е между 4-25 симвула")]
		public string Username { get; set; }

		[StringLength(35, MinimumLength = 6, ErrorMessage = "Трябва да е между 6-35 симвула")]
		public string Email { get; set; }

		[StringLength(40, MinimumLength = 3, ErrorMessage = "Трябва да е между 3-40 симвула")]
		public string Password { get; set; }
	}
}
