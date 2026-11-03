namespace Common;

public record PageRequestDto(int LastId=0, int Count=10, string? Sorts=null);

public record PageResponseDto<T>(IReadOnlyList<T> Items, int LastId, int TotalCount);