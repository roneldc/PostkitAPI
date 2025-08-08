using Postkit.Infrastructure.Data;
using Postkit.Shared.Models;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Postkit.Shared.Enum;
using Postkit.Shared.Interfaces.Queries;
using Microsoft.AspNetCore.Http;
using Postkit.Tenant.Common;

namespace Postkit.Reactions.Queries
{
    public class ReactionQueryBuilder : IReactionQueryBuilder
    {
        private readonly PostkitDbContext context;
        private readonly IHttpContextAccessor http;
        private IQueryable<Reaction> query;

        public ReactionQueryBuilder(PostkitDbContext context, IHttpContextAccessor http)
        {
            this.context = context;
            this.http = http;
            this.query = context.Reactions.Where(c => c.TenantId == http.HttpContext.GetTenantId());
        }

        private ReactionQueryBuilder(IQueryable<Reaction> query, PostkitDbContext context, IHttpContextAccessor http)
        {
            this.context = context;
            this.http = http;
            this.query = context.Reactions.Where(c => c.TenantId == http.HttpContext.GetTenantId());
        }

        public IReactionQueryBuilder WithUser()
        {
            query = query.Include(r => r.User);
            return this;
        }

        public IReactionQueryBuilder WithPost()
        {
            query = query.Include(r => r.Post);
            return this;
        }

        public IReactionQueryBuilder ByPostId(Guid postId)
        {
            query = query.Where(r => r.PostId == postId);
            return this;
        }

        public IReactionQueryBuilder ByPostIds(IEnumerable<Guid> postIds)
        {
            query = query.Where(r => postIds.Contains(r.PostId));
            return this;
        }

        public IReactionQueryBuilder ByUserId(string userId)
        {
            query = query.Where(r => r.UserId == userId);
            return this;
        }

        public IReactionQueryBuilder ByType(ReactionType type)
        {
            query = query.Where(r => r.Type == type);
            return this;
        }

        public IReactionQueryBuilder ByTypes(IEnumerable<ReactionType> types)
        {
            query = query.Where(r => types.Contains(r.Type));
            return this;
        }

        public IReactionQueryBuilder OrderByNewest()
        {
            query = query.OrderByDescending(r => r.CreatedAt);
            return this;
        }

        public IReactionQueryBuilder OrderByType()
        {
            query = query.OrderBy(r => r.Type);
            return this;
        }

        public IReactionQueryBuilder Skip(int count)
        {
            query = query.Skip(count);
            return this;
        }

        public IReactionQueryBuilder Take(int count)
        {
            query = query.Take(count);
            return this;
        }

        public IReactionQueryBuilder Paginate(int page, int pageSize)
        {
            query = query.Skip((page - 1) * pageSize).Take(pageSize);
            return this;
        }
        public IReactionQueryBuilder Where(Expression<Func<Reaction, bool>> predicate)
        {
            query = query.Where(predicate);
            return this;
        }

        public IReactionQueryBuilder AsNoTracking()
        {
            query = query.AsNoTracking();
            return this;
        }

        // Execution methods
        public IQueryable<Reaction> Build()
        {
            return query;
        }

        public async Task<List<Reaction>> ToListAsync(CancellationToken cancellationToken = default)
        {
            return await query.ToListAsync(cancellationToken);
        }

        public async Task<Reaction?> FirstOrDefaultAsync(CancellationToken cancellationToken = default)
        {
            return await query.FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<int> CountAsync(CancellationToken cancellationToken = default)
        {
            return await query.CountAsync(cancellationToken);
        }

        public IQueryable<TResult> Select<TResult>(Expression<Func<Reaction, TResult>> selector)
        {
            return query.Select(selector);
        }

        // Aggregation methods
        public async Task<Dictionary<ReactionType, int>> GroupByTypeAndCountAsync(CancellationToken cancellationToken = default)
        {
            return await query
                .GroupBy(r => r.Type)
                .ToDictionaryAsync(g => g.Key, g => g.Count(), cancellationToken);
        }
    }
}
