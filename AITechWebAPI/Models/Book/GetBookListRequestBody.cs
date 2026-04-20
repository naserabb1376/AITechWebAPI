using AITechWebAPI.Models.Public;
using System.ComponentModel.DataAnnotations;

namespace AITechWebAPI.Models.Book
{
    public class GetBookListRequestBody : GetListRequestBody
    {
        [Display(Name = "کد دسته بندی")]
        public long CategoryId { get; set; } = 0;
    }
}
