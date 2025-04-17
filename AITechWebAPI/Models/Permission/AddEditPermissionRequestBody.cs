using System.ComponentModel.DataAnnotations;

namespace AITechWebAPI.Models.Permission
{
    public class AddEditPermissionRequestBody
    {
        public long ID { get; set; } = 0;

        [Display(Name = "نام دسترسی")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string Name { get; set; }

        [Display(Name = "نام انگلیسی دسترسی")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string Name_EN { get; set; }

        [Display(Name = "آیکون دسترسی")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string Icon { get; set; }

        [Display(Name = "مسیر دسترسی")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string Routename { get; set; }

        [Display(Name = "شرح دسترسی")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string Description { get; set; }

        [Display(Name = "شرح دسترسی انگلیسی")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string Description_EN { get; set; } // توضیحات دسترسی
    }
}