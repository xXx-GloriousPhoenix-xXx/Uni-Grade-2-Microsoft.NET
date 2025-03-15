namespace Lab1.Class {
    public class HotelRoute
    {
        public int Id { get; set; }
        public Hotel Hotel { get; set; } = new();
        public Route Route { get; set; } = new();
        public HotelRoute() { }
        public HotelRoute(int p_id, Route p_route, Hotel p_hotel) {
            Id = p_id;
            Hotel = p_hotel;
            Route = p_route;
        }
        public override bool Equals(object? obj) =>
            obj is not null &&
            obj is HotelRoute ht &&
            Hotel.Equals(ht.Hotel) &&
            Route.Equals(ht.Route);
        public override int GetHashCode() => HashCode.Combine(Id, Hotel, Route);
    }
}
