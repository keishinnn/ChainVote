// Function to toggle password visibility
function togglePasswordVisibility(inputId, buttonId) {
    const passwordInput = document.getElementById(inputId);
    const toggleButton = document.getElementById(buttonId);
    const type = passwordInput.type === 'password' ? 'text' : 'password';
    passwordInput.type = type;

    // Toggle icon
    if (type === 'password') {
        toggleButton.innerHTML = '<i class="bi bi-eye-slash"></i>';
    } else {
        toggleButton.innerHTML = '<i class="bi bi-eye"></i>';
    }
}

// Add event listeners for toggle buttons
document.addEventListener('DOMContentLoaded', function () {
    document.getElementById('toggleCurrentPassword').addEventListener('click', function () {
        togglePasswordVisibility('currentPassword', 'toggleCurrentPassword');
    });

    document.getElementById('toggleNewPassword').addEventListener('click', function () {
        togglePasswordVisibility('newPassword', 'toggleNewPassword');
    });

    document.getElementById('toggleConfirmPassword').addEventListener('click', function () {
        togglePasswordVisibility('confirmPassword', 'toggleConfirmPassword');
    });

    document.getElementById('toggleLoginPassword').addEventListener('click', function () {
        togglePasswordVisibility('loginPassword', 'toggleLoginPassword');
    });
});