using CQRS.Core.Domain;
using Post.Commun.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Post.Cmd.Domain.Aggregates
{
    public class PostAggregate : AggregateRoot
    {
        private bool _active;
        private string _author;
        private readonly Dictionary<Guid, Tuple<String, string>> _comments = new();

        public bool active
        {
            get { return _active; }
            set { _active = value; }
        }
        public PostAggregate()
        {

        }
        public PostAggregate(Guid id, string author, string message)
        {
            RaiseEvent(new PostCreatedEvent
            {
                Id = id,
                Author = author,
                Message = message,
                DatePosted = DateTime.Now,
            });
        }
        public void Apply(PostCreatedEvent @event)
        {
            _id = @event.Id;
            _active = true;
            _author = @event.Author;
        }

        public void EditMessage(string message)
        {
            if (!_active)
            {
                throw new InvalidOperationException("you can not edit the message of a not active post!");
            }
            if (string.IsNullOrEmpty(message))
            {
                throw new InvalidOperationException($"the value of {nameof(message)} can not be null or empty");
            }
            RaiseEvent(new MessageUpdatedEvent { Id = _id, Message = message });
        }
        public void Apply(MessageUpdatedEvent @event)
        {
            _id = @event.Id;
        }
        public void LikePost()
        {
            if (!_active)
            {
                throw new InvalidOperationException("you can not like an inactive post!");
            }
            RaiseEvent(new PostLikedEvent { Id = _id, });
        }
        public void Apply(PostLikedEvent @event)
        {
            _id = @event.Id;
        }

        public void AddComment(string comment, string username)
        {
            if (!_active)
            {
                throw new InvalidOperationException("you can not comment a not active post!");
            }
            if (string.IsNullOrEmpty(comment))
            {
                throw new InvalidOperationException($"the value of {nameof(comment)} can not be null or empty");
            }
            RaiseEvent(new CommentAddedEvent
            {
                Id = _id,
                Comment = comment,
                Username = username,
                CommentDate = DateTime.Now,
                CommentId = Guid.NewGuid(),
            });
        }

        public void Apply(CommentAddedEvent @event)
        {
            _id = @event.Id;
            _comments.Add(@event.CommentId, new Tuple<string, string>(@event.Comment, @event.Username));
        }
        public void EditComment(Guid commentId, string comment, string username)
        {
            if (_active) { throw new InvalidOperationException(" you can not edit a not active post"); }
            if (!_comments[commentId].Item2.Equals(username, StringComparison.CurrentCultureIgnoreCase))
            {
                throw new InvalidOperationException("you not allowed");
            }
            RaiseEvent(new CommentUpdatedEvent
            {
                Id = _id,
                CommentId = commentId,
                Comment = comment,
                Username = username,
                EditDate = DateTime.Now,
            });
        }
        public void Apply(CommentUpdatedEvent @event)
        {
            _id = @event.Id;
            _comments[@event.CommentId]  = new Tuple<string, string> (@event.Comment, @event.Username);
        }
        public void RemoveComment(Guid commentId,string username)
        {
            if(!_active) { throw new InvalidOperationException("you can not remove a comment of an inactive post"); }
            if (!_comments[commentId].Item2.Equals(username, StringComparison.CurrentCultureIgnoreCase))
            {
                throw new InvalidOperationException("you not allowed");
            }
            RaiseEvent(new CommentRemoveEvent { Id = _id, commentId = commentId });
        }
        public void Apply(CommentRemoveEvent @event)
        {
            _id = @event.Id;
            _comments.Remove(@event.commentId);
        }
        public void DeletePost(string username)
        {
            if (!_active)
            {
                throw new InvalidOperationException("post already has been remove");
            }
            if(!_author.Equals(username, StringComparison.CurrentCultureIgnoreCase))
            {
                throw new InvalidOperationException("not allowed");
            }
            RaiseEvent(new PostRemoveEvent
            {
                Id = _id,
            });

        }
        public void Apply(PostRemoveEvent @event)
        {
            _id = @event.Id;
            _active = false;
        }
    }
}
