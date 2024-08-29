using System.ComponentModel.DataAnnotations;

namespace HomeBuild.ViewModel.AccountsVM
{
	public class LogInVM
	{
        [StringLength(35, MinimumLength = 7, ErrorMessage = "Трябва да е между 7-35 симвула")]
		public string Email { get; set; }

        [StringLength(40, MinimumLength = 3, ErrorMessage = "Трябва да е между 3-40 симвула")]
		public string Password { get; set; }

	}
}
