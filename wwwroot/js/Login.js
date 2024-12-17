document.addEventListener("DOMContentLoaded", function () {
    // Varsayılan olarak öğrenci formu göster
    showForm("student-form");
});

function showForm(formId) {
    // Tüm formları gizle
    document.querySelectorAll(".login-form").forEach(form => {
        form.classList.add("hidden");
    });

    // İlgili formu göster
    document.getElementById(formId).classList.remove("hidden");

    // Sekme butonları için aktif durumu ayarla
    document.querySelectorAll(".tab-button").forEach(button => {
        button.classList.remove("active");
    });

    if (formId === "student-form") {
        document.getElementById("ogrenci-tab").classList.add("active");
    } else {
        document.getElementById("personel-tab").classList.add("active");
    }
}
