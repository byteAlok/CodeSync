const sidebar = document.getElementById('sidebarMenu');
const toggleSidebar = document.getElementById('toggleSidebar');
const closeSidebar = document.getElementById('closeSidebar');
const overlay = document.getElementById('overlay');

// Function to set sidebar state based on screen width
function setSidebarState() {
    if (window.innerWidth >= 768) {
        sidebar.classList.remove('collapsed'); // show sidebar on desktop
        overlay.classList.remove('show'); // hide overlay
    } else {
        sidebar.classList.add('collapsed'); // collapse sidebar on mobile
        overlay.classList.remove('show'); // hide overlay initially
    }
}

// Initial check on page load
setSidebarState();

// Toggle sidebar and overlay on mobile
toggleSidebar.addEventListener('click', () => {
    sidebar.classList.toggle('collapsed');
    overlay.classList.toggle('show');
});

// Close sidebar via button or overlay
closeSidebar.addEventListener('click', () => {
    sidebar.classList.add('collapsed');
    overlay.classList.remove('show');
});
overlay.addEventListener('click', () => {
    sidebar.classList.add('collapsed');
    overlay.classList.remove('show');
});

// Update sidebar state on resize
window.addEventListener('resize', setSidebarState);



