using MTPermissionCenter.EFCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AITechDATA.Domain
{
    public class EntityScore : BaseEntity
    {
        public string? ScoreItemTitle { get; set; }  // عنوان سر شاخص یا زیر شاخص
        public float ScoreItemRawScore { get; set; } = 0.0f;  // امتیاز خام داده های شاخص (اجباری برای سطح داده 2)
        public float ScoreItemWeightPercent { get; set; } = 0.0f; //وزن درصدی هر شاخص اجباری برای سطح داده 2
        public float ScoreItemWeightedScore { get; set; } = 0.0f; // امتیاز هر زیر شاخص با احتساب وزن برای لایه داده 2  ScoreItemRawScore / 100) * ScoreItemWeightPercent
        public float ScoreItemTotalScore { get; set; } = 0.0f; // امتیاز کل با احتساب همه زیر شاخص برای هر گزارش مدیریتی یا وظیفه
        public long? ScoreItemParentId { get; set; } // کد والد هر رکورد سرشاخص -> زیر شاخص -> داده
        public string ScoreItemKey { get; set; } //  کلید انگلیسی مربوط به هر شاخص یا زیر شاخص اجباری برای همه سطح ها و یونیک
        public int RecordLevel { get; set; } // 0: Parent 1: Child 2: Data وجه تمایز سطح هر رکورد

        public long UserId { get; set; } // کلید خارجی به User
        public User User { get; set; } // ارتباط با User 

        public string EntityType { get; set; } // نام جدول مبدا (مثلاً "User", "Course", "Event")
        public long ForeignKeyId { get; set; } // کلید خارجی به رکورد اصلی
        public string? TargetObjName { get; set; } // نام یا عنوان یا هر نشانی از رکورد مورد نظر وظیفه یا گزارش مدیریتی (اختیاری)
        public string? Description { get; set; } = ""; // توضیحات
        public long CreatorId { get; set; } = 0; // کاربر ایجاد کننده

    }
    
}