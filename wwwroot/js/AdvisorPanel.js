function showSection(sectionId) {
    const sections = document.querySelectorAll(".section");
    const menuItems = document.querySelectorAll(".nav-item");

    // Tüm sekmeleri gizle
    sections.forEach(section => {
        section.style.display = "none"; // Tüm sekmeleri gizle
    });

    // Seçilen sekmeyi göster
    const activeSection = document.getElementById(sectionId);
    if (activeSection) {
        activeSection.style.display = "block";
    }

    // Menü öğelerini aktif durumdan çıkar
    menuItems.forEach(menuItem => {
        menuItem.classList.remove("active");
    });

    // Seçilen menüyü aktif yap
    const activeMenuItem = Array.from(menuItems).find(
        menuItem => menuItem.textContent.trim() === document.querySelector(`[onclick="showSection('${sectionId}')"]`).textContent.trim()
    );
    if (activeMenuItem) {
        activeMenuItem.classList.add("active");
    }

    // Aktif sekmeyi localStorage'da sakla
    localStorage.setItem("activeSection", sectionId);
}

// LocalStorage kullanarak son aktif menüyü yükleme
document.addEventListener("DOMContentLoaded", () => {
    const activeSection = localStorage.getItem("activeSection") || "duyurular";
    showSection(activeSection);
});

// Çıkış yapma işlemi
function logout() {
    window.location.href = '/'; // Kullanıcıyı giriş sayfasına yönlendir
}
function toggleUpdateForm() {
    const form = document.getElementById("updateForm");
    if (form.style.display === "none" || form.style.display === "") {
        form.style.display = "block"; // Formu göster
    } else {
        form.style.display = "none"; // Formu gizle
    }
}