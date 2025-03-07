using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab1.Class {
    public class ClientTour (int p_id, Client p_client, Tour p_tour)
    {
        public int Id { get; private set; } = p_id;
        public Client Client { get; private set; } = p_client;
        public Tour Tour { get; private set; } = p_tour;
        public override bool Equals(object? obj) =>
            obj is not null &&
            obj is ClientTour ct &&
            Client.Equals(ct.Client) &&
            Tour.Equals(ct.Tour);
        public override int GetHashCode() => HashCode.Combine(Id, Client, Tour);
    }
}
