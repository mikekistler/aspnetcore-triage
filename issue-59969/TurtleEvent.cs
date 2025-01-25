using System.Text.Json.Serialization;

[JsonDerivedType(typeof(Added), typeDiscriminator: "added")]
[JsonDerivedType(typeof(SlowedDown), typeDiscriminator: "slowedDown")]
public abstract record TurtleEvent(DateTime Timestamp)
{
    public sealed record Added(DateTime Timestamp, int Id) : TurtleEvent(Timestamp);

    public sealed record SlowedDown(DateTime Timestamp) : TurtleEvent(Timestamp);
}