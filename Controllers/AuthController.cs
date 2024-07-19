using DotNet_WebAPI.Data;
using DotNet_WebAPI.Dtos;
using DotNet_WebAPI.Hellpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Security.Cryptography;

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
        public IActionResult UserRegister(UsersRegistrationDto usersRegistration)
        {
            if (usersRegistration.Password == usersRegistration.PasswordConfirm)
            {
                string sqlCheckEmailUser = @"
                    SELECT Email FROM TutorialAppSchema.Auth WHERE Email = '" + usersRegistration.Email + "' AND Active = 'Y'";

                IEnumerable<string> EmailUsers = _dapper.LoadData<string>(sqlCheckEmailUser);

                if (EmailUsers.Count() == 0)
                {
                    byte[] passwordSalt = new byte[128 / 8];
                    using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
                    {
                        rng.GetNonZeroBytes(passwordSalt);
                    }

                    byte[] passwordHash = _authHelper.GetPasswordHash(usersRegistration.Password, passwordSalt);

                    string sqlAddAuth = @"
                        INSERT INTO TutorialAppSchema.Auth(
                            [Email],
                            [PasswordHash], 
                            [PasswordSalt]) 
                        VALUES (
                        '" + usersRegistration.Email + "'" +
                        ", @PasswordHash, @PasswordSalt)";

                    List<SqlParameter> sqlParams = new List<SqlParameter>();

                    SqlParameter passwordSaltParam = new SqlParameter("@PasswordSalt", System.Data.SqlDbType.VarBinary);
                    passwordSaltParam.Value = passwordSalt;

                    SqlParameter passwordHashParam = new SqlParameter("@PasswordHash", System.Data.SqlDbType.VarBinary);
                    passwordHashParam.Value = passwordHash;

                    sqlParams.Add(passwordSaltParam);
                    sqlParams.Add(passwordHashParam);

                    if (_dapper.ExecuteSqlWithParameters(sqlAddAuth, sqlParams))
                    {
                        string sqlAddUsers = @"
                            INSERT INTO TutorialAppSchema.Users (
                                [FirstName],
                                [LastName],
                                [Email],
                                [Gender] )
                            VALUES (" +
                                "'" + usersRegistration.FirstName +
                                "', '" + usersRegistration.LastName +
                                "', '" + usersRegistration.Email +
                                "', '" + usersRegistration.Gender +
                                "')";
                        if (_dapper.ExecuteSql(sqlAddUsers))
                        {
                            return Ok();
                        }
                        throw new Exception("Failed to Add user!");
                    }
                    throw new Exception("Failed to register user!");
                }
                throw new Exception("User with this email already exists!");
            }
            throw new Exception("Passwords do not macth!");
        }

        [AllowAnonymous]
        [HttpPost("UsersLogin")]
        public IActionResult UsersLogint(UsersLogingDto usersLoging)
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
    }
}
