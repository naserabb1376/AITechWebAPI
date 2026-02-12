using AITechDATA.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AITechWebAPI.ViewModels
{ 
    public class FieldInFormVM : BaseVM
    {
        public long FormId { get; set; }
        public string FormKey { get; set; }
        public string EntityName { get; set; } // نام جدول مرتبط (مثلاً "User", "Course", "Event")
        public string? FormTitle { get; set; }
        public long FormFieldId { get; set; }
        public string FieldName { get; set; }
        public string DisplayName { get; set; }

    }
}