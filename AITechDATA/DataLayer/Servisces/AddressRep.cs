using AITechDATA.DataLayer.Repositories;
using AITechDATA.Domain;
using AITechDATA.ResultObjects;
using Microsoft.EntityFrameworkCore;
using AITechDATA.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AITechDATA.DataLayer.Servisces
{
    public class AddressRep : IAddressRep
    {
        private AiITechContext _context;

        public AddressRep()
        {
            _context = DbTools.GetDbContext();
        }

        public async Task<BitResultObject> AddAddressAsync(Address address)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                await _context.Addresses.AddAsync(address);
                await _context.SaveChangesAsync();
                result.ID = address.ID;
                _context.Entry(address).State = EntityState.Detached;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> EditAddressAsync(Address address)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                _context.Addresses.Update(address);
                await _context.SaveChangesAsync();
                result.ID = address.ID;
                _context.Entry(address).State = EntityState.Detached;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> ExistAddressAsync(long addressId)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                result.Status = await _context.Addresses
                    .AsNoTracking()
                    .AnyAsync(x => x.ID == addressId);
                result.ID = addressId;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<ListResultObject<Address>> GetAllAddressesAsync(int pageIndex = 1, int pageSize = 20, string searchText = "",string sortQuery ="")
        {
            ListResultObject<Address> results = new ListResultObject<Address>();
            try
            {
                var query = _context.Addresses
                    .AsNoTracking()
                    .Where(x =>
                        (!string.IsNullOrEmpty(x.Street) && x.Street.Contains(searchText)) ||
                        (!string.IsNullOrEmpty(x.City) && x.City.Contains(searchText)) ||
                        (!string.IsNullOrEmpty(x.State) && x.State.Contains(searchText)) ||
                        (!string.IsNullOrEmpty(x.PostalCode) && x.PostalCode.Contains(searchText))
                    );

                results.TotalCount = query.Count();
                results.PageCount = DbTools.GetPageCount(results.TotalCount, pageSize);
                results.Results = await query.OrderByDescending(x => x.CreateDate)
                     .SortBy(sortQuery).ToPaging(pageIndex, pageSize)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                results.Status = false;
                results.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return results;
        }

        public async Task<RowResultObject<Address>> GetAddressByIdAsync(long addressId)
        {
            RowResultObject<Address> result = new RowResultObject<Address>();
            try
            {
                result.Result = await _context.Addresses
                    .AsNoTracking()
                    .SingleOrDefaultAsync(x => x.ID == addressId);
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> RemoveAddressAsync(Address address)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                _context.Addresses.Remove(address);
                await _context.SaveChangesAsync();
                result.ID = address.ID;
                _context.Entry(address).State = EntityState.Detached;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> RemoveAddressAsync(long addressId)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                var address = await GetAddressByIdAsync(addressId);
                result = await RemoveAddressAsync(address.Result);
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }
    }
}