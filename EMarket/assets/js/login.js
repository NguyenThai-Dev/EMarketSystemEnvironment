
class ModernSaaSLoginForm {
    constructor() {
        this.form = document.getElementById('loginForm');
        this.emailInput = document.getElementById('email');
        this.passwordInput = document.getElementById('password');
        this.passwordToggle = document.getElementById('passwordToggle');
        this.submitButton = this.form.querySelector('.submit-btn');
        this.successMessage = document.getElementById('successMessage');
        this.socialButtons = document.querySelectorAll('.social-btn');

        this.init();
    }

    init() {
        this.bindEvents();
        this.setupPasswordToggle();
        this.setupSocialButtons();
    }

    bindEvents() {
        this.form.addEventListener('submit', (e) => this.handleSubmit(e));
        this.emailInput.addEventListener('blur', () => this.validateEmail());
        this.passwordInput.addEventListener('blur', () => this.validatePassword());
        this.emailInput.addEventListener('input', () => this.clearError('email'));
        this.passwordInput.addEventListener('input', () => this.clearError('password'));
    }

    setupPasswordToggle() {
        this.passwordToggle.addEventListener('click', () => {
            const type = this.passwordInput.type === 'password' ? 'text' : 'password';
            this.passwordInput.type = type;

            this.passwordToggle.style.color = type === 'text' ? '#635BFF' : '#8792a2';
        });
    }

    setupSocialButtons() {
        this.socialButtons.forEach(button => {
            button.addEventListener('click', (e) => {
                const provider = button.textContent.trim();
                this.handleSocialLogin(provider, button);
            });
        });
    }

    validateEmail() {
        const email = this.emailInput.value.trim();
        const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;

        if (!email) {
            this.showError('email', 'Email là bắt buộc');
            return false;
        }

        if (!emailRegex.test(email)) {
            this.showError('email', 'Hãy nhập địa chỉ Email hợp lệ');
            return false;
        }

        this.clearError('email');
        return true;
    }

    validatePassword() {
        const password = this.passwordInput.value;

        if (!password) {
            this.showError('password', 'Mật khẩu là bắt buộc');
            return false;
        }

        if (password.length < 6) {
            this.showError('password', 'Mật khẩu phải có ít nhất 6 ký tự');
            return false;
        }

        this.clearError('password');
        return true;
    }

    showError(field, message) {
        const inputGroup = document.getElementById(field).closest('.input-group');
        const errorElement = document.getElementById(`${field}Error`);

        inputGroup.classList.add('error');
        errorElement.textContent = message;
        errorElement.classList.add('show');
    }

    clearError(field) {
        const inputGroup = document.getElementById(field).closest('.input-group');
        const errorElement = document.getElementById(`${field}Error`);

        inputGroup.classList.remove('error');
        errorElement.classList.remove('show');
        setTimeout(() => {
            errorElement.textContent = '';
        }, 200);
    }

    async handleSubmit(e) {
        e.preventDefault();

        const email = this.form.querySelector("input[name='EmailOrUsername']").value;
        const password = this.form.querySelector("input[name='Password']").value;
        const returnUrl = this.form.querySelector("input[name='ReturnUrl']")?.value || "";

        // --- LẤY TOKEN Ở ĐÂY ---
        const token = this.form.querySelector("input[name='__RequestVerificationToken']").value;

        const url = "/Admin/Login/Login";

        if (!this.validateEmail() || !this.validatePassword()) {
            return;
        }

        this.setLoading(true);

        try {
            const response = await fetch(url, {
                method: "POST",
                headers: {
                    "Content-Type": "application/x-www-form-urlencoded"
                },
                // --- NHÉT TOKEN VÀO BODY ---
                body: `EmailOrUsername=${encodeURIComponent(email)}&Password=${encodeURIComponent(password)}&__RequestVerificationToken=${encodeURIComponent(token)}`
            });

            const result = await response.json();

            if (result.success) {
                this.showSuccess();
                const redirectUrl = returnUrl || "/Admin/Admin/Index";
                setTimeout(() => {
                    window.location.href = redirectUrl;
                }, 800);
            }
            else {
                FormUtils.showNotification(result.message, "error");
            }
        } catch (error) {
            FormUtils.showNotification("Lỗi hệ thống: " + error.message, "error");
        } finally {
            this.setLoading(false);
        }
    }


    async handleSocialLogin(provider, button) {
        console.log(`Signing in with ${provider}...`);

        // Simple loading state
        const originalHTML = button.innerHTML;
        button.style.pointerEvents = 'none';
        button.style.opacity = '0.7';
        button.innerHTML = `
            <svg width="14" height="14" viewBox="0 0 14 14" fill="none">
                <circle cx="7" cy="7" r="5.5" stroke="currentColor" stroke-width="1.5" opacity="0.25"/>
                <path d="M12.5 7a5.5 5.5 0 01-5.5 5.5" stroke="currentColor" stroke-width="1.5" stroke-linecap="round">
                    <animateTransform attributeName="transform" type="rotate" dur="1s" values="0 7 7;360 7 7" repeatCount="indefinite"/>
                </path>
            </svg>
            Connecting...
        `;

        try {
            // Thay vì chỉ log, mình sẽ submit cái form ẩn chứa Google Auth
            const form = button.closest('form');
            if (form) {
                form.submit(); // Chạy thẳng lệnh POST sang ExternalLogin
            }
        } catch (error) {
            // Dùng FormUtils để báo lỗi nếu có sự cố JS
            FormUtils.showNotification("Không thể kết nối với " + provider, "error");
        } finally {
            button.style.pointerEvents = 'auto';
            button.style.opacity = '1';
            button.innerHTML = originalHTML;
        }
    }

    setLoading(loading) {
        this.submitButton.classList.toggle('loading', loading);
        this.submitButton.disabled = loading;

        // Disable social buttons during loading
        this.socialButtons.forEach(button => {
            button.style.pointerEvents = loading ? 'none' : 'auto';
            button.style.opacity = loading ? '0.6' : '1';
        });
    }

    showSuccess() {
        // Hide form with smooth transition
        this.form.style.transform = 'scale(0.95)';
        this.form.style.opacity = '0';

        setTimeout(() => {
            this.form.style.display = 'none';
            document.querySelector('.social-buttons').style.display = 'none';
            document.querySelector('.divider').style.display = 'none';

            // Show success message
            this.successMessage.classList.add('show');

        }, 300);

        // Redirect after success display
        setTimeout(() => {
            console.log('Redirecting to dashboard...');
            // window.location.href = '/dashboard';
        }, 2500);
    }
}

// Initialize the form when DOM is loaded
document.addEventListener('DOMContentLoaded', () => {
    new ModernSaaSLoginForm();
});
