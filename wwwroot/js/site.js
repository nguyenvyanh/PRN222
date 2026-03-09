(function () {
    const userNameEl = document.getElementById('authUserName');
    const loginItemEl = document.getElementById('loginNavItem');
    const logoutItemEl = document.getElementById('logoutNavItem');
    const logoutBtnEl = document.getElementById('logoutBtn');

    if (!userNameEl || !loginItemEl || !logoutItemEl || !logoutBtnEl) {
        return;
    }

    const storageKey = 'currentUser';

    function getCurrentUser() {
        const raw = localStorage.getItem(storageKey);
        if (!raw) {
            return null;
        }

        try {
            return JSON.parse(raw);
        } catch {
            localStorage.removeItem(storageKey);
            return null;
        }
    }

    function setCurrentUser(user) {
        localStorage.setItem(storageKey, JSON.stringify(user));
    }

    function clearCurrentUser() {
        localStorage.removeItem(storageKey);
    }

    function renderAuthState() {
        const user = getCurrentUser();

        if (user && user.id) {
            userNameEl.textContent = `Hi, ${user.username ?? 'User'}`;
            loginItemEl.classList.add('d-none');
            logoutItemEl.classList.remove('d-none');
            return;
        }

        userNameEl.textContent = '';
        logoutItemEl.classList.add('d-none');
        loginItemEl.classList.remove('d-none');
    }

    async function login() {
        const username = window.prompt('Username:');
        if (!username) {
            return;
        }

        const password = window.prompt('Password:');
        if (!password) {
            return;
        }

        const response = await fetch('/api/user/login', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ username, password })
        });

        if (!response.ok) {
            const message = await response.text();
            throw new Error(message || 'Đăng nhập thất bại.');
        }

        const user = await response.json();
        setCurrentUser(user);
        renderAuthState();
    }

    async function logout() {
        const user = getCurrentUser();
        if (!user || !user.id) {
            clearCurrentUser();
            renderAuthState();
            return;
        }

        const response = await fetch('/api/user/logout', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ userId: user.id })
        });

        if (!response.ok) {
            const message = await response.text();
            throw new Error(message || 'Đăng xuất thất bại.');
        }

        clearCurrentUser();
        renderAuthState();
    }

    loginItemEl.addEventListener('click', async function (event) {
        event.preventDefault();
        try {
            await login();
        } catch (error) {
            alert(error.message);
        }
    });

    logoutBtnEl.addEventListener('click', async function () {
        try {
            await logout();
        } catch (error) {
            alert(error.message);
        }
    });

    renderAuthState();
})();