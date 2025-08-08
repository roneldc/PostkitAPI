using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Postkit.Infrastructure.Data;
using Postkit.Shared.Interfaces.Queries;
using Postkit.Shared.Models;
using Postkit.Tenant.Common;
using System.Linq.Expressions;

namespace Postkit.Posts.Queries
{
    public class PostQueryBuilder : IPostQueryBuilder
    {
        private readonly PostkitDbContext context;
        private readonly IHttpContextAccessor http;
        private IQueryable<Post> query;

        public PostQueryBuilder(PostkitDbContext context, IHttpContextAccessor http)
        {
            this.context = context;
            this.http = http;
            query = context.Posts.Where(p => p.TenantId == http.HttpContext.GetTenantId() && !p.IsDeleted);
        }

        public IPostQueryBuilder WithAuthor()
        {
            query = query.Include(p => p.User);
            return this;
        }

        public IPostQueryBuilder WithComments(int? limit = null)
        {
            if (limit.HasValue)
            {
                query = query.Include(p => p.Comments.OrderByDescending(c => c.CreatedAt).Take(limit.Value))
                              .ThenInclude(c => c.User);
            }
            else
            {
                query = query.Include(p => p.Comments)
                              .ThenInclude(c => c.User);
            }
            return this;
        }

        public IPostQueryBuilder WithReactions()
        {
            query = query.Include(p => p.Reactions)
                          .ThenInclude(r => r.User);
            return this;
        }

        public IPostQueryBuilder ByUser(string userId)
        {
            query = query.Where(p => p.UserId == userId);
            return this;
        }

        public IPostQueryBuilder ById(Guid postId)
        {
            query = query.Where(p => p.Id == postId);
            return this;
        }

        public IPostQueryBuilder ByIds(IEnumerable<Guid> postIds)
        {
            query = query.Where(p => postIds.Contains(p.Id));
            return this;
        }

        public IPostQueryBuilder Search(string searchTerm)
        {
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(p => p.Title != null && p.Title.Contains(searchTerm) 
                || p.Content.Contains(searchTerm));
            }
            return this;
        }

        public IPostQueryBuilder CreatedAfter(DateTime date)
        {
            query = query.Where(p => p.CreatedAt >= date);
            return this;
        }

        public IPostQueryBuilder CreatedBefore(DateTime date)
        {
            query = query.Where(p => p.CreatedAt <= date);
            return this;
        }

        public IPostQueryBuilder CreatedBetween(DateTime startDate, DateTime endDate)
        {
            query = query.Where(p => p.CreatedAt >= startDate && p.CreatedAt <= endDate);
            return this;
        }

        public IPostQueryBuilder WithMinimumReactions(int minCount)
        {
            query = query.Where(p => p.Reactions.Count >= minCount);
            return this;
        }

        public IPostQueryBuilder WithMinimumComments(int minCount)
        {
            query = query.Where(p => p.Comments.Count >= minCount);
            return this;
        }

        public IPostQueryBuilder OrderByNewest()
        {
            query = query.OrderByDescending(p => p.CreatedAt);
            return this;
        }

        public IPostQueryBuilder OrderByOldest()
        {
            query = query.OrderBy(p => p.CreatedAt);
            return this;
        }

        public IPostQueryBuilder OrderByMostReactions()
        {
            query = query.OrderByDescending(p => p.Reactions.Count);
            return this;
        }

        public IPostQueryBuilder OrderByMostComments()
        {
            query = query.OrderByDescending(p => p.Comments.Count);
            return this;
        }

        public IPostQueryBuilder OrderByTitle()
        {
            query = query.OrderBy(p => p.Title);
            return this;
        }

        public IPostQueryBuilder ThenByNewest()
        {
            query = ((IOrderedQueryable<Post>)query).ThenByDescending(p => p.CreatedAt);
            return this;
        }

        public IPostQueryBuilder ThenByTitle()
        {
            query = ((IOrderedQueryable<Post>)query).ThenBy(p => p.Title);
            return this;
        }

        public IPostQueryBuilder Skip(int count)
        {
            query = query.Skip(count);
            return this;
        }

        public IPostQueryBuilder Take(int count)
        {
            query = query.Take(count);
            return this;
        }

        public IPostQueryBuilder Paginate(int page, int pageSize)
        {
            query = query.Skip((page - 1) * pageSize).Take(pageSize);
            return this;
        }

        public IPostQueryBuilder Where(Expression<Func<Post, bool>> predicate)
        {
            query = query.Where(predicate);
            return this;
        }

        public IPostQueryBuilder AsSplitQuery()
        {
            query = query.AsSplitQuery();
            return this;
        }

        public IPostQueryBuilder AsNoTracking()
        {
            query = query.AsNoTracking();
            return this;
        }

        // Execution methods
        public IQueryable<Post> Build()
        {
            return query;
        }

        public async Task<List<Post>> ToListAsync(CancellationToken cancellationToken = default)
        {
            return await query.ToListAsync(cancellationToken);
        }

        public async Task<Post?> FirstOrDefaultAsync(CancellationToken cancellationToken = default)
        {
            return await query.FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<Post> FirstAsync(CancellationToken cancellationToken = default)
        {
            return await query.FirstAsync(cancellationToken);
        }

        public async Task<bool> AnyAsync(CancellationToken cancellationToken = default)
        {
            return await query.AnyAsync(cancellationToken);
        }

        public async Task<int> CountAsync(CancellationToken cancellationToken = default)
        {
            return await query.CountAsync(cancellationToken);
        }

        public async Task<long> LongCountAsync(CancellationToken cancellationToken = default)
        {
            return await query.LongCountAsync(cancellationToken);
        }

        // Projection methods
        public IQueryable<TResult> Select<TResult>(Expression<Func<Post, TResult>> selector)
        {
            return query.Select(selector);
        }

        public async Task<List<TResult>> SelectToListAsync<TResult>(
            Expression<Func<Post, TResult>> selector,
            CancellationToken cancellationToken = default)
        {
            return await query.Select(selector).ToListAsync(cancellationToken);
        }
    }
}
