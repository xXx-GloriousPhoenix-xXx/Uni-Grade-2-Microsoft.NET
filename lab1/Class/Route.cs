namespace Lab1.Class {
    public class Route
    {
        public int Id { get; set; }
        public Guide Guide { get; set; } = new();
        public int Duration { get; set; }
        public Route() { }
        public Route(int p_id, Guide p_guide, int p_duration) {
            Id = p_id;
            Guide = p_guide;
            Duration = p_duration;
        }
        public override bool Equals(object? obj) =>
            obj is not null &&
            obj is Route r &&
            Guide.Equals(r.Guide) &&
            Duration.Equals(r.Duration);
        public override int GetHashCode() => HashCode.Combine(Id, Guide, Duration);
    }
}
