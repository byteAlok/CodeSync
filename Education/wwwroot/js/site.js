/* ==============================================================
   🚀 CodeSync Interactive Client Scripts
   Theme Engine • Dynamic Footer • Mobile Nav
   ============================================================== */

// Auto collapse mobile navbar on outside click
document.addEventListener('DOMContentLoaded', () => {
    const navbarCollapse = document.getElementById('navbarNav');
    
    document.addEventListener('click', function (event) {
        if (navbarCollapse && navbarCollapse.classList.contains('show')) {
            const isClickInside = navbarCollapse.contains(event.target) || event.target.closest('.navbar-toggler');
            if (!isClickInside) {
                const bsCollapse = bootstrap.Collapse.getInstance(navbarCollapse) || new bootstrap.Collapse(navbarCollapse, { toggle: false });
                bsCollapse.hide();
            }
        }
    });

    // Theme Engine System
    const themeToggleBtn = document.getElementById('theme-toggle');
    const htmlEl = document.documentElement;
    const themeIcon = document.getElementById('theme-icon');

    // Retrieve saved preference or check OS preference
    const savedTheme = localStorage.getItem('codesync_theme');
    const systemPrefersDark = window.matchMedia && window.matchMedia('(prefers-color-scheme: dark)').matches;
    
    let currentTheme = savedTheme || (systemPrefersDark ? 'dark' : 'light');
    
    applyTheme(currentTheme);

    if (themeToggleBtn) {
        themeToggleBtn.addEventListener('click', () => {
            currentTheme = currentTheme === 'light' ? 'dark' : 'light';
            applyTheme(currentTheme);
            localStorage.setItem('codesync_theme', currentTheme);
        });
    }

    // Listen to OS theme changes if user has no saved preference
    if (window.matchMedia) {
        window.matchMedia('(prefers-color-scheme: dark)').addEventListener('change', e => {
            if (!localStorage.getItem('codesync_theme')) {
                applyTheme(e.matches ? 'dark' : 'light');
            }
        });
    }

    function applyTheme(theme) {
        htmlEl.setAttribute('data-bs-theme', theme);
        if (!themeIcon) return;
        if (theme === 'dark') {
            themeIcon.className = 'bi bi-sun-fill text-warning';
        } else {
            themeIcon.className = 'bi bi-moon-stars-fill text-secondary';
        }
    }
});

/* ---------------------------- Footer Dynamic Clock ------------------------ */
function updateFooterClock() {
    const now = new Date();
    const options = {
        weekday: 'long', year: 'numeric', month: 'short',
        day: 'numeric', hour: '2-digit', minute: '2-digit', second: '2-digit'
    };
    const dtEl = document.getElementById("currentDateTime");
    if (dtEl) dtEl.textContent = now.toLocaleString('en-IN', options);
}

const yearEl = document.getElementById("currentYear");
if (yearEl) yearEl.textContent = new Date().getFullYear();

setInterval(updateFooterClock, 1000);
updateFooterClock();