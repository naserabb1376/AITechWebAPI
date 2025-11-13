using System.ComponentModel.DataAnnotations;

namespace AITechWebAPI.Models.LoginMethod
{
    public class AddEditLoginMethodRequestBody
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

        [Display(Name = "شماره موبایل کاربر")]
        public string? MobileNumber { get; set; } // شماره موبایل کاربر

        [Display(Name = "کلمه عبور")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(20)]
        [DataType(DataType.Password)] //Hide Characters
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{6,20}$", ErrorMessage = "کلمه عبور باید شامل حرف و عدد باشد")] //check exist number & alphabet chars in password field
        public string Password { get; set; }

        [Display(Name = "تاریخ انقضا")]
        public string? ExpirationDate { get; set; }
      // public string? Description { get; set; }
    }
}
