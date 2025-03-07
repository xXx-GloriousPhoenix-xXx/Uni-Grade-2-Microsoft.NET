using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab1.Class {
    public class PlaceRoute (int p_id, Route p_route, Place p_place)
    {
        public int Id { get; private set; } = p_id;
        public Route Route { get; private set; } = p_route;
        public Place Place { get; private set; } = p_place;
        public override bool Equals(object? obj) =>
            obj is not null &&
            obj is PlaceRoute pt &&
            Route.Equals(pt.Route) &&
            Place.Equals(pt.Place);
        public override int GetHashCode() => HashCode.Combine(Id, Route, Place);
    }
}
