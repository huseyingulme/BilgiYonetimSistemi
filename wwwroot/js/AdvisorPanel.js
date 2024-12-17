function showSection(sectionId) {
    const sections = document.querySelectorAll('.section');
    sections.forEach(section => section.style.display = 'none');

    document.getElementById(sectionId).style.display = 'block';
}

function toggleUpdateForm() {
    const form = document.getElementById('updateForm');
    form.style.display = form.style.display === 'block' ? 'none' : 'block';
}

function logout() {
    // Çıkış işlemi gerçekleştirilir
    window.location.href = '/Account/Logout';
}
