using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab1.Class {
    public class Client(int p_id, Person p_person) {
        public int Id { get; private set; } = p_id;
        public Person Person { get; private set; } = p_person;
        public override bool Equals(object? obj) =>
            obj is not null &&
            obj is Client c &&
            Person.Equals(c.Person);
        public override int GetHashCode() => HashCode.Combine(Id, Person);
    }
}
