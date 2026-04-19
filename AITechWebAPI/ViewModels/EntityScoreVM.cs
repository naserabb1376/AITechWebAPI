using AITechDATA.Domain;
using MTPermissionCenter.EFCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AITechWebAPI.ViewModels
{
    public class EntityScoreVM : BaseVM
    {
        public string? ScoreItemTitle { get; set; }
        public float ScoreItemWeight { get; set; }
        public float ScoreItemTotalScore { get; set; }
        public long? ScoreItemParentId { get; set; }
        public string? ScoreItemKey { get; set; }
        public string RecordLevel { get; set; } // 0: Parent 1: Child 2: Data

        public long UserId { get; set; } // کلید خارجی به User
        public string UserName { get; set; } // ارتباط با User

        public string EntityType { get; set; } // نام جدول مبدا (مثلاً "User", "Course", "Event")
        public long ForeignKeyId { get; set; } // کلید خارجی به رکورد اصلی
        public string? TargetObjName { get; set; }
        public string? Description { get; set; } = ""; // توضیحات
        public long CreatorId { get; set; } = 0; // کاربر ایجاد کننده
    }
}