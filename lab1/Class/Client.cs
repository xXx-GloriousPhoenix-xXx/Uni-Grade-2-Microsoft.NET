namespace Lab1.Class {
    public class Client {
        public int Id { get; set; }
        public Person Person { get; set; } = new();
        public Client() { }
        public Client(int p_id, Person p_person)
        {
            Id = p_id;
            Person = p_person;
        }
        public override bool Equals(object? obj) =>
            obj is not null &&
            obj is Client c &&
            Person.Equals(c.Person);
        public override int GetHashCode() => HashCode.Combine(Id, Person);
    }
}
