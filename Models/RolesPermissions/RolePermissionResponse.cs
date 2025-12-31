namespace Hms.WebApp.Models.RolesPermissions
{
    public class RolePermission
    {
        public string Code { get; set; } = string.Empty;
        public bool HasPermission { get; set; }
    }
    public class RolePermissionResponse
    {
        public List<RolePermission>? RolePermissions { get; set; }
    }
}
