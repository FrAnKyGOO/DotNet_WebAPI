using AutoMapper;
using DotNet_WebAPI.Data;
using DotNet_WebAPI.Dtos;
using DotNet_WebAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace DotNet_WebAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UsersEFController : Controller
    {
        IMapper _mapper;
        //DataContextEF _entityFramework;
        IUserRepository _userRepository;

        public UsersEFController(IConfiguration config, IUserRepository userRepository)
        {
            //_entityFramework = new DataContextEF(config);

            _mapper = new Mapper(new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<UsersDto, UsersDto>();
                cfg.CreateMap<UsersSalaryModels, UsersSalaryModels>().ReverseMap();
                cfg.CreateMap<UsersJobInfoModels, UsersJobInfoModels>().ReverseMap();
            }));
            _userRepository = userRepository;
        }

        #region Users

        [HttpGet("GetUsers")]
        public IEnumerable<UsersModels> GetUsers()
        {
            IEnumerable<UsersModels> Users = _userRepository.GetUsers();
            return Users;
        }

        [HttpGet("GetSingleUsers/{UserId}")]
        public UsersModels GetSingleUsers(int UserId)
        {

            //UsersModels? Users = _entityFramework.Users
            //    .Where(u => u.UserId == UserId)
            //    .FirstOrDefault<UsersModels>();

            //if (Users == null)
            //{
            //    return Users;
            //}

            //throw new Exception("Failed to Get User");
            return _userRepository.GetSingleUsers(UserId);
        }

        [HttpPut("EditDataUser")]
        public IActionResult EditDataUser(UsersModels user)
        {
            UsersModels? UserDb = _userRepository.GetSingleUsers(user.UserId);

            if (UserDb != null)
            {

                UserDb.Active = user.Active;
                UserDb.FirstName = user.FirstName;
                UserDb.LastName = user.LastName;
                UserDb.Gender = user.Gender;
                UserDb.Email = user.Email;
                if (_userRepository.SavaChanges())
                {
                    return Ok();
                }

                throw new Exception("Failed to Edit User");

            }
            throw new Exception("Failed to Edit User");

        }

        [HttpPost("AddUsers")]
        public IActionResult AddDataUser(UsersDto user)
        {
            UsersDto userDb = _mapper.Map<UsersDto>(user);

            _userRepository.AddEntity<UsersDto>(userDb);
            if (_userRepository.SavaChanges())
            {
                return Ok();
            }

            throw new Exception("Failed to Add User");
        }

        [HttpDelete("DeleteUserByID/{UserId}")]
        public IActionResult DeleteUserByID(int UserId)
        {
            UsersModels? userDb = _userRepository.GetSingleUsers(UserId);

            if (userDb != null)
            {
                _userRepository.RemoveEntity<UsersModels>(userDb);
                if (_userRepository.SavaChanges())
                {
                    return Ok();
                }

                throw new Exception("Failed to Delete User");
            }

            throw new Exception("Failed to Get User");
        }

        #endregion

        #region Users Salary
        [HttpGet("UserSalary/{userId}")]
        public UsersSalaryModels GetUserSalaryEF(int userId)
        {
            return _userRepository.GetSingleUsersSalary(userId);
        }

        [HttpPost("UserSalary")]
        public IActionResult PostUserSalaryEf(UsersSalaryModels userForInsert)
        {
            _userRepository.AddEntity<UsersSalaryModels>(userForInsert);
            if (_userRepository.SavaChanges())
            {
                return Ok();
            }
            throw new Exception("Adding UserSalary failed on save");
        }


        [HttpPut("UserSalary")]
        public IActionResult PutUserSalaryEf(UsersSalaryModels userForUpdate)
        {
            UsersSalaryModels? userToUpdate = _userRepository.GetSingleUsersSalary(userForUpdate.UserId);

            if (userToUpdate != null)
            {
                _mapper.Map(userForUpdate, userToUpdate);
                if (_userRepository.SavaChanges())
                {
                    return Ok();
                }
                throw new Exception("Updating UserSalary failed on save");
            }
            throw new Exception("Failed to find UserSalary to Update");
        }


        [HttpDelete("UserSalary/{userId}")]
        public IActionResult DeleteUserSalaryEf(int userId)
        {
            UsersSalaryModels? userToDelete = _userRepository.GetSingleUsersSalary(userId);

            if (userToDelete != null)
            {
                _userRepository.RemoveEntity<UsersSalaryModels>(userToDelete);
                if (_userRepository.SavaChanges())
                {
                    return Ok();
                }
                throw new Exception("Deleting UserSalary failed on save");
            }
            throw new Exception("Failed to find UserSalary to delete");
        }
        #endregion

        #region Users JobInfo

        [HttpGet("UserJobInfo/{userId}")]
        public UsersJobInfoModels GetUserJobInfoEF(int userId)
        {
            return _userRepository.GetSingleUsersJobInfo(userId);
        }

        [HttpPost("UserJobInfo")]
        public IActionResult PostUserJobInfoEf(UsersJobInfoModels userForInsert)
        {
            _userRepository.AddEntity<UsersJobInfoModels>(userForInsert);
            if (_userRepository.SavaChanges())
            {
                return Ok();
            }
            throw new Exception("Adding UserJobInfo failed on save");
        }


        [HttpPut("UserJobInfo")]
        public IActionResult PutUserJobInfoEf(UsersJobInfoModels userForUpdate)
        {
            UsersJobInfoModels? userToUpdate = _userRepository.GetSingleUsersJobInfo(userForUpdate.UserId);

            if (userToUpdate != null)
            {
                _mapper.Map(userForUpdate, userToUpdate);
                if (_userRepository.SavaChanges())
                {
                    return Ok();
                }
                throw new Exception("Updating UserJobInfo failed on save");
            }
            throw new Exception("Failed to find UserJobInfo to Update");
        }


        [HttpDelete("UserJobInfo/{userId}")]
        public IActionResult DeleteUserJobInfoEf(int userId)
        {
            UsersJobInfoModels? userToDelete = _userRepository.GetSingleUsersJobInfo(userId);

            if (userToDelete != null)
            {
                _userRepository.RemoveEntity<UsersJobInfoModels>(userToDelete);
                if (_userRepository.SavaChanges())
                {
                    return Ok();
                }
                throw new Exception("Deleting UserJobInfo failed on save");
            }
            throw new Exception("Failed to find UserJobInfo to delete");
        }
        #endregion
    }
}
