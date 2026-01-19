function saveDashboardData() {
    // Collect all form data
    const dashboardData = {
        // Hero Stats
        yearsExperience: document.getElementById('yearsExperience').value,
        patientSatisfaction: document.getElementById('patientSatisfaction').value,
        patientsServed: document.getElementById('patientsServed').value,
        departments: document.getElementById('departments').value,

        // Statistics Overview
        totalDoctors: document.getElementById('totalDoctors').value,
        totalPatients: document.getElementById('totalPatients').value,
        availableBeds: document.getElementById('availableBeds').value,
        icuSeats: document.getElementById('icuSeats').value,
        wardSeats: document.getElementById('wardSeats').value,
        nursesOnDuty: document.getElementById('nursesOnDuty').value,
        medicineItems: document.getElementById('medicineItems').value,
        ambulances: document.getElementById('ambulances').value,

        // Department Patient Distribution
        cardiologyPatients: document.getElementById('cardiologyPatients').value,
        neurologyPatients: document.getElementById('neurologyPatients').value,
        orthopedicsPatients: document.getElementById('orthopedicsPatients').value,
        pediatricsPatients: document.getElementById('pediatricsPatients').value,
        generalPatients: document.getElementById('generalPatients').value,

        // Bed Occupancy
        occupiedBeds: document.getElementById('occupiedBeds').value,
        availableBedsChart: document.getElementById('availableBedsChart').value,
        maintenanceBeds: document.getElementById('maintenanceBeds').value,

        // Monthly Admissions
        janAdmissions: document.getElementById('janAdmissions').value,
        febAdmissions: document.getElementById('febAdmissions').value,
        marAdmissions: document.getElementById('marAdmissions').value,
        aprAdmissions: document.getElementById('aprAdmissions').value,
        mayAdmissions: document.getElementById('mayAdmissions').value,
        junAdmissions: document.getElementById('junAdmissions').value,

        // Medicine Stock
        antibiotics: document.getElementById('antibiotics').value,
        painRelief: document.getElementById('painRelief').value,
        vitamins: document.getElementById('vitamins').value,
        emergencyDrugs: document.getElementById('emergencyDrugs').value,
        otherMedicines: document.getElementById('otherMedicines').value
    };

    // Validate required fields
    if (!dashboardData.totalDoctors || !dashboardData.totalPatients) {
        alert('Please fill in all required fields');
        return;
    }

    // Send to backend
    fetch('/Dashboard/SaveDashboardData', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify(dashboardData)
    })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                alert('Dashboard data saved successfully!');
                window.location.href = '/Dashboard/Index';
            } else {
                alert('Error saving data: ' + data.message);
            }
        })
        .catch(error => {
            console.error('Error:', error);
            alert('An error occurred while saving data');
        });
}

function resetForm() {
    if (confirm('Are you sure you want to reset all fields?')) {
        document.querySelectorAll('input[type="number"]').forEach(input => {
            input.value = '';
        });
    }
}


