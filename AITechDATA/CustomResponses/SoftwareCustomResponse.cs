using AITechDATA.Domain;
using AITechDATA.ResultObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AITechDATA.CustomResponses
{
    public class SoftwareListCustomResponse<T>:ListResultObject<T>
    {
        public Dictionary<T, List<Image>?>? ResultImages { get; set; }

    }

    public class SoftwareRowCustomResponse<T> : RowResultObject<T>
    {
        public Dictionary<T, List<Image>?>? ResultImages { get; set; }

    }
}

