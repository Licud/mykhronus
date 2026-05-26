namespace MyKhronus.DataAccess.Shared;

public record struct Option<T>(T value)
{
    public bool IsSet { get; init; }

    public T Value { get; } = value;

    public static implicit operator Option<T>(T value) => new Option<T>(value)
    {
        IsSet = true,
    };
}
