using Postkit.Comments.DTOs;
using Postkit.Comments.Queries;
using Postkit.Shared.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Postkit.Comments.Interfaces
{
    public interface ICommentService
    {
        Task<PagedResponse<CommentDto>> GetByPostIdAsync(CommentQuery query);
        Task<CommentDto> CreateAsync(CreateCommentDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
