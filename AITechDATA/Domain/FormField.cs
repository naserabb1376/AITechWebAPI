using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AITechDATA.Domain
{
    public class FormField : BaseEntity
    {
        public string FieldName { get; set; } 
        public string DisplayName { get; set; } 
        public string? Description { get; set; } = ""; // توضیحات

        public List<FieldInForm> FieldInForms { get; set; }

    }

    public class FormFieldDto
    {
        public string FieldName { get; set; }
        public string DisplayName { get; set; }

    }
}
