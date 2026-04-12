using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AITechDATA.Domain;
using AITechDATA.ResultObjects;

namespace AITechDATA.DataLayer.Repositories
{
    public interface IGadgetAccessRep
    {
        Task<ListResultObject<GadgetAccess>> GetAllGadgetAccessesAsync(string accessUsername="",string gadgetKey="", int pageIndex = 1, int pageSize = 20, string searchText = "",string sortQuery ="");
        Task<ListResultObject<AccessableGadgetsDto>> GetAcessableGadgetsAsync(string accessUsername, string accessPassword, string gadgetKey = "");
        Task<RowResultObject<GadgetAccess>> GetGadgetAccessByIdAsync(long GadgetAccessId);
        Task<RowResultObject<GadgetAccess>> AddGadgetAccessAsync(GadgetAccess GadgetAccess);
        Task<RowResultObject<GadgetAccess>> EditGadgetAccessAsync(GadgetAccess GadgetAccess);
        Task<BitResultObject> RemoveGadgetAccessAsync(GadgetAccess GadgetAccess);
        Task<BitResultObject> RemoveGadgetAccessAsync(long GadgetAccessId);
        Task<BitResultObject> ExistGadgetAccessAsync(long GadgetAccessId);
    }
}
