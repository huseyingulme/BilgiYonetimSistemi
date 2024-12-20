// JavaScript Kodları

document.addEventListener("DOMContentLoaded", function () {
    // Sayfa yüklendiğinde aktif olan bölümü göster
    showSection('ogrenci');

    // Menüdeki sekmelere tıklayınca ilgili bölümü göster
    const menuItems = document.querySelectorAll(".nav-item");
    menuItems.forEach(item => {
        item.addEventListener("click", function () {
            // Önce tüm menü öğelerinden 'active' sınıfını kaldır
            menuItems.forEach(i => i.classList.remove("active"));
            // Tıklanan menü öğesine 'active' sınıfını ekle
            item.classList.add("active");
        });
    });
});

// Belirli bir bölümü göstermek için kullanılan fonksiyon
function showSection(sectionId) {
    const sections = document.querySelectorAll(".section");

    // Tüm bölümleri gizle
    sections.forEach(section => {
        section.style.display = "none";
    });

    // İlgili bölümü göster
    const targetSection = document.getElementById(sectionId);
    if (targetSection) {
        targetSection.style.display = "block";
    }
}

// Bilgileri güncelleme formunu açmak için fonksiyon
function showUpdateForm() {
    const updateForm = document.getElementById("updateForm");
    if (updateForm) {
        updateForm.style.display = "block";
    }
}

// Ders seçimini yönetmek için kullanılan fonksiyon
function toggleCourseSelection(courseId) {
    const inputElement = document.querySelector(`input[value='${courseId}']`);
    if (inputElement) {
        inputElement.checked = !inputElement.checked; // Checkbox durumunu tersine çevir

        // Görsel geri bildirim için seçilen öğeye bir sınıf ekle veya çıkar
        const listItem = inputElement.closest(".course-option");
        if (listItem) {
            if (inputElement.checked) {
                listItem.classList.add("selected");
            } else {
                listItem.classList.remove("selected");
            }
        }
    }
}

// Çıkış yapma işlemini yönetmek için fonksiyon
function logout() {
    if (confirm("Hesabınızdan çıkış yapmak istediğinizden emin misiniz?")) {
        // Çıkış işlemini gerçekleştirin
        window.location.href = "/Logout"; // Çıkış yönlendirmesi
    }
}
