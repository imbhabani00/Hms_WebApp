using System.ComponentModel;

namespace Hms.WebApp.Constants
{
    public static class RoleConstants
    {
        [Description("Admin")]
        public const int ADM = 1;

        [Description("Doctor")]
        public const int DOC = 2;

        [Description("Nurse")]
        public const int NUR = 3;

        [Description("Patient")]
        public const int PAT = 4;

        [Description("Ceo")]
        public const int CEO = 5;

        [Description("Receptionist")]
        public const int REC = 6;

        [Description("Pharmacist")]
        public const int PHA = 7;

        [Description("Lab Technician")]
        public const int LAB = 8;
    }

    public static class RoleNames
    {
        public const string Admin = "ADMIN";
        public const string Supervisor = "SUPERVISOR";
        public const string User = "USER";
        public const string Doctor = "DOCTOR";
        public const string Nurse = "NURSE";
        public const string Patient = "PATIENT";
    }
}
