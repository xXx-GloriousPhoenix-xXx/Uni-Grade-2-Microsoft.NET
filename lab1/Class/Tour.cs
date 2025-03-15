namespace Lab1.Class {
    public class Tour 
    {
        public int Id { get; set; }
        public Route Route { get; set; } = new();
        public Tour() { }
        public Tour(int p_id, Route p_route) {
            Id = p_id;
            Route = p_route;
        }
        public override bool Equals(object? obj) =>
            obj is not null &&
            obj is Tour t &&
            Route.Equals(t.Route);
        public override int GetHashCode() => HashCode.Combine(Id, Route);
    }
}
