using System.Collections.Generic;
using System.Threading.Tasks;
using University.Core.Filters;
using University.Data.Models;

namespace University.Service.Classes
{
    public interface IClassService
    {
        Task<IEnumerable<Class>> GetListByFilter(ClassSearchFilter filter);
        Task<Class> GetDetailsById(int id);
        Task<bool> ValidateClassSeatCapacity(int classId);
        Task<bool> ValidateStudentClassEnrollment(int classId, int studentId);
        Task<bool> Add(Class newClassModel);
        Task<bool> Update(Class CclassModel);
        Task<bool> Remove(Class updateClassModel);
        Task<bool> RegisterClass(StudentEnrolledClass newStudentEnrolledClassModel);
        Task<bool> DropRegisteredClass(StudentEnrolledClass studentEnrolledClassModel);
    }
}
