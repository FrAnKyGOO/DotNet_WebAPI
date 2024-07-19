using DotNet_WebAPI.Models;

namespace DotNet_WebAPI.Data
{
    public class UserRepository : IUserRepository
    {
        DataContextEF _entityFramework;

        public UserRepository(IConfiguration config)
        {
            _entityFramework = new DataContextEF(config);
        }

        public bool SavaChanges()
        {
            return _entityFramework.SaveChanges() > 0;
        }

        public void AddEntity<T>(T entityToAdd)
        {
            if (entityToAdd != null)
            {
                _entityFramework.Add(entityToAdd);
            }
        }

        public void RemoveEntity<T>(T entityToAdd)
        {
            if (entityToAdd != null)
            {
                _entityFramework.Remove(entityToAdd);
            }
        }

        public IEnumerable<UsersModels> GetUsers()
        {
            IEnumerable<UsersModels> Users = _entityFramework.Users.ToList<UsersModels>();
            return Users;
        }

        public UsersModels GetSingleUsers(int UserId)
        {

            UsersModels? Users = _entityFramework.Users
                .Where(u => u.UserId == UserId)
                .FirstOrDefault<UsersModels>();

            if (Users == null)
            {
                return Users;
            }

            throw new Exception("Failed to Get User");

        }
        public UsersSalaryModels GetSingleUsersSalary(int UserId)
        {

            UsersSalaryModels? UsersSalary = _entityFramework.UsersSalary
                .Where(u => u.UserId == UserId)
                .FirstOrDefault<UsersSalaryModels>();

            if (UsersSalary == null)
            {
                return UsersSalary;
            }

            throw new Exception("Failed to Get User");

        }
        public UsersJobInfoModels GetSingleUsersJobInfo(int UserId)
        {

            UsersJobInfoModels? UsersJobInfo = _entityFramework.UsersJobInfo
                .Where(u => u.UserId == UserId)
                .FirstOrDefault<UsersJobInfoModels>();

            if (UsersJobInfo == null)
            {
                return UsersJobInfo;
            }

            throw new Exception("Failed to Get User");

        }
    }
}
