using DotNet_WebAPI.Data;
using DotNet_WebAPI.Dtos;
using DotNet_WebAPI.Hellpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DotNet_WebAPI.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]

    public class AuthController : ControllerBase
    {
        private readonly DataContextDapper _dapper;
        private readonly AuthHelper _authHelper;
        public AuthController(IConfiguration config)
        {
            _dapper = new DataContextDapper(config);
            _authHelper = new AuthHelper(config);

        }

        [AllowAnonymous]
        [HttpPost("UsersRegister")]
        public IActionResult Register(UsersRegistrationDto userForRegistration)
        {
            if (userForRegistration.Password == userForRegistration.PasswordConfirm)
            {
                string sqlCheckUserExists = "SELECT Email FROM TutorialAppSchema.Auth WHERE Email = '" +
                    userForRegistration.Email + "'";

                IEnumerable<string> existingUsers = _dapper.LoadData<string>(sqlCheckUserExists);
                if (existingUsers.Count() == 0)
                {
                    UsersLoginDto userForSetPassword = new UsersLoginDto()
                    {
                        Email = userForRegistration.Email,
                        Password = userForRegistration.Password
                    };
                    if (_authHelper.SetPassword(userForSetPassword))
                    {

                        string sqlAddUser = @"EXEC TutorialAppSchema.spUser_Upsert
                            @FirstName = '" + userForRegistration.FirstName +
                            "', @LastName = '" + userForRegistration.LastName +
                            "', @Email = '" + userForRegistration.Email +
                            "', @Gender = '" + userForRegistration.Gender +
                            "', @Active = 1" +
                            ", @JobTitle = '" + userForRegistration.JobTitle +
                            "', @Department = '" + userForRegistration.Department +
                            "', @Salary = '" + userForRegistration.Salary + "'";
                        // string sqlAddUser = @"
                        //     INSERT INTO TutorialAppSchema.Users(
                        //         [FirstName],
                        //         [LastName],
                        //         [Email],
                        //         [Gender],
                        //         [Active]
                        //     ) VALUES (" +
                        //         "'" + userForRegistration.FirstName + 
                        //         "', '" + userForRegistration.LastName +
                        //         "', '" + userForRegistration.Email + 
                        //         "', '" + userForRegistration.Gender + 
                        //         "', 1)";
                        if (_dapper.ExecuteSql(sqlAddUser))
                        {
                            return Ok();
                        }
                        throw new Exception("Failed to add user.");
                    }
                    throw new Exception("Failed to register user.");
                }
                throw new Exception("User with this email already exists!");
            }
            throw new Exception("Passwords do not match!");
        }

        [AllowAnonymous]
        [HttpPost("UsersLogin")]
        public IActionResult UsersLogint(UsersLoginDto usersLoging)
        {
            string sqlForHashAndSalt = @"
                SELECT 
                    [Email],
                    [PasswordHash],
                    [PasswordSalt] 
                FROM TutorialAppSchema.Auth 
                WHERE Email = '" + usersLoging.Email + "' AND Active = 'Y'";

            UserLoginConfirmationDto userLoginConfirmation = _dapper
                .LoadDataSingle<UserLoginConfirmationDto>(sqlForHashAndSalt);

            byte[] passwordHash = _authHelper.GetPasswordHash(usersLoging.Password, userLoginConfirmation.PasswordSalt);

            //if (passwordHash == userLoginConfirmation.PasswordHash)
            //{

            //}

            for (int i = 0; i < passwordHash.Length; i++)
            {
                if (passwordHash[i] != userLoginConfirmation.PasswordHash[i])
                {
                    return StatusCode(401, "Incorrent password!");
                }
            }

            string userIdSql = @"
                SELECT UserId 
                FROM TutorialAppSchema.Users 
                WHERE Email = '" + usersLoging.Email + "' AND Active = 'Y'";

            int userId = _dapper.LoadDataSingle<int>(userIdSql);

            return Ok(new Dictionary<string, string> {
                {"token", _authHelper.CreateToken(userId)}
            });
        }

        [HttpGet("RefreshToken")]
        public IActionResult RefreshToken()
        {
            string userIdSql = @"
                SELECT UserId 
                FROM TutorialAppSchema.Users 
                WHERE UserId = '" + User.FindFirst("userId")?.Value + "'";

            int userId = _dapper.LoadDataSingle<int>(userIdSql);

            return Ok(new Dictionary<string, string> {
                {"token", _authHelper.CreateToken(userId)}
            });
        }

        [HttpPut("ResetPassword")]
        public IActionResult ResetPassword(UsersLoginDto userForSetPassword)
        {
            if (_authHelper.SetPassword(userForSetPassword))
            {
                return Ok();
            }
            throw new Exception("Failed to update password!");
        }
    }
}
