using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab1.Class {
    public class GuideRating (int p_id, Client p_client, Guide p_guide, decimal p_rating) 
    {
        public int Id { get; private set; } = p_id;
        public Client Client { get; private set; } = p_client;
        public Guide Guide { get; private set; } = p_guide;
        public decimal Rating { get; private set; } = p_rating;
        public override bool Equals(object? obj) =>
            obj is not null &&
            obj is GuideRating gr &&
            Client.Equals(gr.Client) &&
            Guide.Equals(gr.Guide) &&
            Rating.Equals(gr.Rating);
        public override int GetHashCode() => HashCode.Combine(Id, Client, Guide, Rating);
    }
}
