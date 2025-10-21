// wwwroot/js/site.js
(function () {
    const KEY = "sb:collapsed";

    // Helpers
    const $ = (s) => document.querySelector(s);
    const pick = (...sels) => sels.map($).find(Boolean);

    const body = document.body;
    const sidebar = pick(".sb-sidebar");
    const content = pick("#sb-container", ".sb-main", "main") || body;
    const toggle = $("#sb-toggle") || document.querySelector("[data-sb-toggle]");

    if (!sidebar || !content) return;

    // Estado persistido
    if (localStorage.getItem(KEY) === "1") body.classList.add("sb-collapsed");

    // Ajusta el offset leyendo SIEMPRE el ancho real del sidebar
    function applyOffset() {
        const w = sidebar.getBoundingClientRect().width || 0;
        content.style.marginLeft = `${w}px`;
    }

    // 1) Cada clic de toggle
    if (toggle) {
        toggle.addEventListener("click", () => {
            const collapsed = body.classList.toggle("sb-collapsed");
            localStorage.setItem(KEY, collapsed ? "1" : "0");
            // Deja que la animación empiece y vuelve a medir después
            requestAnimationFrame(() => applyOffset());
        });
    }

    // 2) Observa el ancho del sidebar (cubre transición y cambios de responsive)
    if ("ResizeObserver" in window) {
        const ro = new ResizeObserver(applyOffset);
        ro.observe(sidebar);
    } else {
        // Fallback
        window.addEventListener("resize", applyOffset);
        sidebar.addEventListener("transitionend", applyOffset);
    }

    // 3) Inicial
    document.addEventListener("DOMContentLoaded", applyOffset);
    applyOffset();
})();
