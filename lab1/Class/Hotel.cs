using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab1.Class {
    public class Hotel (int p_id, Location p_location)
    {
        public int Id { get; private set; } = p_id;
        public Location Location { get; private set; } = p_location;
        public override bool Equals(object? obj) =>
            obj is not null &&
            obj is Hotel h &&
            Location.Equals(h.Location);
        public override int GetHashCode() => HashCode.Combine(Id, Location);
    }
}
