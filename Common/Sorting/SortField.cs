using System.Linq.Expressions;

namespace Common.Sorting;

public record struct SortField(string Key, SortDirection Direction);

public record struct SortField<T>(Expression<Func<T,object>> SortingExpression, SortDirection SortDirection) ;