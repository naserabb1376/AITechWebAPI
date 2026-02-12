using AITechDATA.Domain;
using AITechDATA.ResultObjects;
using MTPermissionCenter.EFCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AITechDATA.DataLayer.Repositories
{
    public interface IFieldInFormRep
    {
        Task<ListResultObject<FieldInForm>> GetAllFieldInFormsAsync(long FormId = 0, long FormFieldId = 0, int pageIndex = 1, int pageSize = 20, string searchText = "", string sortQuery = "");

        Task<RowResultObject<FieldInForm>> GetFieldInFormByIdAsync(long FieldInFormId);

        Task<BitResultObject> AddFieldInFormsAsync(List<FieldInForm> FieldInForms);

        Task<BitResultObject> EditFieldInFormsAsync(List<FieldInForm> FieldInForms);

        Task<BitResultObject> RemoveFieldInFormsAsync(List<FieldInForm> FieldInForms);

        Task<BitResultObject> RemoveFieldInFormsAsync(List<long> FieldInFormIds);

        Task<BitResultObject> ExistFieldInFormAsync(long FieldInFormId);
    }
}