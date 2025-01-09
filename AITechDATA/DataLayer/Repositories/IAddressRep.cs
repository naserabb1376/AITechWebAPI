using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AITechDATA.Domain;
using AITechDATA.ResultObjects;

namespace AITechDATA.DataLayer.Repositories
{
    public interface IAddressRep
    {
        Task<ListResultObject<Address>> GetAllAddressesAsync(int pageIndex = 1, int pageSize = 20, string searchText = "");

        Task<RowResultObject<Address>> GetAddressByIdAsync(long addressId);

        Task<BitResultObject> AddAddressAsync(Address address);

        Task<BitResultObject> EditAddressAsync(Address address);

        Task<BitResultObject> RemoveAddressAsync(Address address);

        Task<BitResultObject> RemoveAddressAsync(long addressId);

        Task<BitResultObject> ExistAddressAsync(long addressId);
    }
}