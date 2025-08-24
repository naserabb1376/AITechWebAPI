using AITechDATA.Tools;
using System.ComponentModel.DataAnnotations;

namespace AITechDATA.Domain
{
    public class BaseEntity:IHasOtherLangs
    {
        [Key]
        [Display(Name = "آیدی")]
        public long ID { get; set; }

        [Display(Name = "تاریخ ساخت")]
        public DateTime? CreateDate { get; set; }

        [Display(Name = "تاریخ بروزرسانی")]
        public DateTime? UpdateDate { get; set; } = DateTime.Now.ToShamsi();

        public string? OtherLangs { get; set; }
    }

    public class BaseVM
    {
        [Display(Name = "آیدی")]
        public long ID { get; set; }

        [Display(Name = "تاریخ ساخت")]
        public DateTime? CreateDate { get; set; }

        [Display(Name = "تاریخ بروزرسانی")]
        public DateTime? UpdateDate { get; set; } = DateTime.Now.ToShamsi();
    }
}