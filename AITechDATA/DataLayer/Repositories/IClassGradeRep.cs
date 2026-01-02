using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AITechDATA.Domain;
using AITechDATA.ResultObjects;

namespace AITechDATA.DataLayer.Repositories
{
    public interface IClassGradeRep
    {
        Task<ListResultObject<ClassGrade>> GetAllClassGradesAsync(string entityName = "", long ForeignKeyId = 0, int pageIndex = 1, int pageSize = 20, string searchText = "", string sortQuery = "");

        Task<RowResultObject<ClassGrade>> GetClassGradeByIdAsync(long ClassGradeId);

        Task<BitResultObject> AddClassGradeAsync(ClassGrade ClassGrade);
        Task<BitResultObject> EditClassGradeAsync(ClassGrade ClassGrade);

        Task<BitResultObject> RemoveClassGradeAsync(ClassGrade ClassGrade);
        Task<BitResultObject> RemoveClassGradeAsync(long ClassGradeId);
        Task<BitResultObject> ExistClassGradeAsync(long ClassGradeId);
    }
}