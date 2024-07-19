using DotNet_WebAPI.Data;
using DotNet_WebAPI.Dtos;
using DotNet_WebAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace DotNet_WebAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        DataContextDapper _dapper;
        IConfiguration _config;
        public UsersController(IConfiguration config)
        {
            _dapper = new DataContextDapper(config);
            _config = config;
        }

        [HttpGet("TestConnect")]
        public DateTime TestConnect()
        {
            return _dapper.LoadDataSingle<DateTime>("SELECT GETDATE()");
        }

        #region Users
        [HttpGet("GetUsers")]
        public IEnumerable<UsersModels> GetUsers()
        {
            string sql = @"
                SELECT [UserId]
                      ,[FirstName]
                      ,[LastName]
                      ,[Email]
                      ,[Gender]
                      ,[Active]
                  FROM TutorialAppSchema.Users";
            IEnumerable<UsersModels> Users = _dapper.LoadData<UsersModels>(sql);
            return Users;

        }

        [HttpGet("GetSingleUsers/{UserId}")]
        public UsersModels GetSingleUsers(int UserId)
        {
            string sql = @"
                SELECT [UserId]
                    ,[FirstName]
                    ,[LastName]
                    ,[Email]
                    ,[Gender]
                    ,[Active]
                FROM TutorialAppSchema.Users 
                WHERE UserId = " + UserId.ToString();

            UsersModels Users = _dapper.LoadDataSingle<UsersModels>(sql);
            return Users;
        }

        [HttpPut("EditDataUser")]
        public IActionResult EditDataUser(UsersModels user)
        {
            string sql = @"
                        UPDATE TutorialAppSchema.Users
                            SET [FirstName] = '" + user.FirstName +
                                "', [LastName] = '" + user.LastName +
                                "', [Email] = '" + user.Email +
                                "', [Gender] = '" + user.Gender +
                                "', [Active] = '" + user.Active +
                            "' WHERE UserId = " + user.UserId;
            if (_dapper.ExecuteSql(sql))
            {
                return Ok();
            }

            throw new Exception("Failed to Update User");
        }

        [HttpPost("AddUsers")]
        public IActionResult AddDataUser(UsersDto user)
        {
            string sql = @"
                        INSERT INTO TutorialAppSchema.Users (
                            [FirstName],
                            [LastName],
                            [Email],
                            [Gender],
                            [Active] )
                        VALUES
                            ('" + user.FirstName +
                            "', '" + user.LastName +
                            "', '" + user.Email +
                            "', '" + user.Gender +
                            "', '" + user.Active +
                         "')";

            if (_dapper.ExecuteSql(sql))
            {
                return Ok();
            }

            throw new Exception("Failed to Update User");
        }

        [HttpDelete("DeleteUserByID/{UserId}")]
        public IActionResult DeleteUserByID(int UserId)
        {
            string sql_Update_Active = @"
                        UPDATE TutorialAppSchema.Users
                        SET [Active] = 'False' 
                        WHERE UserId = " + UserId.ToString();

            string sql_Delete = @"
                        DELETE FROM TutorialAppSchema.Users
                        WHERE UserId = " + UserId.ToString();

            if (_dapper.ExecuteSql(sql_Update_Active))
            {
                return Ok();
            }

            throw new Exception("Failed to Delete User");
        }
        #endregion


        #region Users Salary

        [HttpGet("GetDataUserSalaryByID/{userId}")]
        public IEnumerable<UsersSalaryModels> GetUserSalary(int userId)
        {
            return _dapper.LoadData<UsersSalaryModels>(@"
            SELECT UserSalary.UserId
                    , UserSalary.Salary
            FROM  TutorialAppSchema.UserSalary
                WHERE UserId = " + userId.ToString());
        }

        [HttpPost("AddUserSalary")]
        public IActionResult PostUserSalary(UsersSalaryModels userSalaryForInsert)
        {
            string sql = @"
            INSERT INTO TutorialAppSchema.UserSalary (
                UserId,
                Salary
            ) VALUES (" + userSalaryForInsert.UserId.ToString()
                    + ", " + userSalaryForInsert.Salary
                    + ")";

            if (_dapper.ExecuteSqlWithRowCount(sql) > 0)
            {
                return Ok(userSalaryForInsert);
            }
            throw new Exception("Adding User Salary failed on save");
        }

        [HttpPut("UpdateDataUserSalaryByID")]
        public IActionResult PutUserSalary(UsersSalaryModels userSalaryForUpdate)
        {
            string sql = "UPDATE TutorialAppSchema.UserSalary SET Salary="
                + userSalaryForUpdate.Salary
                + " WHERE UserId=" + userSalaryForUpdate.UserId.ToString();

            if (_dapper.ExecuteSql(sql))
            {
                return Ok(userSalaryForUpdate);
            }
            throw new Exception("Updating User Salary failed on save");
        }

        [HttpDelete("DeleteDataUserSalaryByID/{userId}")]
        public IActionResult DeleteUserSalary(int userId)
        {
            string sql = "DELETE FROM TutorialAppSchema.UserSalary WHERE UserId=" + userId.ToString();

            if (_dapper.ExecuteSql(sql))
            {
                return Ok();
            }
            throw new Exception("Deleting User Salary failed on save");
        }
        #endregion


        #region Users JobInfo

        [HttpGet("GetDataUserJobInfoByID/{userId}")]
        public IEnumerable<UsersJobInfoModels> GetUserJobInfo(int userId)
        {
            return _dapper.LoadData<UsersJobInfoModels>(@"
            SELECT  UserJobInfo.UserId
                    , UserJobInfo.JobTitle
                    , UserJobInfo.Department
            FROM  TutorialAppSchema.UserJobInfo
                WHERE UserId = " + userId.ToString());
        }

        [HttpPost("AddDataUserJobInfo")]
        public IActionResult PostUserJobInfo(UsersJobInfoModels userJobInfoForInsert)
        {
            string sql = @"
            INSERT INTO TutorialAppSchema.UserJobInfo (
                UserId,
                Department,
                JobTitle
            ) VALUES (" + userJobInfoForInsert.UserId
                    + ", '" + userJobInfoForInsert.Department
                    + "', '" + userJobInfoForInsert.JobTitle
                    + "')";

            if (_dapper.ExecuteSql(sql))
            {
                return Ok(userJobInfoForInsert);
            }
            throw new Exception("Adding User Job Info failed on save");
        }

        [HttpPut("UpdateDataUserJobInfoByID")]
        public IActionResult PutUserJobInfo(UsersJobInfoModels userJobInfoForUpdate)
        {
            string sql = "UPDATE TutorialAppSchema.UserJobInfo SET Department='"
                + userJobInfoForUpdate.Department
                + "', JobTitle='"
                + userJobInfoForUpdate.JobTitle
                + "' WHERE UserId=" + userJobInfoForUpdate.UserId.ToString();

            if (_dapper.ExecuteSql(sql))
            {
                return Ok(userJobInfoForUpdate);
            }
            throw new Exception("Updating User Job Info failed on save");
        }

        // [HttpDelete("UserJobInfo/{userId}")]
        // public IActionResult DeleteUserJobInfo(int userId)
        // {
        //     string sql = "DELETE FROM TutorialAppSchema.UserJobInfo  WHERE UserId=" + userId;

        //     if (_dapper.ExecuteSql(sql))
        //     {
        //         return Ok();
        //     }
        //     throw new Exception("Deleting User Job Info failed on save");
        // }

        [HttpDelete("DelateDataUserJobInfoByID/{userId}")]
        public IActionResult DeleteUserJobInfo(int userId)
        {
            string sql = @"
            DELETE FROM TutorialAppSchema.UserJobInfo 
                WHERE UserId = " + userId.ToString();

            Console.WriteLine(sql);

            if (_dapper.ExecuteSql(sql))
            {
                return Ok();
            }

            throw new Exception("Failed to Delete User");
        }
        #endregion
    }
}
