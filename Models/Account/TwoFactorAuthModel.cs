namespace Hms.WebApp.Models.Account
{
    public class TwoFactorAuthModel
    {
        public string? Token { get; set; }
        public string? Email { get; set; }
        public string? AccessCode { get; set; }
        public int TimeValid { get; set; }
    }
}
