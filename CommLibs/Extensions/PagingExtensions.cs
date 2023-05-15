using CommLibs.Dto;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommLibs.Extensions
{
    public static class PagingExtensions
    {
        public static async Task<PagedResult<T>> ToPagedResultAsync<T>(this IQueryable<T> query, int pageNumber, int pageSize,string? search=null )
        {
            var TotalCount = await query.CountAsync();
            var Items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

            return new PagedResult<T>
            {
                Items = Items,
                TotalCount = TotalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
                Search = search
                
            };
        }
    }
}
