using Postkit.API.Models;

namespace Postkit.API.Helpers
{
    public static class EntityQueryExtensions
    {
        public static IQueryable<Post> FilterByApp(this IQueryable<Post> query, Guid appId)
            => query.Where(p => p.AppId == appId);

        public static IQueryable<Comment> FilterByApp(this IQueryable<Comment> query, Guid appId)
            => query.Where(c => c.AppId == appId);

        public static IQueryable<Reaction> FilterByApp(this IQueryable<Reaction> query, Guid appId)
            => query.Where(r => r.AppId == appId);

        public static IQueryable<ApplicationUser> FilterByApp(this IQueryable<ApplicationUser> query, Guid appId)
            => query.Where(u => u.AppId == appId);
    }
}