using AITechWebAPI.Models.Public;
using System.ComponentModel.DataAnnotations;

namespace AITechWebAPI.Models.Course
{
    public class GetCourseListRequestBody : GetListRequestBody
    {
        [Display(Name = "کد دسته بندی")]
        public long CategoryId { get; set; } = 0;

        [Display(Name = "دوره منتخب")]
        public bool? IsSelected { get; set; } = null;
    }
}
