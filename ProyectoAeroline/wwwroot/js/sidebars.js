(function () {
    const key = 'sb-collapsed';
    const btn = document.getElementById('btnToggleSidebar');

    // Restaurar estado guardado
    const saved = localStorage.getItem(key);
    if (saved === '1') document.body.classList.add('sb-collapsed');

    // Toggle con botón
    if (btn) {
        btn.addEventListener('click', () => {
            document.body.classList.toggle('sb-collapsed');
            localStorage.setItem(key, document.body.classList.contains('sb-collapsed') ? '1' : '0');
        });
    }
})();
