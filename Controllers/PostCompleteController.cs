//using DotNet_WebAPI.Data;
//using DotNet_WebAPI.Models;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;

//namespace DotNet_WebAPI.Controllers
//{
//    [Authorize]
//    [ApiController]
//    [Route("[controller]")]
//    public class PostCompleteController : ControllerBase
//    {
//        DataContextDapper _dapper;
//        public PostCompleteController(IConfiguration config)
//        {
//            _dapper = new DataContextDapper(config);
//        }
//        [HttpGet("Posts/{postId}/{userId}/{searchParam}")]
//        public IEnumerable<PostModels> GetPosts(int postId = 0, int userId = 0, string searchParam = "None")
//        {
//            string sql = @"EXEC TutorialAppSchema.spPosts_Get";
//            string parameters = "";

//            if (postId != 0)
//            {
//                parameters += ", @PostId=" + postId.ToString();
//            }
//            if (userId != 0)
//            {
//                parameters += ", @UserId=" + userId.ToString();
//            }
//            if (searchParam.ToLower() != "none")
//            {
//                parameters += ", @SearchValue='" + searchParam + "'";
//            }

//            if (parameters.Length > 0)
//            {
//                sql += parameters.Substring(1);
//            }

//            return _dapper.LoadData<PostModels>(sql);
//        }

//        [HttpGet("MyPosts")]
//        public IEnumerable<PostModels> GetMyPosts()
//        {
//            string sql = @"EXEC TutorialAppSchema.spPosts_Get @UserId = " +
//                this.User.FindFirst("userId")?.Value;

//            return _dapper.LoadData<PostModels>(sql);
//        }

//        [HttpPut("UpsertPost")]
//        public IActionResult UpsertPost(PostModels postToUpsert)
//        {
//            string sql = @"EXEC TutorialAppSchema.spPosts_Upsert
//                @UserId =" + this.User.FindFirst("userId")?.Value +
//                ", @PostTitle ='" + postToUpsert.PostTitle +
//                "', @PostContent ='" + postToUpsert.PostContent + "'";

//            if (postToUpsert.PostId > 0)
//            {
//                sql += ", @PostId = " + postToUpsert.PostId;
//            }

//            if (_dapper.ExecuteSql(sql))
//            {
//                return Ok();
//            }

//            throw new Exception("Failed to upsert post!");
//        }


//        [HttpDelete("Post/{postId}")]
//        public IActionResult DeletePost(int postId)
//        {
//            string sql = @"EXEC TutorialAppSchema.spPost_Delete @PostId = " +
//                    postId.ToString() +
//                    ", @UserId = " + this.User.FindFirst("userId")?.Value;


//            if (_dapper.ExecuteSql(sql))
//            {
//                return Ok();
//            }

//            throw new Exception("Failed to delete post!");
//        }
//    }
//}
