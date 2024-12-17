document.addEventListener('DOMContentLoaded', function () {
    const navLinks = document.querySelectorAll('.nav-link');
    const sections = document.querySelectorAll('section');

    navLinks.forEach(link => {
        link.addEventListener('click', function (e) {
            e.preventDefault();
            sections.forEach(section => {
                section.classList.add('hidden');
            });

            const targetSection = document.querySelector(link.getAttribute('href'));
            targetSection.classList.remove('hidden');
        });
    });

    document.querySelector('#dashboard').classList.remove('hidden');
});
