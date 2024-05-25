using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using NCoreUtils.OpenTelemetry.Json;

namespace NCoreUtils.OpenTelemetry;

[JsonConverter(typeof(TraceSpanAttributeValueConverter))]
public readonly struct TraceSpanAttributeValue
    : IEquatable<TraceSpanAttributeValue>
{
    private enum TagValue
    {
        Invalid = 0,
        String = 1,
        Integer = 2,
        Boolean = 3
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(TraceSpanAttributeValue left, TraceSpanAttributeValue right) => left.Equals(right);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(TraceSpanAttributeValue left, TraceSpanAttributeValue right) => !(left == right);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator TraceSpanAttributeValue(string value) => new(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator TraceSpanAttributeValue(long value) => new(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator TraceSpanAttributeValue(bool value) => new(value);

    private readonly TagValue tag;

    public readonly TruncatableString StringValue { get; }

    public readonly long IntValue { get; }

    public readonly bool BoolValue { get; }

    public bool IsString => tag == TagValue.String;

    public bool IsInteger => tag == TagValue.Integer;

    public bool IsBoolean => tag == TagValue.Boolean;

    private TraceSpanAttributeValue(TagValue tag, TruncatableString stringValue, long intValue, bool boolValue)
    {
        this.tag = tag;
        StringValue = stringValue;
        IntValue = intValue;
        BoolValue = boolValue;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TraceSpanAttributeValue(string value)
        : this(TagValue.String, TruncatableString.Create(value, 256), default, default)
    { }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal TraceSpanAttributeValue(TruncatableString value)
        : this(TagValue.String, value, default, default)
    { }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TraceSpanAttributeValue(long value)
        : this(TagValue.Integer, default, value, default)
    { }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TraceSpanAttributeValue(bool value)
        : this(TagValue.Integer, default, default, value)
    { }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(TraceSpanAttributeValue other) => tag switch
    {
        TagValue.String => other.IsString && other.StringValue.Equals(StringValue),
        TagValue.Integer => other.IsInteger && other.IntValue == IntValue,
        TagValue.Boolean => other.IsBoolean && other.BoolValue == BoolValue,
        _ => false
    };

    public override bool Equals([NotNullWhen(true)] object? obj)
        => obj is TraceSpanAttributeValue other && Equals(other);

    public override int GetHashCode() => tag switch
    {
        TagValue.String => HashCode.Combine(TagValue.String, StringValue),
        TagValue.Integer => HashCode.Combine(TagValue.Integer, IntValue),
        TagValue.Boolean => HashCode.Combine(TagValue.Boolean, BoolValue),
        _ => -1
    };

    public override string ToString() => tag switch
    {
        TagValue.String => StringValue.ToString(),
        TagValue.Integer => IntValue.ToString(),
        TagValue.Boolean => BoolValue.ToString(),
        _ => string.Empty
    };
}
