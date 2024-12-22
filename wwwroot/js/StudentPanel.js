document.addEventListener("DOMContentLoaded", () => {
    // Ders seçimi işlemleri
    const selectedCourses = new Set();

    // Ders seçimi işlevselliği
    function toggleCourseSelection(element, courseId) {
        const checkbox = element.querySelector('input[type="checkbox"]');
        checkbox.checked = !checkbox.checked;

        if (checkbox.classList.contains('mandatory-checkbox')) {
            const mandatoryCheckboxes = document.querySelectorAll('.mandatory-checkbox');
            const selectedCount = Array.from(mandatoryCheckboxes).filter(cb => cb.checked).length;

            if (selectedCount > 6) {
                alert('Zorunlu derslerden en fazla 6 adet seçebilirsiniz.');
                checkbox.checked = false;
            }
        }

        // Görsel seçili durumu için elementin sınıfını değiştiriyoruz
        if (checkbox.checked) {
            element.classList.add('selected');
        } else {
            element.classList.remove('selected');
        }
    }

    // Ders seçimi formunu gönderme işlemi
    document.getElementById("courseSelectionForm")?.addEventListener("submit", async (e) => {
        e.preventDefault(); // Formun normalde gönderilmesini engelliyoruz
        const selectedCourseIds = Array.from(document.querySelectorAll('input[type="checkbox"]:checked')).map(cb => cb.value);

        if (selectedCourseIds.length === 0) {
            alert('Lütfen en az bir ders seçin!');
            return;
        }

        try {
            const response = await fetch("/Student/SubmitCourseSelections", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                },
                body: JSON.stringify({ courseIds: selectedCourseIds }),
            });

            if (response.ok) {
                alert("Dersler başarıyla seçildi.");
            } else {
                alert("Ders seçim işlemi başarısız oldu.");
            }
        } catch (error) {
            console.error("Ders seçimi hatası:", error);
            alert("Bir hata oluştu, lütfen tekrar deneyin.");
        }
    });
});
