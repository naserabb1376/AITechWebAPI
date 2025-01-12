using System.ComponentModel.DataAnnotations;

namespace AITechWebAPI.Models.Login
{
    public class AddEditLoginRequestBody
    {
        public long ID { get; set; } = 0;

        [Display(Name = "کد کاربر")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [Range(1, long.MaxValue, ErrorMessage = "مقدار {0} باید بزرگتر از 0 باشد")]
        public long UserID { get; set; }

        [Display(Name = "توکن")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string Token { get; set; }

        [Display(Name = "روش ورود")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string Method { get; set; }

        [Display(Name = "کلمه عبور")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string Password { get; set; }

        [Display(Name = "تاریخ انقضا")]
        public DateTime? ExpirationDate { get; set; }
      // public string? Description { get; set; }
    }
}
