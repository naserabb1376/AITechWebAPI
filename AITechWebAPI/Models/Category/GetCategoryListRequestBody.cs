using AITechWebAPI.Models.Public;
using System.ComponentModel.DataAnnotations;

namespace AITechWebAPI.Models.Category
{
    public class GetCategoryListRequestBody : GetListRequestBody
    {
        public string CategoryEntityType { get; set; } = "";
    }
}
