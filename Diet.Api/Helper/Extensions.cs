using System;
using System.Linq;
using Diet.Api.Features.Filter;

namespace Diet.Api.Helper
{
    public static class Extensions
    {
        public static IQueryable<T> Paginate<T>(this IQueryable<T> source, int pageIndex, int pageSize)
        {
            return source.Skip(pageIndex * pageSize).Take(pageSize);
        }

        public static int GetPageCount(int itemCount, int pageSize)
        {
            return (int) Math.Ceiling(itemCount / (double) pageSize);
        }

        public static IQueryable<TSource> Filter<TSource>(this IQueryable<TSource> source, string filter)
        {
            return string.IsNullOrEmpty(filter) ? source : source.Where(new ExpressionProvider<TSource>().Filter(filter));
        }
    }
}