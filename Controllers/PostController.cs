using DotNet_WebAPI.Data;
using DotNet_WebAPI.Dtos;
using DotNet_WebAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DotNet_WebAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]

    public class PostController : ControllerBase
    {
        private readonly DataContextDapper _dapper;
        public PostController(IConfiguration config)
        {
            _dapper = new DataContextDapper(config);
        }

        [HttpGet("UserGetPosts")]
        public IEnumerable<PostModels> GetPosts()
        {
            string sql = @"
                SELECT [PostId],
                    [UserId],
                    [PostTitle],
                    [PostContent],
                    [PostCreated],
                    [PostUpdated] 
                FROM TutorialAppSchema.Posts";

            return _dapper.LoadData<PostModels>(sql);
        }


        [HttpGet("PostSingleByID/{postId}")]
        public PostModels GetPostSingle(int postId)
        {
            string sql = @"
                SELECT [PostId],
                    [UserId],
                    [PostTitle],
                    [PostContent],
                    [PostCreated],
                    [PostUpdated] 
                FROM TutorialAppSchema.Posts
                WHERE PostId = " + postId.ToString();

            return _dapper.LoadDataSingle<PostModels>(sql);
        }


        [HttpGet("PostsByUser/{userId}")]
        public IEnumerable<PostModels> GetPostsByUser(int userId)
        {
            string sql = @"
                SELECT [PostId],
                    [UserId],
                    [PostTitle],
                    [PostContent],
                    [PostCreated],
                    [PostUpdated] 
                FROM TutorialAppSchema.Posts
                WHERE UserId = " + userId.ToString();

            return _dapper.LoadData<PostModels>(sql);
        }


        [HttpGet("MyPosts")]
        public IEnumerable<PostModels> GetMyPosts()
        {
            string sql = @"
                SELECT [PostId],
                    [UserId],
                    [PostTitle],
                    [PostContent],
                    [PostCreated],
                    [PostUpdated] 
                FROM TutorialAppSchema.Posts
                WHERE UserId = " + this.User.FindFirst("userId")?.Value;

            return _dapper.LoadData<PostModels>(sql);
        }


        [HttpPost("UsersPost")]
        public IActionResult AddPost(AddPostDto postToAdd)
        {
            string sql = @"
            INSERT INTO TutorialAppSchema.Posts(
                [UserId],
                [PostTitle],
                [PostContent],
                [PostCreated],
                [PostUpdated]) 
            VALUES (
                " + this.User.FindFirst("userId")?.Value
                + ",'" + postToAdd.PostTitle
                + "','" + postToAdd.PostContent
                + "', GETDATE(), GETDATE() )";
            if (_dapper.ExecuteSql(sql))
            {
                return Ok();
            }

            throw new Exception("Failed to create new post!");
        }

        [HttpPut("UsersEditPost")]
        public IActionResult EditPost(EditPostDto postToEdit)
        {
            string sql = @"
                UPDATE TutorialAppSchema.Posts 
                SET PostContent = '" + postToEdit.PostContent +
                    "', PostTitle = '" + postToEdit.PostTitle +
                    @"', PostUpdated = GETDATE()
                WHERE PostId = " + postToEdit.PostId.ToString() + "AND UserId = " + this.User.FindFirst("userId")?.Value;

            if (_dapper.ExecuteSql(sql))
            {
                return Ok();
            }

            throw new Exception("Failed to edit post!");
        }

        [HttpDelete("UsersDeletePostByID/{postId}")]
        public IActionResult UsersDeletePostByID(int postId)
        {
            string sql = @"
                DELETE FROM TutorialAppSchema.Posts 
                WHERE PostId = " + postId.ToString() + "AND UserId = " + this.User.FindFirst("userId")?.Value;

            if (_dapper.ExecuteSql(sql))
            {
                return Ok();
            }

            throw new Exception("Failed to delete post!");
        }

        [HttpGet("PostsBySearch/{searchParam}")]
        public IEnumerable<PostModels> PostsBySearch(string searchParam)
        {
            string sql = @"
                SELECT [PostId],
                    [UserId],
                    [PostTitle],
                    [PostContent],
                    [PostCreated],
                    [PostUpdated] 
                FROM TutorialAppSchema.Posts
                WHERE PostTitle LIKE '%" + searchParam + "%'" + " OR PostContent LIKE '%" + searchParam + "%'";

            return _dapper.LoadData<PostModels>(sql);
        }
    }
}
