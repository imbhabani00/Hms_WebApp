namespace Hms.WebApp.Models.Account
{
    public class AuthModel
    {
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? ReturnUrl { get; set; }
        public string? Name { get; set; }
        public string? RememberMe { get; set; }
    }

    public class AuthResponseModel
    {
        public AuthData Data { get; set; } = new();
        public int StatusCode { get; set; }
        public string? Message { get; set; }
        public string? ErrorMessage { get; set; }
    }

    public class AuthData
    {
        public string? Token { get; set; } = "";
        public string RefreshToken { get; set; } = "";
        public string Email { get; set; } = "";
        public DateTime Expires { get; set; }
        public string? AccessCode { get; set; }
        public int TimeValid { get; set; }
    }
}
