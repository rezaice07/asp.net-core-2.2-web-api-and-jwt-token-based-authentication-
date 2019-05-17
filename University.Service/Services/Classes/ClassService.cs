using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using University.Core.Filters;
using University.Data.Models;
using static University.Core.Utilities.AppConstants;

namespace University.Service.Classes
{
    public class ClassService : IClassService
    {
        #region Private Member
        private readonly UniversityDbContext db;

        #endregion

        #region Ctor

        public ClassService(UniversityDbContext db)
        {
            this.db = db;
        }

        #endregion

        #region public Methods     

        public Task<IEnumerable<Class>> GetListByFilter(ClassSearchFilter filter)
        {
            List<Class> classList = new List<Class>();

            var query = db.Class
                            .Where(f =>
                            (f.Status != StatusConstants.Deleted) &&
                            (filter.Status == null || filter.Status == string.Empty || f.Status == Convert.ToInt32(filter.Status)) &&
                            (filter.SearchTerm == string.Empty || f.Name.Contains(filter.SearchTerm.Trim())));

            filter.TotalCount = query.Count();

            //sorting 
            Func<Class, object> OrderByStringField = null;

            switch (filter.SortColumn)
            {
                case "Name":
                    OrderByStringField = p => p.Name;
                    break;
                case "Capacity":
                    OrderByStringField = p => p.Capacity;
                    break;
                default:
                    OrderByStringField = p => p.Name;
                    break;
            }
            //end sorting  

            var finalQuery = filter.SortDirection == "ASC" ? query.OrderBy(OrderByStringField) : query.OrderByDescending(OrderByStringField);

            classList = finalQuery.Skip((filter.PageNumber - 1) * filter.PageSize)
                                        .Take(filter.PageSize)
                                        .AsParallel()
                                        .ToList();
            return Task.Run(()=> classList.AsEnumerable());
        }

        public Task<Class> GetDetailsById(int id)
        {
            var singleClass = db.Class
                .FirstOrDefault(d => d.Id == id);

            return Task.Run(() => singleClass);

        }

        public Task<bool> ValidateClassSeatCapacity(int classId)
        {
            var validateClass = db.Class
                .Any(d => d.Status == StatusConstants.Active
                      && d.Id == classId
                      && d.Capacity > 0);

            return Task.Run(() => validateClass);
        }

        public Task<bool> ValidateStudentClassEnrollment(int classId, int studentId)
        {
            var validateStudentClassEnrollment = db.StudentEnrolledClass
                .Any(d => d.ClassId == classId
                      && d.UserId == studentId);

            return Task.Run(() => !validateStudentClassEnrollment);
        }

        public Task<bool> Add(Class newClassModel)
        {
            try
            {
                db.Class.Add(newClassModel);
                db.SaveChanges();
                return Task.Run(() => true);
            }
            catch
            {
                return Task.Run(() => false);
            }
        }

        public Task<bool> Update(Class updatedClassModel)
        {
            try
            {
                var upateClass = db.Class.FirstOrDefault(d => d.Id == updatedClassModel.Id);

                if (upateClass == null)
                    return Task.Run(() => false);

                upateClass.Name = updatedClassModel.Name;
                db.SaveChanges();

                return Task.Run(() => true);
            }
            catch
            {
                return Task.Run(() => false);
            }
        }

        public Task<bool> Remove(Class removeClassModel)
        {
            try
            {
                var removeClass = db.Class.FirstOrDefault(d => d.Id == removeClassModel.Id);

                if (removeClass == null)
                    return Task.Run(() => false);
                removeClass.Status = removeClass.Status;

                db.SaveChanges();

                return Task.Run(() => true);
            }
            catch
            {
                return Task.Run(() => false);
            }
        }

        public Task<bool> RegisterClass(StudentEnrolledClass newStudentEnrolledClassModel)
        {
            var currentDate = DateTime.UtcNow;
            bool isOperationSuccess = true;

            using (var ts = new TransactionScope())
            {
                try
                {                   
                    var updateCasss = db.Class
                        .FirstOrDefault(f => f.Status == StatusConstants.Active &&
                                            f.Id == newStudentEnrolledClassModel.ClassId);

                    if (updateCasss == null)
                        isOperationSuccess = false;

                    if(updateCasss.Capacity<=0)
                        isOperationSuccess = false;

                    if (isOperationSuccess)
                    {
                        //let's add into student enroll class
                        newStudentEnrolledClassModel.EnrolledDate = currentDate;
                        db.StudentEnrolledClass.Add(newStudentEnrolledClassModel);
                        db.SaveChanges();

                        //let's update student class seat Capacity
                        updateCasss.Capacity = updateCasss.Capacity - 1;
                        updateCasss.ChangedDateUtc = currentDate;
                        db.SaveChanges();
                    }
                }
                catch
                {
                    isOperationSuccess = false;
                }

                //to complete this transaction
                if (isOperationSuccess)
                    ts.Complete();

                ts.Dispose();
            }

            return Task.Run(()=>isOperationSuccess);
        }

        public Task<bool> DropRegisteredClass(StudentEnrolledClass studentEnrolledClassModel)
        {
            var currentDate = DateTime.UtcNow;
            bool isOperationSuccess = true;

            using (var ts = new TransactionScope())
            {
                try
                {
                    var updateCasss = db.Class
                        .FirstOrDefault(f => f.Status == StatusConstants.Active &&
                                            f.Id == studentEnrolledClassModel.ClassId);

                    if (updateCasss == null)
                        isOperationSuccess = false;                   

                    if (isOperationSuccess)
                    {
                        //let's remove this student enrolled class
                        var removeStudentEnrolledClass = db.StudentEnrolledClass
                            .FirstOrDefault(f => f.Id == studentEnrolledClassModel.Id);

                        db.StudentEnrolledClass.Remove(removeStudentEnrolledClass);
                        db.SaveChanges();

                        //let's update student class seat Capacity
                        updateCasss.Capacity = updateCasss.Capacity + 1;
                        updateCasss.ChangedDateUtc = currentDate;
                        db.SaveChanges();
                    }
                }
                catch
                {
                    isOperationSuccess = false;
                }

                //to complete this transaction
                if (isOperationSuccess)
                    ts.Complete();

                ts.Dispose();
            }

            return Task.Run(() => isOperationSuccess);
        }

        #endregion
    }
}
