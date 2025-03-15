namespace Lab1.Class {
    public class Hotel
    {
        public int Id { get; set; }
        public Location Location { get; set; } = new();
        public Hotel() { }
        public Hotel(int p_id, Location p_location) {
            Id = p_id;
            Location = p_location;
        }
        public override bool Equals(object? obj) =>
            obj is not null &&
            obj is Hotel h &&
            Location.Equals(h.Location);
        public override int GetHashCode() => HashCode.Combine(Id, Location);
    }
}
