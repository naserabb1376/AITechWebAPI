using AITechDATA.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AITechWebAPI.ViewModels
{
    public class FormFieldVM : BaseVM
    {
        public string FieldName { get; set; } 
        public string DisplayName { get; set; } 
        public string? Description { get; set; } = ""; // توضیحات

    }
}
