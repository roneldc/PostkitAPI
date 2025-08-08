using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Postkit.Infrastructure.Data;
using Postkit.Shared.Interfaces.Queries;
using Postkit.Shared.Models;
using Postkit.Tenant.Common;
using System.Linq.Expressions;

namespace Postkit.Comments.Queries
{
    public class CommentQueryBuilder : ICommentQueryBuilder
    {
        private readonly PostkitDbContext context;
        private readonly IHttpContextAccessor http;
        private IQueryable<Comment> query;

        public CommentQueryBuilder(PostkitDbContext context, IHttpContextAccessor http)
        {
            this.context = context;
            this.http = http;
            this.query = context.Comments.Where(c => c.TenantId == http.HttpContext.GetTenantId() && !c.IsDeleted);
        }

        private CommentQueryBuilder(IQueryable<Comment> query, PostkitDbContext context, IHttpContextAccessor http)
        {
            this.context = context;
            this.http = http;
            this.query = context.Comments.Where(c => c.TenantId == http.HttpContext.GetTenantId() && !c.IsDeleted);
        }

        public ICommentQueryBuilder WithUser()
        {
            query = query.Include(c => c.User);
            return this;
        }

        public ICommentQueryBuilder WithPost()
        {
            query = query.Include(c => c.Post);
            return this;
        }

        public ICommentQueryBuilder WithPostAndAuthor()
        {
            query = query.Include(c => c.Post)
                          .ThenInclude(p => p!.User);
            return this;
        }

        public ICommentQueryBuilder ByPostId(Guid postId)
        {
            query = query.Where(c => c.PostId == postId);
            return this;
        }

        public ICommentQueryBuilder ByPostIds(IEnumerable<Guid> postIds)
        {
            query = query.Where(c => postIds.Contains(c.PostId));
            return this;
        }

        public ICommentQueryBuilder ByUserId(string userId)
        {
            query = query.Where(c => c.UserId == userId);
            return this;
        }

        public ICommentQueryBuilder Search(string searchTerm)
        {
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(c => c.Content.Contains(searchTerm));
            }
            return this;
        }

        public ICommentQueryBuilder CreatedAfter(DateTime date)
        {
            query = query.Where(c => c.CreatedAt >= date);
            return this;
        }

        public ICommentQueryBuilder CreatedBefore(DateTime date)
        {
            query = query.Where(c => c.CreatedAt <= date);
            return this;
        }

        public ICommentQueryBuilder OrderByNewest()
        {
            query = query.OrderByDescending(c => c.CreatedAt);
            return this;
        }

        public ICommentQueryBuilder OrderByOldest()
        {
            query = query.OrderBy(c => c.CreatedAt);
            return this;
        }

        public ICommentQueryBuilder Skip(int count)
        {
            query = query.Skip(count);
            return this;
        }

        public ICommentQueryBuilder Take(int count)
        {
            query = query.Take(count);
            return this;
        }

        public ICommentQueryBuilder Paginate(int page, int pageSize)
        {
            query = query.Skip((page - 1) * pageSize).Take(pageSize);
            return this;
        }

        public ICommentQueryBuilder Where(Expression<Func<Comment, bool>> predicate)
        {
            query = query.Where(predicate);
            return this;
        }

        public ICommentQueryBuilder AsNoTracking()
        {
            query = query.AsNoTracking();
            return this;
        }

        // Execution methods
        public IQueryable<Comment> Build()
        {
            return query;
        }

        public async Task<List<Comment>> ToListAsync(CancellationToken cancellationToken = default)
        {
            return await query.ToListAsync(cancellationToken);
        }

        public async Task<Comment?> FirstOrDefaultAsync(CancellationToken cancellationToken = default)
        {
            return await query.FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<int> CountAsync(CancellationToken cancellationToken = default)
        {
            return await query.CountAsync(cancellationToken);
        }

        public IQueryable<TResult> Select<TResult>(Expression<Func<Comment, TResult>> selector)
        {
            return query.Select(selector);
        }
    }
}
