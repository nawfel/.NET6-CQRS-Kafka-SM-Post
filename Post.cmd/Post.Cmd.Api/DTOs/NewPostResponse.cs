using Post.Commun.DTOs;
using System.Reflection.Metadata.Ecma335;

namespace Post.Cmd.Api.DTOs
{
    public class NewPostResponse :BaseResponse
    {
        public Guid Id { get; set; }
    }
}
