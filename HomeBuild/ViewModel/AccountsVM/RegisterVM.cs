using System.ComponentModel.DataAnnotations;

namespace HomeBuild.ViewModel.AccountsVM
{
	public class RegisterVM
	{
		[StringLength(25, MinimumLength = 4, ErrorMessage = "Трябва да е между 3-25 симвула")]
		public string Username { get; set; }

        [StringLength(35, MinimumLength = 6, ErrorMessage = "Трябва да е между 6-35 симвула")]
		public string Email { get; set; }

        [StringLength(40, MinimumLength = 3, ErrorMessage = "Трябва е междъ 3-40 симвула и да съдържа една главан и малка буква специален знак и число")]
		public string Password { get; set; }
	}
}
