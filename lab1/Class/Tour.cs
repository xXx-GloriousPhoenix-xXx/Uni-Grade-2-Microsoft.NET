using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab1.Class {
    public class Tour (int p_id, Route p_route) 
    {
        public int Id { get; private set; } = p_id;
        public Route Route { get; private set; } = p_route;
        public override bool Equals(object? obj) =>
            obj is not null &&
            obj is Tour t &&
            Route.Equals(t.Route);
        public override int GetHashCode() => HashCode.Combine(Id, Route);
    }
}
