// ==========================================================
// 🚨🚨🚨 مهم: تنظیمات API - آدرس کامل و دقیق (Absolute URL) 🚨🚨🚨
// ==========================================================
// ** آدرس‌ها با پورت 44382 و کنترلر جدید Auth به‌روزرسانی شدند. **
export const API_BASE_URL = "https://localhost:44382/api/Auth";

// 💡 هر دو عملیات چک کردن و ورود نهایی از متد /api/Auth/login استفاده می‌کنند.
export const CHECK_REGISTRATION_API_URL = `${API_BASE_URL}/login`;
export const LOGIN_API_URL = `${API_BASE_URL}/login`;
export const REGISTER_API_URL = `${API_BASE_URL}/register`;
// ==========================================================
//     پایان تنظیمات API - بقیه کد نیازی به تغییر ندارد
// ==========================================================


/**
 * تابع فراخوانی API برای بررسی وضعیت ثبت نام (استفاده از متد Login در سرور)
 * در سرور شما: اگر کاربر پیدا نشود، متد Login کد 404 (NotFound) برمی‌گرداند.
 * @param {string} mobile - شماره موبایل
 * @returns {Promise<{isRegistered: boolean}>}
 */
export async function checkUserRegistrationStatus(mobile) {

    // در این مرحله فقط شماره موبایل را می‌فرستیم و رمز عبور را خالی می‌گذاریم.
    const payload = { PhoneNumber: mobile, Password: "" };

    try {
        const response = await fetch(CHECK_REGISTRATION_API_URL, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(payload)
        });

        // 🚨 وضعیت مورد انتظار از کنترلر C# برای کاربر ثبت نام نشده: 404 (NotFound)
        if (response.status === 404) {
            // ❌ کاربر ثبت نام نکرده است (AuthController شما 404 برمی‌گرداند)
            return { isRegistered: false };
        }

        // 🚨 وضعیت مورد انتظار برای کاربر ثبت نام کرده: 400 (BadRequest) یا 200 (OK)
        // اگر کاربر پیدا شود اما رمز عبور خالی باشد، C# شما 400 برمی‌گرداند (نیاز به رمز).
        if (response.status === 400 || response.ok) {
            // ✅ کاربر ثبت نام کرده است
            return { isRegistered: true };
        }


        // اگر کد خطای دیگری غیر از 404 یا 400 رخ داد:
        let errorMessage = `خطا در سرور (کد: ${response.status}).`;
        try {
            const errorData = await response.json();
            errorMessage = errorData.message || errorData.title || errorMessage;
        } catch { }
        throw new Error(errorMessage);

    } catch (error) {
        console.error("API Call Error:", error);

        // --- منطق شبیه سازی اضطراری (در صورت قطع شدن ارتباط) ---
        if (error.message.includes('Failed to fetch') || error.message.includes('خطا در برقراری ارتباط')) {
            await new Promise(resolve => setTimeout(resolve, 800));
            // شبیه سازی بر اساس پیش شماره
            const isRegistered = mobile.startsWith('091');
            return { isRegistered: isRegistered }; // 091x = ثبت شده | 093x = جدید
        }
        // --- پایان منطق شبیه سازی ---

        throw new Error(error.message || 'خطای ناشناخته در اتصال API.');
    }
}


/**
 * تابع ورود با رمز عبور
 */
export async function attemptLogin(mobile, password) {

    const payload = { PhoneNumber: mobile, Password: password };

    try {
        const response = await fetch(LOGIN_API_URL, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(payload)
        });

        if (!response.ok) {
            let errorMessage = "نام کاربری یا رمز عبور اشتباه است.";
            // 401 برای رمز اشتباه و 404 برای کاربر جدید (که در چک ثبت نام مدیریت شد)
            if (response.status !== 401 && response.status !== 404) {
                errorMessage = `خطای سرور (کد: ${response.status}).`;
            }
            try {
                const errorData = await response.json();
                errorMessage = errorData.message || errorData.title || errorMessage;
            } catch { }
            throw new Error(errorMessage);
        }

        const data = await response.json();
        return data; // شامل token و redirectUrl

    } catch (error) {
        console.error("Login API Call Error:", error);
        throw new Error(error.message || 'خطا در برقراری ارتباط با سرور هنگام ورود.');
    }
}

/**
 * تابع ثبت نام نهایی
 */
export async function submitRegistration(fullName, phoneNumber, password) {
    const payload = { FullName: fullName, PhoneNumber: phoneNumber, Password: password };

    try {
        const response = await fetch(REGISTER_API_URL, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(payload)
        });

        if (!response.ok) {
            let errorMessage = "خطا در ثبت نام. لطفا اطلاعات را بررسی کنید.";
            try {
                const errorData = await response.json();
                if (errorData.errors && Array.isArray(errorData.errors)) {
                    errorMessage += ": " + errorData.errors.join(", ");
                } else {
                    errorMessage = errorData.message || errorMessage;
                }
            } catch { }
            throw new Error(errorMessage);
        }

        const data = await response.json();
        return data; // شامل token و redirectUrl

    } catch (error) {
        console.error("Registration API Call Error:", error);
        throw new Error(error.message || 'خطا در برقراری ارتباط با سرور هنگام ثبت نام.');
    }
}