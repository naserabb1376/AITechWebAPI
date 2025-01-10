using AITechDATA.Tools;
using System.ComponentModel.DataAnnotations;

namespace AITechDATA.Domain
{
    public class BaseEntity
    {
        [Key]
        [Display(Name = "آیدی")]
        public long ID { get; set; }

        [Display(Name = "تاریخ ساخت")]
        public DateTime? CreateDate { get; set; }

        [Display(Name = "تاریخ بروزرسانی")]
        public DateTime? UpdateDate { get; set; } = DateTime.Now.ToShamsi();

        //[Display(Name = "توضیحات")]
        //public string? Description { get; set; }
    }
}