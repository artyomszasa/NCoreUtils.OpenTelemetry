using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json.Serialization;

namespace NCoreUtils.OpenTelemetry;

[method: JsonConstructor]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
public readonly struct TruncatableString(string? value, int truncatedByteCount = 0)
    : IEquatable<TruncatableString>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(TruncatableString left, TruncatableString right) => left.Equals(right);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(TruncatableString left, TruncatableString right) => !(left == right);

    private static UTF8Encoding Utf8 { get; } = new(false);

    public static TruncatableString Create(string? source, int maxUtf8Length)
    {
        if (string.IsNullOrEmpty(source))
        {
            return default;
        }
        if (Utf8.GetByteCount(source) <= maxUtf8Length)
        {
            return new(source);
        }
        ReadOnlySpan<char> span;
        if (source.Length > maxUtf8Length)
        {
            span = source.AsSpan(..maxUtf8Length);
        }
        else
        {
            span = source.AsSpan(..^1);
        }
        while (Utf8.GetByteCount(span) > maxUtf8Length)
        {
            span = span[..^1];
        }
        return new(new(span), Utf8.GetByteCount(source.AsSpan(span.Length..)));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(TruncatableString other)
        => _value == other._value
            && TruncatedByteCount == other.TruncatedByteCount;

    public override bool Equals([NotNullWhen(true)] object? obj)
        => obj is TruncatableString other && Equals(other);

    public override int GetHashCode()
        => HashCode.Combine(Value, TruncatedByteCount);

    private readonly string? _value = value;

    [JsonPropertyName("value")]
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public string Value => _value ?? string.Empty;

    [JsonPropertyName("truncatedByteCount")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int TruncatedByteCount { get; } = truncatedByteCount;

    public override string ToString()
        => TruncatedByteCount == 0
            ? Value
            : $"{Value}...";
}