using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Postkit.Shared.Helpers
{
    public class PagedResponse<T>
    {
        public List<T> Data { get; set; } = new List<T>();
        public PaginationMetadata Pagination { get; set; } = new PaginationMetadata();
    }
}
