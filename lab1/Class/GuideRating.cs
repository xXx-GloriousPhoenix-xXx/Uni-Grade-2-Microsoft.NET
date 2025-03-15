namespace Lab1.Class {
    public class GuideRating
    {
        public int Id { get; set; }
        public Client Client { get; set; } = new();
        public Guide Guide { get; set; } = new();
        public decimal Rating { get; set; }
        public GuideRating() { }
        public GuideRating(int p_id, Client p_client, Guide p_guide, decimal p_rating) {
            Id = p_id;
            Client = p_client;
            Guide = p_guide;
            Rating = p_rating;
        }
        public override bool Equals(object? obj) =>
            obj is not null &&
            obj is GuideRating gr &&
            Client.Equals(gr.Client) &&
            Guide.Equals(gr.Guide) &&
            Rating.Equals(gr.Rating);
        public override int GetHashCode() => HashCode.Combine(Id, Client, Guide, Rating);
    }
}
