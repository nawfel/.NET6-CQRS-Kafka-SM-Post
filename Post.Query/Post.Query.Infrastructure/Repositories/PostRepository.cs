using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Post.Query.Domain.Entities;
using Post.Query.Domain.Repositories;
using Post.Query.Infrastructure.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Post.Query.Infrastructure.Repositories
{
    public class PostRepository : IPostRepository
    {
        private readonly DatabaseContextFactory _contextFactory;

        public PostRepository(DatabaseContextFactory contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task CreateAsync(PostEntity post)
        {
            using DatabaseContext context = _contextFactory.CreateContext();

            context.Posts.Add(post);
            _ = await context.SaveChangesAsync();

        }

        public async Task DeleteAsync(Guid postId)
        {
            using DatabaseContext context = _contextFactory.CreateContext();
            var post = await GetByIdAsync(postId);

            if (post != null) return;
            context.Posts.Remove(post);
            _ = await context.SaveChangesAsync();
        }

        public async Task<PostEntity> GetByIdAsync(Guid postId)
        {
            using DatabaseContext context = _contextFactory.CreateContext();
            return await context.Posts
                .Include(p => p.Comments)
                .FirstOrDefaultAsync(x => x.PostId == postId);

        }

        public async Task<List<PostEntity>> ListAll()
        {
            using DatabaseContext context = _contextFactory.CreateContext();
            return await context.Posts
                .AsNoTracking()
                .Include(p => p.Comments)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<PostEntity>> ListByAthorAsync(string author)
        {
            using DatabaseContext context = _contextFactory.CreateContext();
            return await context.Posts
                .AsNoTracking()
                .Include(p => p.Comments)
                .Where(x => x.Author == author)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<PostEntity>> ListWithCommentAsync()
        {
            using DatabaseContext context = _contextFactory.CreateContext();
            return await context.Posts
                .AsNoTracking()
                .Include(p => p.Comments)
                .Where(x=>x.Comments !=null && x.Comments.Any())
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<PostEntity>> ListWithLikesAsync(int numberOfLikes)
        {
            using DatabaseContext context = _contextFactory.CreateContext();
            return await context.Posts
                .AsNoTracking()
                .Include(p => p.Comments)
                .Where(x => x.Likes>=numberOfLikes)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task UpdateAsync(PostEntity post)
        {
            using DatabaseContext context = _contextFactory.CreateContext();
            context.Posts.Add(post);
           _= await context.SaveChangesAsync();
        }
    }
}
