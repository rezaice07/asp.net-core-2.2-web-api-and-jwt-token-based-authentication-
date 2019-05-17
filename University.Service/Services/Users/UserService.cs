using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using University.Data.Models;
using University.Service.Encriptions;

namespace University.Service.Users
{
    public class UserService : IUserService
    {
        #region Private Member
        private readonly UniversityDbContext db;

        #endregion

        #region Ctor

        public UserService(UniversityDbContext db)
        {
            this.db = db;
        }

        #endregion

        #region public Methods      

        public Task<User> GetDetailsById(int id)
        {
            var user = db.User
                .FirstOrDefault(d => d.Id == id);

            return Task.Run(()=>user);

        }

        public Task<User> GetDetailsByEmail(string email)
        {
            var user = db.User                
                .Include(i => i.UserRole)
                .FirstOrDefault(d => d.Email == email);

            return Task.Run(() => user);
        }

        public Task<bool> Add(User newUserModel)
        {
            var currentDate = DateTime.UtcNow;

            try
            {
                //create salt and password hash
                var randomPass = EncryptionService.GenerateRandomPassword(8);
                var salt = EncryptionService.CreateRandomSalt();
                var passwordHash = EncryptionService.HashPassword(randomPass, salt);

                newUserModel.PasswordHash = passwordHash;
                newUserModel.PasswordSalt = salt;
                newUserModel.CreatedDateUtc = currentDate;

                db.User.Add(newUserModel);
                db.SaveChanges();

                return Task.Run(() => true);
            }
            catch
            {
                return Task.Run(() => false);
            }
        }

        public Task<bool> Update(User user)
        {
            try
            {
                var upateUser = db.User.FirstOrDefault(d => d.Id == user.Id);

                if (upateUser == null)
                    return Task.Run(() => false);

                upateUser.FirstName = user.FirstName;
                upateUser.LastName = user.LastName;

                db.SaveChanges();

                return Task.Run(() => true);
            }
            catch
            {
                return Task.Run(() => false);
            }
        }        

        public Task<bool> Remove(User user)
        {
            try
            {
                var removeUser = db.User.FirstOrDefault(d => d.Id == user.Id);

                if (removeUser == null)
                    return Task.Run(() => false);
                removeUser.Status = user.Status;

                db.SaveChanges();

                return Task.Run(() => true);
            }
            catch
            {
                return Task.Run(() => false);
            }
        }

        public Task<bool> ResetPassword(User user)
        {
            try
            {
                var updateUser = db.User.FirstOrDefault(d => d.Id == user.Id);

                if (updateUser == null)
                    return Task.Run(() => false);

                var randomPass = EncryptionService.GenerateRandomPassword(8);
                var salt = EncryptionService.CreateRandomSalt();
                var passwordHash = EncryptionService.HashPassword(randomPass, salt);

                updateUser.PasswordHash = passwordHash;
                updateUser.PasswordSalt = salt;

                db.SaveChanges();

                return Task.Run(() => true);
            }
            catch
            {
                return Task.Run(() => false);
            }
        }

        #endregion
    }
}
