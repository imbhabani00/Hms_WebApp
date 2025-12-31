using System.ComponentModel.Design;

namespace Hms.WebApp.Models.RolesPermissions
{
    public class PermissionsViewModel
    {
        //Menu Visibility
        public bool CanAccessDashboard { get; set; }
        public bool CanAccessPatients { get; set; }
        public bool CanAccessDoctors { get; set; }
        public bool CanAccessAppointments { get; set; }
        public bool CanAccessReports { get; set; }
        public bool CanAccessDepartments { get; set; }

        //Dashboards
        public bool HasDashboardView { get; set; }

        //Roles & Permissions 
        public bool HasRolesView { get; set; }
        public bool HasRolesAdd { get; set; }
        public bool HasRolesEdit { get; set; }
        public bool HasRolesDelete { get; set; }
        public bool HasRolesPermissions { get; set; }

        //Users
        public bool HasUsersView { get; set; }
        public bool HasUsersAdd { get; set; }
        public bool HasUsersEdit { get; set; }
        public bool HasUsersDelete { get; set; }

        //Patients
        public bool HasPatientsView { get; set; }
        public bool HasPatientsAdd { get; set; }
        public bool HasPatientsEdit { get; set; }
        public bool HasPatientsDelete { get; set; }
    }
}
