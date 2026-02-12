using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AITechDATA.Domain
{ 
    public class FieldInForm : BaseEntity
    {
        public long FormId { get; set; }

        [ForeignKey("FormId")]
        public SubmitForm Form { get; set; }

        public long FormFieldId { get; set; }

        [ForeignKey("FormFieldId")]
        public FormField FormField { get; set; }

    }
}