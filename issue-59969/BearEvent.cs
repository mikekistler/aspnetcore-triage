using System.Text.Json.Serialization;

[JsonDerivedType(typeof(Added), typeDiscriminator: "added")]
[JsonDerivedType(typeof(Angered), typeDiscriminator: "angered")]
public abstract record BearEvent(DateTime Timestamp)
{
    public sealed record Added(DateTime Timestamp, int Id) : BearEvent(Timestamp);

    public sealed record Angered(DateTime Timestamp) : BearEvent(Timestamp);
}