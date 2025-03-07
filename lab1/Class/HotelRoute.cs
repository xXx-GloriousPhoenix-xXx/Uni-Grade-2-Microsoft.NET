using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab1.Class {
    public class HotelRoute (int p_id, Route p_route, Hotel p_hotel)
    {
        public int Id { get; private set; } = p_id;
        public Hotel Hotel { get; private set; } = p_hotel;
        public Route Route { get; private set; } = p_route;
        public override bool Equals(object? obj) =>
            obj is not null &&
            obj is HotelRoute ht &&
            Hotel.Equals(ht.Hotel) &&
            Route.Equals(ht.Route);
        public override int GetHashCode() => HashCode.Combine(Id, Hotel, Route);
    }
}
