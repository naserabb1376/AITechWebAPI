using AITechDATA.Domain;

namespace AITechWebAPI.ViewModels
{
    public class CityVM : BaseEntity
    {
        public long CityParentID { get; set; }
        public string CityName { get; set; }
        public bool DefaultCity { get; set; }
    }
}