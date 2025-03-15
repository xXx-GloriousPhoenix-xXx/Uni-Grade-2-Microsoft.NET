namespace Lab1.Class {
    public class HotelRating 
    {
        public int Id { get; set; }
        public Hotel Hotel { get; set; } = new();
        public Client Client { get; set; } = new();
        public decimal Rating { get; set; }
        public HotelRating() { }
        public HotelRating(int p_id, Hotel p_hotel, Client p_client, decimal p_rating) {
            Id = p_id;
            Hotel = p_hotel;
            Client = p_client;
            Rating = p_rating;
        }
        public override bool Equals(object? obj) =>
            obj is not null &&
            obj is HotelRating hr &&
            Hotel.Equals(hr.Hotel) &&
            Client.Equals(hr.Client) &&
            Rating.Equals(hr.Rating);
        public override int GetHashCode() => HashCode.Combine(Id, Hotel, Client, Rating);
    }
}
