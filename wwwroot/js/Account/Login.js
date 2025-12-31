$(document).ready(function () {
    debugger
    $('.toggle-password-icon').click(function (e) {
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

    // Form Submit Handler
    document.getElementById('loginForm').addEventListener('submit', function (e) {
        const loginBtn = document.getElementById('loginBtn');
        const loginSpinner = document.getElementById('loginSpinner');
        const loginText = document.getElementById('loginText');

        loginSpinner.classList.remove('d-none');
        loginText.textContent = 'Signing in...';
        loginBtn.disabled = true;
    });


    $('#loginForm').submit(function (e) {
        e.preventDefault();
        var $btn = $('#loginBtn'), $spinner = $btn.find('.spinner-border');

        $btn.prop('disabled', true);
        $spinner.removeClass('d-none');

        $.ajax({
            url: 'Account/Login',
            type: 'POST',
            data: $(this).serialize(),
            success: function (result) {
                if (result.success) {
                    if (result.response) {
                        sessionStorage.setItem('tempToken', result.response.token);
                        window.location.href = `/Account/TwoFactorAuth?token=${result.response.token}&email=${result.response.email}`;
                    } else {
                        window.location.href = result.redirectUrl;
                    }
                } else {
                    toastr.error(result.message);
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
});