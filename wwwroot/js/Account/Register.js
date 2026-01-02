$(document).ready(function () {
    $('#toggleIconPassword').on('click', function () {
        var passwordInput = $('#passwordInput');
        if (passwordInput.attr('type') === 'password') {
            passwordInput.attr('type', 'text');
            $(this).removeClass('fa-eye').addClass('fa-eye-slash');
        } else {
            passwordInput.attr('type', 'password');
            $(this).removeClass('fa-eye-slash').addClass('fa-eye');
        }
    });

    $('#toggleIconConfirm').on('click', function () {
        var confirmPasswordInput = $('#confirmPasswordInput');
        if (confirmPasswordInput.attr('type') === 'password') {
            confirmPasswordInput.attr('type', 'text');
            $(this).removeClass('fa-eye').addClass('fa-eye-slash');
        } else {
            confirmPasswordInput.attr('type', 'password');
            $(this).removeClass('fa-eye-slash').addClass('fa-eye');
        }
    });

    $('#FirstName').on('input', function () {
        $(this).next('.text-danger').text('');
    });

    $('#LastName').on('input', function () {
        $(this).next('.text-danger').text('');
    });

    $('#Email').on('input', function () {
        $(this).next('.text-danger').text('');
    });

    $('#passwordInput').on('input', function () {
        $('#passwordError').text('');
    });

    $('#confirmPasswordInput').on('input', function () {
        $('#confirmError').text('');
    });
});

function validatePassword() {
    var password = $('#passwordInput').val();
    var confirmPassword = $('#confirmPasswordInput').val();

    $('#passwordError').text('');
    $('#confirmError').text('');

    var isValid = true;

    if (!password) {
        $('#passwordError').text('Password is required');
        isValid = false;
    } else if (password.length < 8) {
        $('#passwordError').text('Password must be at least 8 characters');
        isValid = false;
    }

    if (!confirmPassword) {
        $('#confirmError').text('Confirm password is required');
        isValid = false;
    } else if (password !== confirmPassword) {
        $('#confirmError').text('Passwords do not match');
        isValid = false;
    }

    return isValid;
}

function isValidEmail(email) {
    var emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return emailRegex.test(email);
}

$('#registerBtn').on('click', function (e) {
    debugger
    e.preventDefault();
    var firstName = $('#FirstName').val().trim();
    var lastName = $('#LastName').val().trim();
    var email = $('#Email').val().trim();
    var agreeCheckbox = $('#agreeCheckbox').is(':checked');

    $('.text-danger').text('');
    var isValid = true;

    if (!firstName) {
        $('#FirstName').next('.text-danger').text('First name is required');
        isValid = false;
    }

    if (!lastName) {
        $('#LastName').next('.text-danger').text('Last name is required');
        isValid = false;
    }

    if (!email) {
        $('#Email').next('.text-danger').text('Email is required');
        isValid = false;
    } else if (!isValidEmail(email)) {
        $('#Email').next('.text-danger').text('Invalid email format');
        isValid = false;
    }

    if (!validatePassword()) {
        isValid = false;
    }

    if (!agreeCheckbox) {
        ShowToasterMessage('error', 'Please agree to Terms & Conditions');
        isValid = false;
    }

    if (!isValid) return;

    var $btn = $('#registerBtn');
    var $spinner = $btn.find('.spinner-border');
    $btn.prop('disabled', true);
    $spinner.removeClass('d-none');
    $btn.html('<span class="spinner-border spinner-border-sm me-2"></span>Creating Account...');

    var formData = new FormData();
    formData.append('FirstName', firstName);
    formData.append('LastName', lastName);
    formData.append('Email', email);
    formData.append('Password', $('#passwordInput').val());
    formData.append('ConfirmPassword', $('#confirmPasswordInput').val());
    formData.append('AgreeToTerms', agreeCheckbox);

    $.ajax({
        url: '/Account/Register',
        type: 'POST',
        data: formData,
        processData: false,
        contentType: false,
        success: function (response) {
            debugger
            if (response.statusCode === 200) {
                ShowToasterMessage('success', 'Registration successful! Please check your email.', 'Success');
                setTimeout(function () {
                    window.location.href = '/Account/Login';
                }, 2000);
            } else {
                debugger
                ShowToasterMessage('error', response.errorMessage || 'Registration failed', 'Error');
            }
        },
        error: function (xhr) {
            debugger
            var errorMsg = 'Registration failed. Please try again.';
            if (xhr.responseJSON && xhr.responseJSON.message) {
                errorMsg = xhr.responseJSON.message;
            }
            ShowToasterMessage('error', errorMsg, 'Error');
        },
        complete: function () {
            $btn.prop('disabled', false);
            $spinner.addClass('d-none');
            $btn.html('Create Account');
        }
    });
});