namespace Hms.WebApp.Models.RolesPermissions
{
    public class RolePermission
    {
        public string Code { get; set; } = string.Empty;
        public int ModuleId { get; set; }
        public string ModuleCode { get; set; }
        public string ModuleName { get; set; }
        public string ModuleIcon { get; set; }
        public string ModuleRoute { get; set; }
        public int DisplayOrder { get; set; }
        public int PermissionId { get; set; }
        public string PermissionCode { get; set; }
        public string PermissionName { get; set; }
        public string PermissionType { get; set; }
        public bool HasPermission { get; set; }
    }
    public class RolePermissionResponse
    {
        public List<RolePermission>? RolePermissions { get; set; }
    }
}
