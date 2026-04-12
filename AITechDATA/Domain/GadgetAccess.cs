using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AITechDATA.Domain
{
    // GadgetAccess: جدول گجت‌ها
    public class GadgetAccess : BaseEntity
    {
        public string GadgetKey { get; set; }
        public string? GadgetUrl { get; set; }
        public string? GadgetDescription { get; set; }
        public string AccessUsername { get; set; }
        public string AccessPassword { get; set; }
        public DateTime? AccessStartDate { get; set; }
        public DateTime AccessEndDate { get; set; }

    }

    public class AccessableGadgetsDto
    {
        public string GadgetKey { get; set; }
        public string? GadgetUrl { get; set; }
        public string? GadgetDescription { get; set; }
    }
}