namespace HomeBuild.ViewModel.AccountsVM
{
	public class AllAccountsVM
	{
		public string Id { get; set; }
		public string UserName { get; set; }
		public string Email { get; set; }
		public string FullName { get; set; }
		public IList<string> Roles { get; set; }
	}
}
