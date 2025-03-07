using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab1.Class {
    public class Guide(int p_id, Person p_person, int p_experience) {
        public int Id { get; private set; } = p_id;
        public Person Person { get; private set; } = p_person;
        public int Experience { get; private set; } = p_experience;
        public override bool Equals(object? obj) =>
            obj is not null &&
            obj is Guide g &&
            Person.Equals(g.Person) &&
            Experience.Equals(g.Experience);
        public override int GetHashCode() => HashCode.Combine(Id, Person, Experience);
    }
}
