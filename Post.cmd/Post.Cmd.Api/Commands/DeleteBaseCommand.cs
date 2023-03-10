using CQRS.Core.Commands;

namespace Post.Cmd.Api.Commands
{
    public class DeleteBaseCommand :BaseCommand
    {
        public string Username { get; set; }
    }
}
