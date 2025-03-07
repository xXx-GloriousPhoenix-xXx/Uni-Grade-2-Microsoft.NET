using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab1.Class {
    public class Route (int p_id, Guide p_guide, int p_duration) 
    {
        public int Id { get; private set; } = p_id;
        public Guide Guide { get; private set; } = p_guide;
        public int Duration { get; private set; } = p_duration;
        public override bool Equals(object? obj) =>
            obj is not null &&
            obj is Route r &&
            Guide.Equals(r.Guide) &&
            Duration.Equals(r.Duration);
        public override int GetHashCode() => HashCode.Combine(Id, Guide, Duration);
    }
}
