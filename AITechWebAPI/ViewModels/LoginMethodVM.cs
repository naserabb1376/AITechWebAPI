using AITechDATA.Domain;

namespace AITechWebAPI.ViewModels
{
    // LoginMethod: جدول روش‌های ورود
    public class LoginMethodVM : BaseVM
    {
        public long UserId { get; set; } // کلید خارجی به User
        public string UserName { get; set; } // ارتباط با User
        public string Method { get; set; } // نوع روش (SMS, Token)
        public string Token { get; set; } // کد پیامک یا توکن
        public DateTime? ExpirationDate { get; set; } // تاریخ انقضا
    }
}