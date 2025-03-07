using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab1.Class {
    public class Person(int p_id, string p_surname, string p_name)
    {
        public int Id { get; private set; } = p_id;
        public string Surname { get; private set; } = p_surname;
        public string Name { get; private set; } = p_name;
        public string FullName() => $"{Surname} {Name}";
        public override bool Equals(object? obj) =>
            obj is not null &&
            obj is Person p &&
            Surname.Equals(p.Surname) &&
            Name.Equals(p.Name);
        public override int GetHashCode() => HashCode.Combine(Id, Surname, Name);
    }
}
