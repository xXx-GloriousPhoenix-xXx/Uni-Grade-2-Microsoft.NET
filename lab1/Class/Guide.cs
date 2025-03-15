namespace Lab1.Class {
    public class Guide {
        public int Id { get; set; }
        public Person Person { get; set; } = new();
        public int Experience { get; set; }
        public Guide() { }
        public Guide(int p_id, Person p_person, int p_experience) {
            Id = p_id;
            Person = p_person;
            Experience = p_experience;
        }
        public override bool Equals(object? obj) =>
            obj is not null &&
            obj is Guide g &&
            Person.Equals(g.Person) &&
            Experience.Equals(g.Experience);
        public override int GetHashCode() => HashCode.Combine(Id, Person, Experience);
    }
}
