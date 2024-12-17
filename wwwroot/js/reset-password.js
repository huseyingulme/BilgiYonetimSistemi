document.addEventListener('DOMContentLoaded', function () {
    const form = document.querySelector('form');
    const usernameInput = document.getElementById('username');
    const newPasswordInput = document.getElementById('newPassword');
    const confirmPasswordInput = document.getElementById('confirmPassword');

    form.addEventListener('submit', function (event) {
        let valid = true;

        if (usernameInput.value.trim() === "") {
            alert('Kullanıcı adı alanını boş bırakmayın.');
            valid = false;
        }

        if (newPasswordInput.value !== confirmPasswordInput.value) {
            alert('Yeni şifreler uyuşmuyor.');
            valid = false;
        }

        if (newPasswordInput.value.length < 6) {
            alert('Şifreniz en az 6 karakter uzunluğunda olmalıdır.');
            valid = false;
        }

        if (!valid) {
            event.preventDefault();
        }
    });
});
