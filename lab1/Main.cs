using Lab1.Class;
using System.Data;
using System.Text;

namespace Lab1 {
    public class Config(int p_client, int p_guide, int p_route, decimal[] p_hotel_weight, decimal[] p_guide_weight)
    {
        public int Client { get; private set; } = p_client;
        public int Guide { get; private set; } = p_guide;
        public int Route { get; private set; } = p_route;
        public decimal[] Hotel_Weight { get; private set; } = p_hotel_weight;
        public decimal[] Guide_Weight { get; private set; } = p_guide_weight;
    }
    public partial class LINQToObjectsQuery : QueryProcessor 
    {
        public Config Config { get; private set; }
        #region Data
        public List<Person> Person { get; private set; }
        public List<Client> Client { get; private set; }
        public List<Guide> Guide { get; private set; }
        public List<Route> Route { get; private set; }
        public List<Tour> Tour { get; private set; }
        public List<ClientTour> ClientTour { get; private set; }
        public List<Location> Location { get; private set; }
        public List<Place> Place { get; private set; }
        public List<Hotel> Hotel { get; private set; }
        public List<PlaceRoute> PlaceRoute { get; private set; }
        public List<HotelRoute> HotelRoute { get; private set; }
        public List<HotelRating> HotelRating { get; private set; }
        public List<GuideRating> GuideRating { get; private set; }
        #endregion
        public LINQToObjectsQuery(Config p_config) 
        { 
            Config = p_config;
            Person = Enumerable.Range(1, Config.Client + Config.Guide).Select(i => new Person(i, $"Surname_{i}", $"Name_{i}")).ToList();
            Client = Person.Take(Config.Client).Select((p, i) => new Client(i + 1, p)).ToList();
            Guide = Person.Skip(Config.Client).Take(Config.Guide).Select((p, i) => new Guide(i + 1, p, new Random().Next(1, 11))).ToList();
            Route = Enumerable.Range(1, Config.Route).Select(i => new Route(i, Guide[new Random().Next(Guide.Count)], new Random().Next(4, 15))).ToList();
            Tour = Enumerable.Range(1, Config.Client / 10).Select(i => new Tour(i, Route[new Random().Next(Route.Count)])).ToList();
            ClientTour = Client.Chunk(10).SelectMany((group, index) => group
                .Select((c, i) => new ClientTour(index * 10 + i + 1, c, Tour[index]))).ToList();
            Location = Enumerable.Range(1, 80).Select(i => new Location(i, $"City_{i}", $"Address_{i}")).ToList();
            Place = Location.Take(20).Select((l, i) => new Place(i + 1, $"Name_{i + 1}", l)).ToList();
            Hotel = Location.Skip(20).Take(60).Select((h, i) => new Hotel(i + 1, h)).ToList();
            PlaceRoute = Route.SelectMany(r => Place.OrderBy(_ => new Random().Next()).Take(new Random().Next(3, 6))
                .Select(p => new { Route = r, Place = p })).Select((pr, index) => new PlaceRoute(index + 1, pr.Route, pr.Place)).ToList();
            HotelRoute = Route.SelectMany(r => Hotel.OrderBy(_ => new Random().Next()).Take(new Random().Next(9, 15))
                .Select(h => new { Route = r, Hotel = h })).Select((hr, index) => new HotelRoute(index + 1, hr.Route, hr.Hotel)).ToList();
            HotelRating = HotelRoute.SelectMany(hr => ClientTour.Where(ct => ct.Tour.Route == hr.Route)
                .Select(ct => new { hr.Hotel, ct.Client, Rating = GetRating(Config.Hotel_Weight) }))
                .Select((hr, i) => new HotelRating(i + 1, hr.Hotel, hr.Client, hr.Rating)).ToList();
            GuideRating = Tour.SelectMany(t => ClientTour.Where(ct => ct.Tour == t)
                .Select(ct => new { ct.Client, t.Route.Guide, Rating = GetRating(Config.Guide_Weight) }))
                .Select((gr, i) => new GuideRating(i + 1, gr.Client, gr.Guide, gr.Rating)).ToList();
        }
        public static decimal GetRating(decimal[] weight_list) => 1m * weight_list[Random.Shared.Next(weight_list.Length)];
    }
    public static class Test
    {
        public static void Main() 
        {
            Console.OutputEncoding = Encoding.UTF8;
            var client = 4800;
            var guide = 12;
            var route = 50;
            var hotel_weight = new decimal[] { 1, 2, 3, 3, 4, 4, 4, 4, 5, 5, 5, 5, 5, 5, 5, 5, 5 };
            var guide_weight = new decimal[] { 1, 2, 3, 3, 4, 4, 4, 4, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5 };
            
            var cfg = new Config(client, guide, route, hotel_weight, guide_weight);
            var qgen = new LINQToObjectsQuery(cfg);

            while (true)
            {
                var result = qgen.CallMethod();
                if (result) break;
            }
        }        
    }
}
