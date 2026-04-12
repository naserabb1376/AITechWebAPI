

using AITechDATA.Domain;
using System.ComponentModel.DataAnnotations.Schema;

namespace AITechWebAPI.ViewModels
{
    // GadgetAccess: جدول گجت‌ها
    public class GadgetAccessVM : BaseVM
    {
        public string GadgetKey { get; set; }
        public string? GadgetUrl { get; set; }
        public string? GadgetDescription { get; set; }
        public string AccessUsername { get; set; }
        public string AccessPassword { get; set; }
        public DateTime? AccessStartDate { get; set; }
        public DateTime AccessEndDate { get; set; }

    }
}