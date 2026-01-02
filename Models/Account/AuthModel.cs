namespace Hms.WebApp.Models.Account
{
    public class AuthModel
    {
        public string? Email { get; set; }
        public string? Password { get; set; }
        public int TenantId { get; set; }
        public string? ReturnUrl { get; set; }
        public string? RememberMe { get; set; }
    }

    public class AuthResponseModel
    {
        public string? Token { get; set; }
        public string? RefreshToken { get; set; }
        public string? Email { get; set; }
        public DateTime? Expires { get; set; }
        public int? UserId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public int? RoleId { get; set; }
        public string? RoleName { get; set; }
        public int? TenantId { get; set; }
        public int StatusCode { get; set; }
        public string? Message { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
