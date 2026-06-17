// Close the mobile sidebar when a nav link is tapped
document.addEventListener('DOMContentLoaded', function () {
    var sidebar = document.getElementById('sidebar');
    if (!sidebar) return;
    sidebar.querySelectorAll('.nav-link').forEach(function (link) {
        link.addEventListener('click', function () {
            if (window.innerWidth <= 768) sidebar.classList.remove('open');
        });
    });
    // Auto-dismiss success toast
    var toast = document.querySelector('.toast-msg.success');
    if (toast) setTimeout(function () { toast.style.transition = 'opacity .4s'; toast.style.opacity = '0'; setTimeout(function(){ toast.remove(); }, 400); }, 3500);
});
