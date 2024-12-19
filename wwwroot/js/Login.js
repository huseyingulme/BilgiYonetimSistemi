document.addEventListener("DOMContentLoaded", () => {
    showForm("staff-form"); // Varsayılan olarak öğrenci giriş formunu göster

    // Öğrenci formu kontrolü
    const studentForm = document.getElementById("student-form");
    studentForm.addEventListener("submit", function (event) {
        if (!validateForm("student-form")) {
            event.preventDefault();
        }
    });

    // Personel formu kontrolü
    const staffForm = document.getElementById("staff-form");
    staffForm.addEventListener("submit", function (event) {
        if (!validateForm("staff-form")) {
            event.preventDefault();
        }
    });
});

// Form gösterme işlevi
function showForm(formId) {
    const studentForm = document.getElementById("student-form");
    const staffForm = document.getElementById("staff-form");

    const studentBtn = document.getElementById("student-login-btn");
    const staffBtn = document.getElementById("staff-login-btn");

    if (formId === "student-form") {
        studentForm.classList.add("active");
        staffForm.classList.remove("active");

        studentBtn.classList.add("active");
        staffBtn.classList.remove("active");
    } else if (formId === "staff-form") {
        staffForm.classList.add("active");
        studentForm.classList.remove("active");

        staffBtn.classList.add("active");
        studentBtn.classList.remove("active");
    }
}

// Form doğrulama
function validateForm(formId) {
    const form = document.getElementById(formId);
    const username = form.querySelector("input[name='Username']").value.trim();
    const password = form.querySelector("input[name='Password']").value.trim();

    if (!username || !password) {
        alert("Kullanıcı adı ve şifre alanları boş bırakılamaz!");
        return false; // Form gönderimini engelle
    }
    return true; // Form gönderimine izin ver
}
