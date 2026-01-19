const primaryColor = '#667eea';
const secondaryColor = '#764ba2';
const purpleAccent = '#6f42c1';

const chartColors = [
    'rgba(102, 126, 234, 0.8)',
    'rgba(118, 75, 162, 0.8)',
    'rgba(111, 66, 193, 0.8)',
    'rgba(102, 126, 234, 0.6)',
    'rgba(118, 75, 162, 0.6)'
];

// Bar Chart - Patient Distribution
const patientCtx = document.getElementById('patientChart').getContext('2d');
new Chart(patientCtx, {
    type: 'bar',
    data: {
        labels: ['Cardiology', 'Neurology', 'Orthopedics', 'Pediatrics', 'General'],
        datasets: [{
            label: 'Number of Patients',
            data: [65, 59, 80, 48, 72],
            backgroundColor: chartColors,
            borderColor: purpleAccent,
            borderWidth: 2,
            borderRadius: 10
        }]
    },
    options: {
        responsive: true,
        maintainAspectRatio: true,
        plugins: {
            legend: { display: false }
        },
        scales: {
            y: {
                beginAtZero: true,
                grid: { color: 'rgba(0,0,0,0.05)' }
            },
            x: {
                grid: { display: false }
            }
        }
    }
});

// Pie Chart - Bed Occupancy
const bedCtx = document.getElementById('bedChart').getContext('2d');
new Chart(bedCtx, {
    type: 'doughnut',
    data: {
        labels: ['Occupied', 'Available', 'Under Maintenance'],
        datasets: [{
            data: [168, 156, 26],
            backgroundColor: chartColors.slice(0, 3),
            borderColor: '#fff',
            borderWidth: 4
        }]
    },
    options: {
        responsive: true,
        maintainAspectRatio: true,
        plugins: {
            legend: {
                position: 'bottom',
                labels: { padding: 20, font: { size: 13 } }
            }
        }
    }
});

// Line Chart - Monthly Admissions
const admissionCtx = document.getElementById('admissionChart').getContext('2d');
new Chart(admissionCtx, {
    type: 'line',
    data: {
        labels: ['January', 'February', 'March', 'April', 'May', 'June'],
        datasets: [{
            label: 'Admissions',
            data: [250, 290, 310, 280, 340, 324],
            borderColor: purpleAccent,
            backgroundColor: 'rgba(111, 66, 193, 0.1)',
            tension: 0.4,
            fill: true,
            pointBackgroundColor: purpleAccent,
            pointBorderColor: '#fff',
            pointBorderWidth: 3,
            pointRadius: 6,
            pointHoverRadius: 8
        }]
    },
    options: {
        responsive: true,
        maintainAspectRatio: true,
        plugins: {
            legend: { display: false }
        },
        scales: {
            y: {
                beginAtZero: true,
                grid: { color: 'rgba(0,0,0,0.05)' }
            },
            x: {
                grid: { display: false }
            }
        }
    }
});

// Polar Area Chart - Medicine Stock
const medicineCtx = document.getElementById('medicineChart').getContext('2d');
new Chart(medicineCtx, {
    type: 'polarArea',
    data: {
        labels: ['Antibiotics', 'Pain Relief', 'Vitamins', 'Emergency Drugs', 'Others'],
        datasets: [{
            data: [320, 280, 195, 250, 200],
            backgroundColor: chartColors,
            borderColor: '#fff',
            borderWidth: 3
        }]
    },
    options: {
        responsive: true,
        maintainAspectRatio: true,
        plugins: {
            legend: {
                position: 'bottom',
                labels: { padding: 20, font: { size: 13 } }
            }
        }
    }
});