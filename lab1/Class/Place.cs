using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab1.Class {
    public class Place (int p_id, string p_name, Location p_location)
    {
        public int Id { get; private set; } = p_id;
        public string Name { get; private set; } = p_name;
        public Location Location { get; private set; } = p_location;
        public override bool Equals(object? obj) =>
            obj is not null &&
            obj is Place p &&
            Name.Equals(p.Name) &&
            Location.Equals(p.Location);
        public override int GetHashCode() => HashCode.Combine(Id, Name, Location);
    }
}
