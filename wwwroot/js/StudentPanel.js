function toggleSection(sectionId) {
    const section = document.getElementById(sectionId);
    const isHidden = section.classList.contains('hidden');

    document.querySelectorAll('.section').forEach(s => {
        s.classList.add('hidden');
        s.classList.remove('visible');
    });

    if (isHidden) {
        section.classList.remove('hidden');
        section.classList.add('visible');
    }
}

