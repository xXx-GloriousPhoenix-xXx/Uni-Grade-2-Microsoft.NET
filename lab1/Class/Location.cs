using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab1.Class {
    public class Location (int p_id, string p_city, string p_address) 
    {
        public int Id { get; private set; } = p_id;
        public string City { get; private set; } = p_city;
        public string Address { get; private set; } = p_address;
        public override bool Equals(object? obj) =>
            obj is not null &&
            obj is Location l &&
            City.Equals(l.City) &&
            Address.Equals(l.Address);
        public override int GetHashCode() => HashCode.Combine(Id, City, Address);
    }
}
