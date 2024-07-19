namespace DotNet_WebAPI.Dtos
{
    public partial class AddPostDto
    {
        public string PostTitle { get; set; } = string.Empty;
        public string PostContent { get; set; } = string.Empty;
    }

    public partial class EditPostDto
    {
        public int PostId { get; set; }
        public string PostTitle { get; set; } = string.Empty;
        public string PostContent { get; set; } = string.Empty;
    }
}
