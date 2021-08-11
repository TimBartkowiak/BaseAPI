using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BaseAPI.Entities;
using BaseAPI.Models;

namespace BaseAPI.Utils
{
    public static class DataPager
    {
        public static Paged<TEntity> Paginate<TEntity>(
            this IQueryable<TEntity> query,
            int page,
            int limit)
            where TEntity : AbstractEntity
        {

            var paged = new Paged<TEntity>();

            page = (page < 0) ? 1 : page;

            paged.CurrentPage = page;
            paged.PageSize = limit;

            var totalItemsCountTask = query.Count();

            var startRow = (page - 1) * limit;
            paged.Items = query
                .Skip(startRow)
                .Take(limit)
                .ToList();

            paged.TotalItems = totalItemsCountTask;
            paged.TotalPages = (int)Math.Ceiling(paged.TotalItems / (double)limit);

            return paged;
        }
    }
}