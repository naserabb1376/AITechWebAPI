using System.ComponentModel.DataAnnotations;

namespace AITechWebAPI.Models.UserGroup
{
    public class AddEditUserGroupRequestBody
    {
        public long ID { get; set; } = 0;

        [Display(Name = "کد کاربر")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public long UserId { get; set; }

        [Display(Name = "کد گروه")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public long GroupId { get; set; }

    }
}
