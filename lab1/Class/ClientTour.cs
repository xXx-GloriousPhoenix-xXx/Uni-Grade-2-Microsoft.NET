namespace Lab1.Class {
    public class ClientTour
    {
        public int Id { get; set; }
        public Client Client { get; set; } = new();
        public Tour Tour { get; set; } = new();
        public ClientTour() { }
        public ClientTour(int p_id, Client p_client, Tour p_tour) {
            Id = p_id;
            Client = p_client;
            Tour = p_tour;
        }
        public override bool Equals(object? obj) =>
            obj is not null &&
            obj is ClientTour ct &&
            Client.Equals(ct.Client) &&
            Tour.Equals(ct.Tour);
        public override int GetHashCode() => HashCode.Combine(Id, Client, Tour);
    }
}
