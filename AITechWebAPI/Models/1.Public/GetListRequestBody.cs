using System.ComponentModel.DataAnnotations;

namespace AITechWebAPI.Models.Public
{
    public class GetListRequestBody
    {
        public int PageIndex { get; set; } = 1;

        [Display(Name = "اندازه صفحه")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public int PageSize { get; set; }
        public string SearchText { get; set; } = "";
        public string SortQuery { get; set; } = "";
    }
}
