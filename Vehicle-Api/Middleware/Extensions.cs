using Common;
using Common.Sorting;

namespace Vehicle_Api.Middleware;

public static class Extensions
{
    extension<T>(Sorter<T> sorter) where T: BaseEntity
    {
        public  IOrderedQueryable<T> ApplySort(IQueryable<T> query)
        {
            IOrderedQueryable<T>? orderedQueryable = null;
            foreach (var sortField in sorter)
                orderedQueryable = orderedQueryable is null
                    ? ApplyFirstSort(query, sortField)
                    : ApplyThenBySort(orderedQueryable, sortField);
            return orderedQueryable!;
        }

        private static IOrderedQueryable<T> ApplyFirstSort(IQueryable<T> query, SortField<T> sortField)
            => sortField.SortDirection is SortDirection.Ascending
                ? query.OrderBy(sortField.SortingExpression)
                : query.OrderByDescending(sortField.SortingExpression);
        private static IOrderedQueryable<T> ApplyThenBySort(IOrderedQueryable<T> query, SortField<T> sortField)
            => sortField.SortDirection is SortDirection.Ascending
                ? query.ThenBy(sortField.SortingExpression)
                : query.ThenByDescending(sortField.SortingExpression);

    }
}
