$(document).ready(function () {
    debugger
    $('.toggle-password-icon').on('click',function (e) {
        e.preventDefault();
        e.stopPropagation();

        var $input = $('#passwordInput');
        var $icon = $('#toggleIcon');

        if ($input.attr('type') === 'password') {
            $input.attr('type', 'text');
            $icon.removeClass('fa-eye').addClass('fa-eye-slash');
        } else {
            $input.attr('type', 'password');
            $icon.removeClass('fa-eye-slash').addClass('fa-eye');
        }
    });

    $('#Email').on('input', function () {
        $(this).next('.text-danger').text('');
    });

    $('#passwordInput').on('input', function () {
        $(this).next('.text-danger').text('');
    });
});

$('#loginBtn').on('click', function (e) {
    debugger
    e.preventDefault();
    var isValid = true;

    var emailAddress = $('#Email').val().trim();
    var password = $('#passwordInput').val().trim();
    var agreedCheckBox = $('#rememberMeCheckbox').is(':checked');

    $('.text-danger').text('');

    if (!emailAddress) {
        $('#Email').next('.text-danger').text('Email is required');
        isValid = false;
    }

    if (!password) {
        $('#passwordError').text('Password is required');
        isValid = false;
    }

    if (!agreedCheckBox) {
        ShowToasterMessage('error', 'Please agree to Terms & Conditions');
        isValid = false;
    }

    if (!isValid) return;

    var $btn = $('#loginBtn'), $spinner = $btn.find('.spinner-border');
    $btn.prop('disabled', true);
    $spinner.removeClass('d-none');

    var formData = new FormData();
    formData.append('Email', emailAddress);
    formData.append('Password', password);
    formData.append('RememberMe', agreedCheckBox);

    $.ajax({
        url: '/Account/Login',
        type: 'POST',
        data: formData,
        processData: false,
        contentType: false,
        success: function (response) {
            debugger
            if (response.success) {
                if (response.response) {
                    sessionStorage.setItem('tempToken', response.response.token);
                    window.location.href = `/Account/TwoFactorAuth?token=${response.response.token}&email=${response.response.email}`;
                } else {
                    window.location.href = response.redirectUrl;
                }
            } else {
                toastr.error(response.message);
            }
        },
        error: function () {
            toastr.error('Login failed');
        },
        complete: function () {
            $btn.prop('disabled', false);
            $spinner.addClass('d-none');
        }
    });
});