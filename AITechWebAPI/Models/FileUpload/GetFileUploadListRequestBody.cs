using AITechWebAPI.Models.Public;
using System.ComponentModel.DataAnnotations;

namespace AITechWebAPI.Models.FileUpload
{
    public class GetFileUploadListRequestBody : GetListRequestBody
    {

        [Display(Name = "کد تمرین")]
        public long AssignmentId { get; set; } = 0;
    }
}
