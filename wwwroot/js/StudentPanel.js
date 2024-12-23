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
    const activeSection = localStorage.getItem("activeSection") || "duyurular";
    showSection(activeSection);
});




// Ders seçimi işlevi
let selectedCourses = [];
function toggleSelection(element, courseId) {
    if (element.classList.contains("selected")) {
        element.classList.remove("selected");
        selectedCourses = selectedCourses.filter(id => id !== courseId);
    } else {
        element.classList.add("selected");
        selectedCourses.push(courseId);
    }
}

function toggleCourseSelection(element, courseId) {
    const checkbox = element.querySelector('input[type="checkbox"]');
    const isMandatory = checkbox.classList.contains('mandatory-checkbox');
    checkbox.checked = !checkbox.checked;

    if (isMandatory) {
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

// Ders seçimlerini onaylama
function confirmCourseSelection() {
    if (selectedCourses.length === 0) {
        alert("Lütfen en az bir ders seçin.");
        return;
    }

    // Seçilen dersleri API'ye gönder
    fetch('/Student/SubmitCourseSelections', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(selectedCourses)
    })
        .then(response => {
            if (response.ok) {
                alert("Ders seçimi başarıyla kaydedildi!");
                location.reload();
            } else {
                alert("Ders seçimi sırasında bir hata oluştu.");
            }
        })
        .catch(error => {
            console.error("Hata:", error);
            alert("Ders seçimi sırasında bir hata oluştu.");
        });
}

// Ders listesi tıklama etkinliği
document.addEventListener("DOMContentLoaded", () => {
    const courseList = document.getElementById("courseList");
    const form = document.getElementById("courseSelectionForm");

    courseList.addEventListener("click", event => {
        const clickedElement = event.target.closest(".course-option");
        if (clickedElement) {
            const checkbox = clickedElement.querySelector("input[type='checkbox']");
            checkbox.checked = !checkbox.checked;
            clickedElement.classList.toggle("selected", checkbox.checked);
        }
    });

    // Form gönderiminde seçili ders kontrolü
    form.addEventListener("submit", event => {
        const selectedCourses = document.querySelectorAll("input[name='selectedCourseIds']:checked");
        if (selectedCourses.length === 0) {
            event.preventDefault();
            alert("Lütfen en az bir ders seçin.");
        }
    });
});





// Çıkış yapma işlemi
function logout() {
    window.location.href = '/'; // Kullanıcıyı giriş sayfasına yönlendir
}

