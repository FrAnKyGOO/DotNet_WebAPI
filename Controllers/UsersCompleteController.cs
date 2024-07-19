using DotNet_WebAPI.Data;
using DotNet_WebAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace DotNet_WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersCompleteController : ControllerBase
    {
        DataContextDapper _dapper;
        public UsersCompleteController(IConfiguration config)
        {
            _dapper = new DataContextDapper(config);
        }

        [HttpGet("GetUsers/{userId}/{isActive}")]
        // public IEnumerable<User> GetUsers()
        public IEnumerable<UsersCompleteModels> GetUsers(int userId, bool isActive)
        {
            string sql = @"EXEC TutorialAppSchema.spUsers_Get";
            string parameters = "";

            if (userId != 0)
            {
                parameters += ", @UserId=" + userId.ToString();
            }
            if (isActive)
            {
                parameters += ", @Active=" + isActive.ToString();
            }

            if (parameters.Length > 0)
            {
                sql += parameters.Substring(1);//, parameters.Length);
            }

            IEnumerable<UsersCompleteModels> users = _dapper.LoadData<UsersCompleteModels>(sql);
            return users;
        }

        [HttpPut("UpsertUser")]
        public IActionResult UpsertUser(UsersCompleteModels user)
        {
            string sql = @"EXEC TutorialAppSchema.spUser_Upsert
            @FirstName = '" + user.FirstName +
                "', @LastName = '" + user.LastName +
                "', @Email = '" + user.Email +
                "', @Gender = '" + user.Gender +
                "', @Active = '" + user.Active +
                "', @JobTitle = '" + user.JobTitle +
                "', @Department = '" + user.Department +
                "', @Salary = '" + user.Salary +
                "', @UserId = " + user.UserId;

            if (_dapper.ExecuteSql(sql))
            {
                return Ok();
            }

            throw new Exception("Failed to Update User");
        }

        [HttpDelete("DeleteUser/{userId}")]
        public IActionResult DeleteUser(int userId)
        {
            string sql = @"TutorialAppSchema.spUser_Delete
            @UserId = " + userId.ToString();

            if (_dapper.ExecuteSql(sql))
            {
                return Ok();
            }

            throw new Exception("Failed to Delete User");
        }
    }
}
