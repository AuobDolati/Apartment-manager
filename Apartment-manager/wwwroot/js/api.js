// ==========================================================
//         🚨🚨🚨 مهم: تنظیمات API - حتماً ویرایش شود 🚨🚨🚨
// ==========================================================
// **لطفاً آدرس‌های زیر را با آدرس کامل (شامل پروتکل http/https و پورت) سرور ASP.NET Core خود جایگزین کنید.**
// مثال: "https://localhost:7000/api/Account/CheckRegistration"
export const CHECK_REGISTRATION_API_URL = "/api/Account/CheckRegistration";
export const LOGIN_API_URL = "/api/Account/Login";
// ==========================================================
//     پایان تنظیمات API - بقیه کد نیازی به تغییر ندارد
// ==========================================================


/**
 * تابع فراخوانی API برای بررسی وضعیت ثبت نام (با منطق شبیه سازی در صورت شکست)
 * @param {string} mobile - شماره موبایل
 * @returns {Promise<{isRegistered: boolean}>}
 */
export async function checkUserRegistrationStatus(mobile) {

    const isPlaceholder = CHECK_REGISTRATION_API_URL.startsWith("/") || CHECK_REGISTRATION_API_URL.includes("CheckRegistration");

    try {
        if (isPlaceholder && !window.location.host.includes('localhost')) {
            throw new Error("❌ آدرس API در کد جاوااسکریپت تنظیم نشده است. لطفا آدرس کامل سرور را جایگزین کنید.");
        }

        const response = await fetch(CHECK_REGISTRATION_API_URL, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ MobileNumber: mobile })
        });

        if (!response.ok) {
            let errorMessage = `خطا در سرور (کد: ${response.status}).`;
            try {
                const errorData = await response.json();
                errorMessage = errorData.message || errorData.title || errorMessage;
            } catch { }
            throw new Error(errorMessage);
        }

        const data = await response.json();

        if (typeof data.isRegistered !== 'boolean') {
            throw new Error("ساختار پاسخ API نادرست است: فیلد 'isRegistered' یافت نشد.");
        }
        return data;

    } catch (error) {
        console.error("API Call Error:", error);

        // --- منطق شبیه سازی اضطراری ---
        if (isPlaceholder || error.message.includes('❌ آدرس API') || error.message.includes('Failed to fetch') || error.message.includes('خطا در برقراری ارتباط')) {
            // از utils.js برای نمایش پیام استفاده می‌شود.
            // showMessage("⚠️ هشدار: اتصال به API برقرار نشد. از منطق شبیه‌سازی برای تست استفاده می‌شود. (091X = ثبت شده | 093X = جدید)", 'warning');

            await new Promise(resolve => setTimeout(resolve, 800));

            // شبیه سازی بر اساس پیش شماره
            const isRegistered = mobile.startsWith('091');
            return { isRegistered: isRegistered };
        }
        // --- پایان منطق شبیه سازی ---

        throw new Error(error.message || 'خطای ناشناخته در اتصال API.');
    }
}


/**
 * تابع ورود با رمز عبور
 */
export async function attemptLogin(mobile, password) {

    const isPlaceholder = LOGIN_API_URL.startsWith("/") || LOGIN_API_URL.includes("Login");

    try {
        const response = await fetch(LOGIN_API_URL, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ MobileNumber: mobile, Password: password })
        });

        if (!response.ok) {
            let errorMessage = "نام کاربری یا رمز عبور اشتباه است.";
            if (response.status !== 400 && response.status !== 401) {
                errorMessage = `خطای سرور (کد: ${response.status}).`;
            }
            try {
                const errorData = await response.json();
                errorMessage = errorData.message || errorData.title || errorMessage;
            } catch { }
            throw new Error(errorMessage);
        }

        const data = await response.json();
        return data;

    } catch (error) {
        console.error("Login API Call Error:", error);

        // --- منطق شبیه سازی اضطراری ورود ---
        if (isPlaceholder || error.message.includes('Failed to fetch') || error.message.includes('خطا در برقراری ارتباط')) {
            await new Promise(resolve => setTimeout(resolve, 800));
            if (password === '123') {
                // showMessage('ورود با موفقیت انجام شد! (حالت شبیه سازی)', 'success');
                return { success: true, token: 'fake-token' };
            } else {
                throw new Error('رمز عبور وارد شده اشتباه است. (حالت شبیه سازی)');
            }
        }
        // --- پایان منطق شبیه سازی ---

        throw new Error(error.message || 'خطا در برقراری ارتباط با سرور هنگام ورود.');
    }
}