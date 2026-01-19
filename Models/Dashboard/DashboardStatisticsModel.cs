using System.ComponentModel.DataAnnotations;

namespace Hms.WebApp.Models.Dashboard
{
    public class DashboardStatisticsModel
    {
        [Display(Name = "Departments")]
        [Required(ErrorMessage = "Departments is required")]
        public int Departments { get; set; }

        [Display(Name = "Patients Served")]
        [Required(ErrorMessage = "Patients Served is required")]
        public int PatientsServed { get; set; }

        [Display(Name = "Patient Satisfaction (%)")]
        [Required(ErrorMessage = "Patient Satisfaction is required")]
        [Range(0, 100, ErrorMessage = "Patient Satisfaction must be between 0 and 100")]
        public decimal PatientSatisfaction { get; set; }

        [Display(Name = "Years Experience")]
        [Required(ErrorMessage = "Years Experience is required")]
        public int YearsExperience { get; set; }

        // Section 2: Hospital Statistics
        [Display(Name = "Total Doctors")]
        [Required(ErrorMessage = "Total Doctors is required")]
        public int TotalDoctors { get; set; }

        [Display(Name = "Total Patients")]
        [Required(ErrorMessage = "Total Patients is required")]
        public int TotalPatients { get; set; }

        [Display(Name = "Available Beds")]
        [Required(ErrorMessage = "Available Beds is required")]
        public int AvailableBeds { get; set; }

        [Display(Name = "ICU Seats")]
        [Required(ErrorMessage = "ICU Seats is required")]
        public int ICUSeats { get; set; }

        [Display(Name = "Ward Seats")]
        [Required(ErrorMessage = "Ward Seats is required")]
        public int WardSeats { get; set; }

        [Display(Name = "Nurses On Duty")]
        [Required(ErrorMessage = "Nurses On Duty is required")]
        public int NursesOnDuty { get; set; }

        [Display(Name = "Medicine Items")]
        [Required(ErrorMessage = "Medicine Items is required")]
        public int MedicineItems { get; set; }

        [Display(Name = "Ambulances")]
        [Required(ErrorMessage = "Ambulances is required")]
        public int Ambulances { get; set; }

        // Section 3: Chart Data - Department Patient Distribution
        [Display(Name = "Cardiology")]
        public int CardiologyPatients { get; set; }

        [Display(Name = "Neurology")]
        public int NeurologyPatients { get; set; }

        [Display(Name = "Orthopedics")]
        public int OrthopedicsPatients { get; set; }

        [Display(Name = "Pediatrics")]
        public int PediatricsPatients { get; set; }

        [Display(Name = "General")]
        public int GeneralPatients { get; set; }

        // Bed Occupancy Status
        [Display(Name = "Occupied Beds")]
        public int OccupiedBeds { get; set; }

        [Display(Name = "Available Beds")]
        public int AvailableBedsChart { get; set; }

        [Display(Name = "Under Maintenance")]
        public int MaintenanceBeds { get; set; }

        // Monthly Patient Admissions
        [Display(Name = "January")]
        public int JanAdmissions { get; set; }

        [Display(Name = "February")]
        public int FebAdmissions { get; set; }

        [Display(Name = "March")]
        public int MarAdmissions { get; set; }

        [Display(Name = "April")]
        public int AprAdmissions { get; set; }

        [Display(Name = "May")]
        public int MayAdmissions { get; set; }

        [Display(Name = "June")]
        public int JunAdmissions { get; set; }

        // Medicine Stock Distribution
        [Display(Name = "Antibiotics")]
        public int Antibiotics { get; set; }

        [Display(Name = "Pain Relief")]
        public int PainRelief { get; set; }

        [Display(Name = "Vitamins")]
        public int Vitamins { get; set; }

        [Display(Name = "Emergency Drugs")]
        public int EmergencyDrugs { get; set; }

        [Display(Name = "Others")]
        public int OtherMedicines { get; set; }
    }
}
