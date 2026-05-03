

using AITechDATA.Domain;
using System.ComponentModel.DataAnnotations.Schema;

namespace AITechWebAPI.ViewModels
{
    public class TimeFunctionVM : BaseVM
    {
        public long UserId { get; set; } // کلید خارجی به User
        public string UserName { get; set; } // ارتباط با User

        public DateTime TimeFunctionStartDate { get; set; }
        public DateTime TimeFunctionEndDate { get; set; }
        public string? Description { get; set; }


    }
}