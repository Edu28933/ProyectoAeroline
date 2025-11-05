(function () {
    const key = 'sb-collapsed';
    const btn = document.getElementById('btnToggleSidebar');
    const sidebar = document.querySelector('.sb-sidebar');
    let overlay = document.querySelector('.sb-overlay');
    let isMobile = window.innerWidth < 992;

    // Crear overlay si no existe
    if (!overlay) {
        const overlayDiv = document.createElement('div');
        overlayDiv.className = 'sb-overlay';
        overlayDiv.addEventListener('click', closeSidebar);
        document.body.appendChild(overlayDiv);
        overlay = overlayDiv;
    }

    // Función para abrir sidebar en móviles
    function openSidebar() {
        if (sidebar) {
            sidebar.classList.add('show');
            if (overlay) overlay.classList.add('show');
            document.body.style.overflow = 'hidden';
        }
    }

    // Función para cerrar sidebar en móviles
    function closeSidebar() {
        if (sidebar) {
            sidebar.classList.remove('show');
            if (overlay) overlay.classList.remove('show');
            document.body.style.overflow = '';
        }
    }

    // Detectar cambios de tamaño de pantalla
    function handleResize() {
        const wasMobile = isMobile;
        isMobile = window.innerWidth < 992;
        
        if (wasMobile !== isMobile) {
            if (!isMobile) {
                // Ya no es móvil, cerrar overlay y resetear
                closeSidebar();
            }
        }
    }

    window.addEventListener('resize', handleResize);

    // Restaurar estado guardado (solo en desktop)
    const saved = localStorage.getItem(key);
    if (saved === '1' && !isMobile) {
        document.body.classList.add('sb-collapsed');
    }

    // Toggle con botón
    if (btn) {
        btn.addEventListener('click', () => {
            if (isMobile) {
                // En móviles, abrir/cerrar drawer
                if (sidebar && sidebar.classList.contains('show')) {
                    closeSidebar();
                } else {
                    openSidebar();
                }
            } else {
                // En desktop, colapsar/expandir
                document.body.classList.toggle('sb-collapsed');
                localStorage.setItem(key, document.body.classList.contains('sb-collapsed') ? '1' : '0');
            }
        });
    }

    // Cerrar sidebar al hacer clic en un enlace en móviles
    if (sidebar && isMobile) {
        const links = sidebar.querySelectorAll('.sb-link');
        links.forEach(link => {
            link.addEventListener('click', () => {
                setTimeout(closeSidebar, 300);
            });
        });
    }
})();
