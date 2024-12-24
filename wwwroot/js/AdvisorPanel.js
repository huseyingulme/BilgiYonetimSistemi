// Sayfa içeriklerini gösterme işlevi
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


function viewStudentDetails(studentID) {
    // Modal kutusunu seç
    const modal = document.getElementById("studentDetailsModal");

    // Modalı temizle
    document.getElementById("studentName").textContent = "Yükleniyor...";
    document.getElementById("studentEmail").textContent = "";
    const coursesList = document.getElementById("studentCoursesList");
    coursesList.innerHTML = ""; // Önceki dersleri temizle

    modal.style.display = "block";

    // API çağrısı
    fetch(`https://localhost:7227/api/Students/${studentID}`)
        .then(response => {
            if (!response.ok) {
                throw new Error(`API Hatası: ${response.statusText}`);
            }
            return response.json();
        })
        .then(data => {
            // Modalı doldur
            document.getElementById("studentName").textContent = `${data.firstName} ${data.lastName}`;
            document.getElementById("studentEmail").textContent = data.email;

            if (data.courses && data.courses.length > 0) {
                data.courses.forEach(course => {
                    const listItem = document.createElement("li");
                    listItem.textContent = `${course.courseName} (Seçim Tarihi: ${course.selectionDate})`;
                    coursesList.appendChild(listItem);
                });
            } else {
                const listItem = document.createElement("li");
                listItem.textContent = "Bu öğrenci henüz ders seçimi yapmamış.";
                coursesList.appendChild(listItem);
            }
        })
        .catch(error => {
            console.error("Hata:", error);
            alert("Öğrenci bilgileri yüklenirken bir hata oluştu.");
        });
}

document.addEventListener("DOMContentLoaded", () => {
    // Onayla butonlarına tıklama olayı
    document.querySelectorAll(".btn-approve").forEach(button => {
        button.addEventListener("click", async event => {
            event.preventDefault(); // Formun varsayılan gönderimini engelle
            const form = event.target.closest("form");

            // Formdan verileri al
            const id = form.querySelector("input[name='id']").value;
            const studentID = form.querySelector("input[name='studentID']").value;
            const courseID = form.querySelector("input[name='courseID']").value;

            // Onaylama API isteği
            const approvedSelection = {
                CourseID: courseID,
                StudentID: studentID,
                SelectionDate: new Date().toISOString(),
                IsApproved: true
            };

            try {
                // API'ye POST isteği gönder
                const postResponse = await fetch("https://localhost:7227/api/StudentCourseSelections", {
                    method: "POST",
                    headers: {
                        "Content-Type": "application/json"
                    },
                    body: JSON.stringify(approvedSelection)
                });

                if (postResponse.ok) {
                    // API'ye DELETE isteği gönder
                    const deleteResponse = await fetch(`https://localhost:7227/api/NonConfirmedSelections/${id}`, {
                        method: "DELETE"
                    });

                    if (deleteResponse.ok) {
                        alert("Ders başarıyla onaylandı ve silindi!");
                        // Sayfayı yeniden yükle
                        location.reload();
                    } else {
                        const errorText = await deleteResponse.text();
                        alert(`Onay başarılı ancak silme sırasında hata oluştu: ${errorText}`);
                    }
                } else {
                    const errorText = await postResponse.text();
                    alert(`Ders onaylanırken hata oluştu: ${errorText}`);
                }
            } catch (error) {
                console.error("Onaylama sırasında bir hata oluştu:", error);
                alert("Onaylama sırasında bir hata oluştu. Daha sonra tekrar deneyin.");
            }
        });
    });

    // Reddet butonlarına tıklama olayı
    document.querySelectorAll(".btn-reject").forEach(button => {
        button.addEventListener("click", async event => {
            event.preventDefault(); // Formun varsayılan gönderimini engelle
            const form = event.target.closest("form");
            const id = form.querySelector("input[name='id']").value;

            try {
                // API'ye DELETE isteği gönder
                const response = await fetch(`https://localhost:7227/api/NonConfirmedSelections/${id}`, {
                    method: "DELETE"
                });

                if (response.ok) {
                    alert("Ders başarıyla reddedildi ve silindi!");
                    // Sayfayı yeniden yükle
                    location.reload();
                } else {
                    const errorText = await response.text();
                    alert(`Ders reddedilirken hata oluştu: ${errorText}`);
                }
            } catch (error) {
                console.error("Reddetme sırasında bir hata oluştu:", error);
                alert("Reddetme sırasında bir hata oluştu. Daha sonra tekrar deneyin.");
            }
        });
    });
});

function closeModal() {
    const modal = document.getElementById("studentDetailsModal");
    if (modal) {
        modal.style.display = "none";
    }
}

