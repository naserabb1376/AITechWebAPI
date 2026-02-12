using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AITechDATA.Domain;
using AITechDATA.ResultObjects;

namespace AITechDATA.DataLayer.Repositories
{
    public interface IFormFieldRep
    {
        Task<ListResultObject<FormField>> GetAllFormFieldsAsync(int pageIndex = 1, int pageSize = 20, string searchText = "", string sortQuery = "");
        Task<RowResultObject<FormField>> GetFormFieldByIdAsync(long FormFieldId);
        Task<BitResultObject> AddFormFieldAsync(FormField FormField);
        Task<BitResultObject> EditFormFieldAsync(FormField FormField);
        Task<BitResultObject> RemoveFormFieldAsync(FormField FormField);
        Task<BitResultObject> RemoveFormFieldAsync(long FormFieldId);
        Task<BitResultObject> ExistFormFieldAsync(long FormFieldId);
    }
}