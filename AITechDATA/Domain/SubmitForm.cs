using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AITechDATA.Domain
{
    public class SubmitForm : BaseEntity
    {
        public string FormKey { get; set; }
        public string EntityName { get; set; } // نام جدول مرتبط (مثلاً "User", "Course", "Event")
        public string? Title { get; set; }
        public string? Description { get; set; } = ""; // توضیحات
        public long CreatorId { get; set; } = 0; // کاربر ایجاد کننده

        public List<FieldInForm> FieldInForms { get; set; }

    }

    public class SubmitFormObjDto : BaseEntity
    {
        public string FormKey { get; set; }
        public string EntityName { get; set; } // نام جدول مرتبط (مثلاً "User", "Course", "Event")
        public string? Title { get; set; }
        public string? Description { get; set; } = ""; // توضیحات
        public long CreatorId { get; set; } = 0; // کاربر ایجاد کننده

        public List<FormFieldDto> Fields { get; set; } = new List<FormFieldDto>();

    }
}
