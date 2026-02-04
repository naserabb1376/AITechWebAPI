using AITechDATA.Domain;

namespace AITechWebAPI.ViewModels
{ 
    public class UserPermissionVM : BaseVM
    {
        public long UserId { get; set; }
        public long PermissionId { get; set; }

        public bool IsGranted { get; set; } = true; // allow/deny override
        public bool OwnerOnly { get; set; }

    }
}