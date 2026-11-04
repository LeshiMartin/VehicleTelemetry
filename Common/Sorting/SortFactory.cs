using System.Collections;
using System.Linq.Expressions;

namespace Common.Sorting;

public static class SortFactory
{
    public static IReadOnlyList<SortField> ParseSort(string? sorts)
    {
        if (string.IsNullOrEmpty(sorts))
            return [];

        var sortingParts = sorts.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        return sortingParts.Select(ParseSortingPart).ToArray();
    }


    private static SortField ParseSortingPart(string sortingPart)
        => sortingPart.StartsWith('-')
            ? new SortField(sortingPart[1..], SortDirection.Descending)
            : new SortField(sortingPart, SortDirection.Ascending);

    extension(PageRequestDto dto)
    {
        public Sorter<T> GetSorter<T>(SortingExpression<T> sortingExpressions) where T : BaseEntity
        {
            var requests = ParseSort(dto.Sorts);
            if (requests.Count == 0)
                return new Sorter<T>([new SortField<T>(x => x.Id, SortDirection.Ascending)]);

            var results = new List<SortField<T>>();
            foreach (var (key, sortingDirection) in requests)
                if (sortingExpressions.TryGetValue(key, out var expression))
                    results.Add(new SortField<T>(expression, sortingDirection));
            return new Sorter<T>(results);
        }
    }
}


public class Sorter<T>(IReadOnlyList<SortField<T>> sortingField) : IReadOnlyList<SortField<T>> where T : BaseEntity
{
    public IEnumerator<SortField<T>> GetEnumerator() => sortingField.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public int Count => sortingField.Count;

    public SortField<T> this[int index] => sortingField[index];
}

public class SortingExpression<T> : Dictionary<string, Expression<Func<T, object>>>
{
    public SortingExpression():this(StringComparer.InvariantCultureIgnoreCase)
    {
        
    }

    public SortingExpression(IEqualityComparer<string> equalityComparer):base(equalityComparer)
    {
        
    }
}