const newPasswordInput = document.getElementById('newPassword');
const confirmPasswordInput = document.getElementById('confirmPassword');
const lengthCheck = document.getElementById('lengthCheck');
const upperCheck = document.getElementById('upperCheck');
const lowerCheck = document.getElementById('lowerCheck');
const numberCheck = document.getElementById('numberCheck');
const passwordMatch = document.getElementById('passwordMatch');

function updateStrengthIndicators(password) {
    // Length Check
    if (password.length >= 8) {
        lengthCheck.classList.replace('text-danger', 'text-success');
        lengthCheck.textContent = '✅ At least 8 characters';
    } else {
        lengthCheck.classList.replace('text-success', 'text-danger');
        lengthCheck.textContent = '❌ At least 8 characters';
    }

    // Uppercase Check
    if (/[A-Z]/.test(password)) {
        upperCheck.classList.replace('text-danger', 'text-success');
        upperCheck.textContent = '✅ At least one uppercase letter';
    } else {
        upperCheck.classList.replace('text-success', 'text-danger');
        upperCheck.textContent = '❌ At least one uppercase letter';
    }

    // Lowercase Check
    if (/[a-z]/.test(password)) {
        lowerCheck.classList.replace('text-danger', 'text-success');
        lowerCheck.textContent = '✅ At least one lowercase letter';
    } else {
        lowerCheck.classList.replace('text-success', 'text-danger');
        lowerCheck.textContent = '❌ At least one lowercase letter';
    }

    // Number Check
    if (/\d/.test(password)) {
        numberCheck.classList.replace('text-danger', 'text-success');
        numberCheck.textContent = '✅ At least one number';
    } else {
        numberCheck.classList.replace('text-success', 'text-danger');
        numberCheck.textContent = '❌ At least one number';
    }
}

function checkPasswordMatch() {
    if (newPasswordInput.value && confirmPasswordInput.value &&
        newPasswordInput.value === confirmPasswordInput.value) {
        passwordMatch.classList.replace('text-danger', 'text-success');
        passwordMatch.textContent = '✅ Passwords match';
    } else {
        passwordMatch.classList.replace('text-success', 'text-danger');
        passwordMatch.textContent = '❌ Passwords do not match';
    }
}

// Attach event listeners
newPasswordInput.addEventListener('input', function () {
    updateStrengthIndicators(newPasswordInput.value);
    checkPasswordMatch();
});

confirmPasswordInput.addEventListener('input', checkPasswordMatch);
