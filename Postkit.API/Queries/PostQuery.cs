using Postkit.API.Models;

public class PostQuery
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;

    public Guid? AppId { get; set; }
    public string? Search { get; set; }
    public Guid? PostId { get; set; }
    public string? UserId { get; set; }

    public IQueryable<Post> ApplyFilters(IQueryable<Post> query)
    {
        if (AppId.HasValue && AppId.Value != Guid.Empty)
            query = query.Where(p => p.AppId == AppId.Value);

        if (PostId.HasValue && PostId.Value != Guid.Empty)
            query = query.Where(p => p.Id == PostId.Value);

        if (!string.IsNullOrEmpty(UserId))
            query = query.Where(p => p.User!.Id == UserId);

        if (!string.IsNullOrWhiteSpace(Search))
        {
            var lowerSearch = Search.ToLower();
            query = query.Where(p =>
                p.Title.ToLower().Contains(lowerSearch) ||
                p.Content.ToLower().Contains(lowerSearch) ||
                p.User!.UserName!.ToLower().Contains(lowerSearch));
        }

        return query;
    }
}