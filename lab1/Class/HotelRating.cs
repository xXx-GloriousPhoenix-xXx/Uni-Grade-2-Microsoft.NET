using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab1.Class {
    public class HotelRating (int p_id, Hotel p_hotel, Client p_client, decimal p_rating)
    {
        public int Id { get; private set; } = p_id;
        public Hotel Hotel { get; private set; } = p_hotel;
        public Client Client { get; private set; } = p_client;
        public decimal Rating { get; private set; } = p_rating;
        public override bool Equals(object? obj) =>
            obj is not null &&
            obj is HotelRating hr &&
            Hotel.Equals(hr.Hotel) &&
            Client.Equals(hr.Client) &&
            Rating.Equals(hr.Rating);
        public override int GetHashCode() => HashCode.Combine(Id, Hotel, Client, Rating);
    }
}
