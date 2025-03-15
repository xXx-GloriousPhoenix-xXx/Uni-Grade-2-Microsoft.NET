namespace Lab1.Class
{
    public class Person
    {
        public int Id { get; set; }
        public string Surname { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string FullName() => $"{Surname} {Name}";
        public Person() { }
        public Person(int p_id, string p_surname, string p_name)
        {
            Id = p_id;
            Surname = p_surname;
            Name = p_name;
        }
        public override bool Equals(object? obj) =>
            obj is not null &&
            obj is Person p &&
            Surname.Equals(p.Surname) &&
            Name.Equals(p.Name);
        public override int GetHashCode() => HashCode.Combine(Id, Surname, Name);
    }
}
