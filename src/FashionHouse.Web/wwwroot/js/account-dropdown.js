document.addEventListener('click', function (e) {
    var toggle = e.target.closest('.account-dropdown__toggle');
    var openMenu = document.querySelector('.account-dropdown__menu.show');

    if (toggle) {
        e.preventDefault();
        var menu = toggle.parentElement.querySelector('.account-dropdown__menu');
        var isOpen = menu.classList.contains('show');
        // close any other open menu first
        document.querySelectorAll('.account-dropdown__menu.show').forEach(function (m) {
            m.classList.remove('show');
        });
        if (!isOpen) menu.classList.add('show');
    } else if (!e.target.closest('.account-dropdown')) {
        document.querySelectorAll('.account-dropdown__menu.show').forEach(function (m) {
            m.classList.remove('show');
        });
    }
});