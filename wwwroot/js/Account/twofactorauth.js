$(document).ready(function () {

    // Auto-focus and numeric only
    $('#accessCode').on('input', function () {
        this.value = this.value.replace(/[^0-9]/g, '');
        $('#accessCodeError').text('');
    });

    // Auto-submit when 6 digits entered
    $('#accessCode').on('input', function () {
        if ($(this).val().length === 6) {
            $('#verifyBtn').click();
        }
    });

    // Verify button click
    $('#twoFactorForm').on('submit', function (e) {
        e.preventDefault();

        var accessCode = $('#accessCode').val().trim();
        var token = $('#token').val();

        if (!accessCode || accessCode.length !== 6) {
            $('#accessCodeError').text('Please enter 6-digit code');
            return;
        }

        var $btn = $('#verifyBtn');
        var $spinner = $btn.find('.spinner-border');
        $btn.prop('disabled', true);
        $spinner.removeClass('d-none');
        $btn.text('Verifying...');

        $.ajax({
            url: '/Account/ValidateAccessCode',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify({
                Token: token,
                AccessCode: accessCode
            }),
            success: function (result) {
                if (result.success) {
                    ShowToasterMessage('success', 'Login successful!', 'Success');
                    setTimeout(function () {
                        window.location.href = result.redirectUrl;
                    }, 1000);
                } else {
                    ShowToasterMessage('error', result.message || 'Invalid access code', 'Error');
                    $('#accessCode').val('').focus();
                }
            },
            error: function () {
                ShowToasterMessage('error', 'Verification failed. Please try again.', 'Error');
            },
            complete: function () {
                $btn.prop('disabled', false);
                $spinner.addClass('d-none');
                $btn.text('Verify Code');
            }
        });
    });

    // Resend button click
    $('#resendBtn').on('click', function () {
        var token = $('#token').val();
        var $btn = $(this);
        $btn.prop('disabled', true);

        $.ajax({
            url: '/Account/ResendAccessCode',
            type: 'POST',
            data: { token: token },
            success: function (result) {
                if (result.success) {
                    ShowToasterMessage('success', 'Access code resent successfully!', 'Success');
                    $('#accessCode').val('').focus();
                } else {
                    ShowToasterMessage('error', result.message || 'Failed to resend code', 'Error');
                }
            },
            error: function () {
                ShowToasterMessage('error', 'Failed to resend code', 'Error');
            },
            complete: function () {
                $btn.prop('disabled', false);
            }
        });
    });
});