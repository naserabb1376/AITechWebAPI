using System.ComponentModel.DataAnnotations;

namespace AITechDATA.Models.City
{
    public class AddEditCiyRequestBody
    {
        public long ID { get; set; } = 0;

        [Display(Name = "نام شهر یا استان")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string CityName { get; set; }
        public long ParentId { get; set; } = 0;
        public bool DefaultCity { get; set; } = false;
       // public string? Description { get; set; }

    }
}
