// Sayfa içeriklerini gösterme işlevi
function showSection(sectionId) {
    const sections = document.querySelectorAll(".section");
    const menuItems = document.querySelectorAll(".nav-item");

    // Tüm sekmeleri gizle
    sections.forEach(section => section.style.display = "none");

    // Seçilen sekmeyi göster
    document.getElementById(sectionId).style.display = "block";

    // Menü öğelerini aktif durumdan çıkar ve seçilen menüyü aktif yap
    menuItems.forEach(menuItem => menuItem.classList.remove("active"));
    document.querySelector(`[onclick="showSection('${sectionId}')"]`).classList.add("active");

    // Aktif sekmeyi localStorage'da sakla
    localStorage.setItem("activeSection", sectionId);
}

// LocalStorage kullanarak son aktif menüyü yükleme
document.addEventListener("DOMContentLoaded", () => {
    const activeSection = localStorage.getItem("activeSection") || "dersler";
    showSection(activeSection);
});

// Ders seçim işlevi
function toggleCourseSelection(element, courseId) {
    const checkbox = element.querySelector('input[type="checkbox"]');
    checkbox.checked = !checkbox.checked;

    if (checkbox.classList.contains('mandatory-checkbox')) {
        const mandatoryCheckboxes = document.querySelectorAll('.mandatory-checkbox');
        const selectedCount = Array.from(mandatoryCheckboxes).filter(cb => cb.checked).length;

        if (selectedCount > 6) {
            alert('Zorunlu derslerden en fazla 6 adet seçebilirsiniz.');
            checkbox.checked = false; // Fazladan seçimi iptal et
            return;
        }
    }

    // Görsel seçili durumu için elementin sınıfını değiştiriyoruz
    if (checkbox.checked) {
        element.classList.add('selected');
    } else {
        element.classList.remove('selected');
    }
}

// Çıkış yapma işlemi
function logout() {
    window.location.href = '/'; // Kullanıcıyı giriş sayfasına yönlendir
}
