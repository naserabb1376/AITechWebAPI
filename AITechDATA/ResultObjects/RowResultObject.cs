using AITechDATA.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AITechDATA.ResultObjects
{
    public class RowResultObject<T>
    {
        public bool Status { get; set; } = true;
        public string ErrorMessage { get; set; } = "";
        public T Result { get; set; }
        public Image Image { get; set; } = new Image();
    }
}
