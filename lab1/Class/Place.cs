namespace Lab1.Class {
    public class Place
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public Location Location { get; set; } = new();
        public Place() { }
        public Place(int p_id, string p_name, Location p_location) {
            Id = p_id;
            Name = p_name;
            Location = p_location;
        }
        public override bool Equals(object? obj) =>
            obj is not null &&
            obj is Place p &&
            Name.Equals(p.Name) &&
            Location.Equals(p.Location);
        public override int GetHashCode() => HashCode.Combine(Id, Name, Location);
    }
}
