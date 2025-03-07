using Lab1.Class;
using System.Data;
using System.Text;

namespace Lab1.Use {
    public class Config(int p_client, int p_guide, int p_route, decimal[] p_hotel_weight, decimal[] p_guide_weight)
    {
        public int Client { get; private set; } = p_client;
        public int Guide { get; private set; } = p_guide;
        public int Route { get; private set; } = p_route;
        public decimal[] Hotel_Weight { get; private set; } = p_hotel_weight;
        public decimal[] Guide_Weight { get; private set; } = p_guide_weight;
    }
    public class LINQToObjectsQuery
    {
        public Config Config { get; private set; }
        #region Data
        public List<Person> Person { get; private set; }
        public List<Client> Client { get; private set; }
        public List<Guide> Guide { get; private set; }
        public List<Route> Route { get; private set; }
        public List<Tour> Tour { get; private set; }
        public List<ClientTour> Client_Tour { get; private set; }
        public List<Location> Location { get; private set; }
        public List<Place> Place { get; private set; }
        public List<Hotel> Hotel { get; private set; }
        public List<PlaceRoute> Place_Route { get; private set; }
        public List<HotelRoute> Hotel_Route { get; private set; }
        public List<HotelRating> Hotel_Rating { get; private set; }
        public List<GuideRating> Guide_Rating { get; private set; }
        #endregion
        public LINQToObjectsQuery(Config p_config) 
        { 
            Config = p_config;
            Person = Enumerable.Range(1, Config.Client + Config.Guide).Select(i => new Person(i, $"Surname_{i}", $"Name_{i}")).ToList();
            Client = Person.Take(Config.Client).Select((p, i) => new Client(i + 1, p)).ToList();
            Guide = Person.Skip(Config.Client).Take(Config.Guide).Select((p, i) => new Guide(i + 1, p, new Random().Next(1, 11))).ToList();
            Route = Enumerable.Range(1, Config.Route).Select(i => new Route(i, Guide[new Random().Next(Guide.Count)], new Random().Next(4, 15))).ToList();
            Tour = Enumerable.Range(1, Config.Client / 10).Select(i => new Tour(i, Route[new Random().Next(Route.Count)])).ToList();
            Client_Tour = Client.Chunk(10).SelectMany((group, index) => group
                .Select((c, i) => new ClientTour(index * 10 + i + 1, c, Tour[index]))).ToList();
            Location = Enumerable.Range(1, 80).Select(i => new Location(i, $"City_{i}", $"Address_{i}")).ToList();
            Place = Location.Take(20).Select((l, i) => new Place(i + 1, $"Name_{i + 1}", l)).ToList();
            Hotel = Location.Skip(20).Take(60).Select((h, i) => new Hotel(i + 1, h)).ToList();
            Place_Route = Route.SelectMany(r => Place.OrderBy(_ => new Random().Next()).Take(new Random().Next(3, 6))
                .Select(p => new { Route = r, Place = p })).Select((pr, index) => new PlaceRoute(index + 1, pr.Route, pr.Place)).ToList();
            Hotel_Route = Route.SelectMany(r => Hotel.OrderBy(_ => new Random().Next()).Take(new Random().Next(9, 15))
                .Select(h => new { Route = r, Hotel = h })).Select((hr, index) => new HotelRoute(index + 1, hr.Route, hr.Hotel)).ToList();
            Hotel_Rating = Hotel_Route.SelectMany(hr => Client_Tour.Where(ct => ct.Tour.Route == hr.Route)
                .Select(ct => new { hr.Hotel, ct.Client, Rating = GetRating(Config.Hotel_Weight) }))
                .Select((hr, i) => new HotelRating(i + 1, hr.Hotel, hr.Client, hr.Rating)).ToList();
            Guide_Rating = Tour.SelectMany(t => Client_Tour.Where(ct => ct.Tour == t)
                .Select(ct => new { ct.Client, t.Route.Guide, Rating = GetRating(Config.Guide_Weight) }))
                .Select((gr, i) => new GuideRating(i + 1, gr.Client, gr.Guide, gr.Rating)).ToList();
        }
        public static decimal GetRating(decimal[] weight_list) => 1m * weight_list[Random.Shared.Next(weight_list.Length)];
        private static void ShowQuery<T>(string title, IEnumerable<T> query, Dictionary<string, string> field_name)
        {
            Console.WriteLine($"———————{title}:———————");
            var last_item = query.LastOrDefault();
            if (last_item is null)
            {
                Console.WriteLine("Результатів не знайдено");
            }
            else
            {
                foreach (var item in query)
                {
                    foreach (var prop in item!.GetType().GetProperties())
                    {
                        string name = field_name[prop.Name];
                        Console.WriteLine($"{name}: {prop.GetValue(item)}");
                    }
                    if (!item.Equals(last_item))
                    {
                        Console.WriteLine();
                    }
                }
            }
            Console.WriteLine("———————————————————————————\n");
        }
        /// <summary>
        /// Most experienced guides
        /// </summary>
        public void SelfQuery_1()
        {
            var sq = (from r in Route
                      join ct in Client_Tour on r equals ct.Tour.Route
                      group ct by r.Guide into GuideClientGroup
                      let gc = GuideClientGroup.Count()
                      orderby GuideClientGroup.Key.Experience descending, gc descending
                      select new
                      {
                          Guide = GuideClientGroup.Key.Id,
                          AgeExperience = GuideClientGroup.Key.Experience,
                          ClientExperience = gc
                      }).Take(3);
            ShowQuery("Self Query 1", sq, new Dictionary<string, string>
            {
                { "Guide", "Гід" },
                { "AgeExperience", "Стаж" },
                { "ClientExperience", "Кількість клієнтів" }
            });
        }
        /// <summary>
        /// Most popular visited place
        /// </summary>
        public void SelfQuery_2()
        {
            var sq = (from pr in Place_Route
                     join ct in Client_Tour on pr.Route equals ct.Tour.Route
                     group ct.Client by pr into PlaceClientGroup
                     let pc = PlaceClientGroup.Count()
                     select new
                     {
                         Place = PlaceClientGroup.Key.Id,
                         TotalClient = pc
                     }).Take(5);
            ShowQuery("Self Query 2", sq, new Dictionary<string, string>
            {
                { "Place", "Місце" },
                { "TotalClient", "Кількість відвідувань" }
            });
        }
        /// <summary>
        /// Statistics of rate chance
        /// </summary>
        public void SelfQuery_3()
        {
            var sq = from hr in Hotel_Rating
                     group hr by hr.Rating into RatingGroup
                     let tr = (decimal)(Hotel_Rating.Count)
                     let cr = RatingGroup.Count()
                     orderby RatingGroup.Key descending
                     select new
                     {
                         Rating = RatingGroup.Key,
                         Chance = $"{Math.Round(cr / tr * 100, 2)} %"
                     };
            ShowQuery("Self Query 3", sq, new Dictionary<string, string>
            {
                { "Rating", "Рейтинг" },
                { "Chance", "Вірогідність" }
            });
        }
        /// <summary>
        /// ID of route for each duration
        /// </summary>
        public void SelfQuery_4()
        {
            var sq = from r in Route
                     group r by r.Duration into DurationGroup
                     orderby DurationGroup.Key descending
                     let ids = DurationGroup.Select(dg => dg.Id.ToString()).Aggregate((curr, next) => curr + " " + next)
                     let tid = DurationGroup.Count()
                     select new
                     {
                         Duration = DurationGroup.Key,
                         Route = ids,
                         TotalRoute = tid
                     };
            ShowQuery("Self Query 4", sq, new Dictionary<string, string>
            {
                { "Duration", "Тривалість" },
                { "Route", "Маршрути" },
                { "TotalRoute", "Загальна кількість" }
            });
        }
        /// <summary>
        /// Returns IEnumerable of client names who's indexes are in 1% by id, 
        /// visited hotels with index in 10% 
        /// and visited places with index in 20%
        /// </summary>
        public IEnumerable<string> SelfQuery_5() 
        {
            var bq = from ct in Client_Tour.DistinctBy(c => c.Client.Person)
                     let cq = Client.Count
                     where ct.Client.Id <= cq * 0.01m
                     select new { ct.Client, ct.Tour };
            var nq1 = from ct in bq
                      join r in Route on ct.Tour.Route equals r
                      join hr in Hotel_Route on r equals hr.Route
                      let hq = Hotel.Count
                      where hr.Hotel.Id <= hq * 0.1m
                      select ct.Client;
            var nq2 = from ct in bq
                      join r in Route on ct.Tour.Route equals r
                      join pr in Place_Route on r equals pr.Route
                      let pq = Place.Count
                      where pr.Place.Id <= pq * 0.2m
                      select ct.Client;
            var sq = from c in nq1.Intersect(nq2)
                     select c.Person.FullName();
            return sq;
        }
        /// <summary>
        /// Returns IEnumerable of person who are either clients, who visited tours which indexes are lower than 1/100 of total,
        ///  or guides who visited tours which indexes are greater than 99/100 of total
        /// </summary>
        public IEnumerable<string> SelfQuery_6() 
        {
            var cp = from ct in Client_Tour
                     let tq = Tour.Count
                     where ct.Tour.Id < tq / 100
                     select ct.Client.Person.FullName();
            var gp = from g in Guide
                     join t in Tour on g equals t.Route.Guide
                     let tq = Tour.Count
                     where t.Route.Id > 99 * tq / 100
                     select g.Person.FullName();
            var cgp = cp.Union(gp);
            return cgp;
        }
        /// <summary>
        /// Best cities by average hotel rating
        /// </summary>
        public void SelfQuery_7() {
            var sq = (from l in Location
                      join hr in Hotel_Rating on l equals hr.Hotel.Location
                      group hr by l.City into CityHotelGroup
                      let ar = CityHotelGroup.Average(chg => chg.Rating)
                      orderby ar descending
                      select new
                      {
                          City = CityHotelGroup.Key,
                          AverageRating = $"{Math.Round(ar, 2)} / 5"
                      }).Take(5);
            ShowQuery("Self Query 7", sq, new Dictionary<string, string>
            {
                { "City", "Місто" },
                { "AverageRating", "Оцінка готелів" }
            });
        }
        /// <summary>
        /// Returns most commonly visited city
        /// </summary>
        public void SelfQuery_8() 
        {
            var sq = (from rl in Hotel_Route.Select(hr => new { hr.Route, hr.Hotel.Location })
                          .Union(Place_Route.Select(pr => new { pr.Route, pr.Place.Location }))
                      join t in Tour on rl.Route equals t.Route
                      group t by rl.Location into TLG //Tour Location Group
                      let cv = TLG.Count()
                      orderby cv descending, TLG.Key.Id ascending
                      select new
                      {
                          TLG.Key.City,
                          TotalVisit = cv
                      }).Take(3);
            ShowQuery("Self Query 8", sq, new Dictionary<string, string>
            {
                { "City", "Місто" },
                { "TotalVisit", "Кількість відвідувань" }
            });
        }
        /// <summary>
        /// Most popular route among last 100 clients
        /// </summary>
        public void SelfQuery_9()
        {
            var sq = (from ct in Client_Tour
                      let cq = Client.Count
                      where cq - 100 < ct.Client.Id && ct.Client.Id <= cq
                      group ct by ct.Tour.Route into ClientRouteGroup
                      let rc = ClientRouteGroup.Count()
                      orderby rc descending, ClientRouteGroup.Key.Id ascending
                      select new
                      {
                          Route = ClientRouteGroup.Key.Id,
                          Total = rc
                      }).Take(3);
            ShowQuery("Self Query 9", sq, new Dictionary<string, string>
            {
                { "Route", "Маршрут" },
                { "Total", "Кількість турів" }
            });
        }
        /// <summary>
        /// Least popular hotel among guides with less than 1 year experience
        /// </summary>
        public void SelfQuery_10()
        {
            var sq = (from hr in Hotel_Route
                     join t in Tour.Where(t => t.Route.Guide.Experience < 2) on hr.Route equals t.Route
                     group t by hr.Hotel into HotelVisitGroup
                     let hv = HotelVisitGroup.Count()
                     orderby hv ascending, HotelVisitGroup.Key.Id ascending
                     select new
                     {
                         Hotel = HotelVisitGroup.Key.Id,
                         TotalVisit = hv
                     }).Take(5);
            ShowQuery("SelfQuery 10", sq, new Dictionary<string, string>
            {
                { "Hotel", "Готель" },
                { "TotalVisit", "Кількість відвідувань"}
            });
        }
        public void TaskQuery_1()
        {
            var tq = from g in Guide
                     join r in Route on g equals r.Guide
                     join t in Tour on r equals t.Route
                     join ct in Client_Tour on t equals ct.Tour
                     join gr in Guide_Rating on g equals gr.Guide
                     group new { ct, gr } by g into GuideGroup
                     let gc = GuideGroup.Select(gg => gg.ct.Client).Distinct().Count()
                     let tc = (decimal)Client.Count
                     let rs = Route.Where(r => r.Guide == GuideGroup.Key)
                     let cr = Math.Round(gc / tc, 2)
                     let ar = Math.Round(GuideGroup.Average(gg => gg.gr.Rating), 2)
                     where ar > 0.9m * 5m && cr < 0.1m
                     select new
                     {
                         Route = string.Join(", ", rs.Select(r => r.Id)),
                         ChoiceRatio = cr,
                         AverageRating = $"{Math.Round(ar, 2)} / 5"
                     };
            ShowQuery("Task Query 1", tq, new Dictionary<string, string>
            {
                { "Route", "Тури" },
                { "ChoiceRatio", "Вірогідність обрання" },
                { "AverageRating", "Оцінка гідів" }
            });
        }
        public void TaskQuery_2()
        {
            var tq = (from hr in Hotel_Route
                      join t in Tour on hr.Route equals t.Route
                      group t by hr.Hotel into HotelVisit
                      let v = HotelVisit.Count()
                      orderby v descending, HotelVisit.Key.Id ascending
                      select new
                      {
                          Hotel = HotelVisit.Key.Id,
                          TotalVisit = v
                      }).First();
            ShowQuery("Task Query 2", [tq], new Dictionary<string, string>
            {
                { "Hotel", "Готель"},
                { "TotalVisit", "Кількість груп"}
            });
        }
        public void TaskQuery_3()
        {
            var tq = from g in Guide
                     join t in Tour on g equals t.Route.Guide
                     join gr in Guide_Rating on g equals gr.Guide
                     group new { t, gr } by g into GuideGroup
                     let gt = GuideGroup.Select(gg => gg.t).Distinct().Count()
                     let ar = Math.Round(GuideGroup.Average(gg => gg.gr.Rating), 2)
                     where gt > 10 && ar > 4.5m
                     orderby GuideGroup.Key.Id ascending, gt descending, ar descending
                     select new
                     {
                         Guide = GuideGroup.Key.Id,
                         TotalTour = gt,
                         AverageRating = $"{Math.Round(ar, 2)} / 5"
                     };
            ShowQuery("Task Query 3", tq, new Dictionary<string, string>
            {
                { "Guide", "Гід" },
                { "TotalTour", "Кількість груп" },
                { "AverageRating", "Оцінка гіду" }
            });
        }
        public void TaskQuery_4() {
            var tq = (from hro in Hotel_Route
                      join hra in Hotel_Rating on hro.Hotel equals hra.Hotel
                      join ct in Client_Tour on hro.Route equals ct.Tour.Route
                      group new { hra, ct } by hro.Route into HotelRatingGroup
                      let ar = HotelRatingGroup.Average(hrg => hrg.hra.Rating)
                      let rc = HotelRatingGroup.Select(hrg => hrg.ct.Client).Distinct().Count()
                      where ar > 4m && rc > 50 && HotelRatingGroup.Key.Duration > 7
                      orderby rc descending, ar descending
                      select new
                      {
                          Route = HotelRatingGroup.Key.Id,
                          TotalClient = rc,
                          AverageRating = $"{Math.Round(ar, 2)} / 5"
                      }).Take(3);
            ShowQuery("Task Query 4", tq, new Dictionary<string, string>
            {
                { "Route", "Маршрут" },
                { "TotalClient", "Кількість клієнтів" },
                { "AverageRating", "Оцінка готелів" }
            });
        }
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

            qgen.SelfQuery_1();

            //var result = qgen.SelfQuery_5();
            //foreach (var r in result)
            //{
            //    Console.WriteLine(r);
            //}
        }        
    }
}
