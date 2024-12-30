using System.Linq.Expressions;
using Mapster;
using Microsoft.EntityFrameworkCore;
using PGManagementService.Data.DTO;

namespace PGManagementService.Helpers
{
    public static class QueryableExtensions
        {
        public static async Task<PaginatedResult<TOutput>> GetPaginatedDataAsync<TInput, TOutput>(
                this IQueryable<TInput> query,
                int pageNumber,
                int pageSize,
                string sortBy = null,
                bool sortDescending = false,
                Expression<Func<TInput, TOutput>> selector = null)
        {
            // Apply sorting if a sort property is specified
            if (!string.IsNullOrEmpty(sortBy))
            {
                query = ApplySorting(query, sortBy, sortDescending);
            }

            IQueryable<TOutput> projectedQuery = null;
            // Apply projection (mapping to a different output type) if provided
            if (selector != null)
            {
                projectedQuery = query.Select(selector);

            }
            else
            {
                projectedQuery = query.Select(x => x.Adapt<TOutput>());
            }

            // Get total count
            var totalItems = await query.CountAsync();

            // Apply pagination
            var data = await projectedQuery.Skip((pageNumber - 1) * pageSize)
                                  .Take(pageSize)
                                  .ToListAsync();

            return new PaginatedResult<TOutput>(data, pageNumber, pageSize, totalItems);
        }

        // Helper method for dynamic sorting
        private static IQueryable<T> ApplySorting<T>(
                IQueryable<T> query,
                string sortBy,
                bool descending)
            {
                var parameter = Expression.Parameter(typeof(T), "x");
                var property = Expression.Property(parameter, sortBy);
                var lambda = Expression.Lambda(property, parameter);

                string methodName = descending ? "OrderByDescending" : "OrderBy";
                var methodCall = Expression.Call(
                    typeof(Queryable),
                    methodName,
                    new Type[] { query.ElementType, property.Type },
                    query.Expression,
                    Expression.Quote(lambda));

                return query.Provider.CreateQuery<T>(methodCall);
            }
        }


    


}
