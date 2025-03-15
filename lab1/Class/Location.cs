namespace Lab1.Class {
    public class Location
    {
        public int Id { get; set; }
        public string City { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public Location() { }
        public Location(int p_id, string p_city, string p_address) {
            Id = p_id;
            City = p_city;
            Address = p_address;
        }
        public override bool Equals(object? obj) =>
            obj is not null &&
            obj is Location l &&
            City.Equals(l.City) &&
            Address.Equals(l.Address);
        public override int GetHashCode() => HashCode.Combine(Id, City, Address);
    }
}
