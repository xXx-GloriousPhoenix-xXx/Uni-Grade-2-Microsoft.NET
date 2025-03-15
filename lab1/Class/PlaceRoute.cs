namespace Lab1.Class {
    public class PlaceRoute
    {
        public int Id { get; set; }
        public Route Route { get; set; } = new();
        public Place Place { get; set; } = new();
        public PlaceRoute() { }
        public PlaceRoute(int p_id, Route p_route, Place p_place) {
            Id = p_id;
            Route = p_route;
            Place = p_place;
        }
        public override bool Equals(object? obj) =>
            obj is not null &&
            obj is PlaceRoute pt &&
            Route.Equals(pt.Route) &&
            Place.Equals(pt.Place);
        public override int GetHashCode() => HashCode.Combine(Id, Route, Place);
    }
}
