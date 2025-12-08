/**
 * نمایش پیام به کاربر در جعبه پیام
 * @param {string} message - متن پیام
 * @param {string} type - نوع پیام (success, error, warning)
 */
export function showMessage(message, type = 'error') {
    const messageBox = document.getElementById('message-box');
    if (!messageBox) return;

    messageBox.textContent = message;
    messageBox.className = 'mt-4 p-3 rounded-lg text-sm';
    messageBox.classList.remove('hidden');

    if (type === 'error') {
        messageBox.classList.add('bg-red-100', 'text-red-700', 'border', 'border-red-300');
    } else if (type === 'success') {
        messageBox.classList.add('bg-green-100', 'text-green-700', 'border', 'border-green-300');
    } else if (type === 'warning') {
        messageBox.classList.add('bg-yellow-100', 'text-yellow-700', 'border', 'border-yellow-300');
    }

    // پنهان کردن پیام پس از 7 ثانیه
    setTimeout(() => {
        messageBox.classList.add('hidden');
    }, 7000);
}

/**
 * ذخیره شماره موبایل به صورت موقت در Local Storage
 * @param {string} mobile - شماره موبایل
 */
export function saveMobile(mobile) {
    localStorage.setItem('submittedMobile', mobile);
}

/**
 * بازیابی شماره موبایل از Local Storage
 * @returns {string | null}
 */
export function getMobile() {
    return localStorage.getItem('submittedMobile');
}

/**
 * حذف شماره موبایل ذخیره شده
 */
export function clearMobile() {
    localStorage.removeItem('submittedMobile');
}

/**
 * کنترل نمایش Loading
 * @param {boolean} show 
 */
export function setLoading(show) {
    const loadingIndicator = document.getElementById('loading-indicator');
    if (loadingIndicator) {
        if (show) {
            loadingIndicator.classList.remove('hidden');
        } else {
            loadingIndicator.classList.add('hidden');
        }
    }
}