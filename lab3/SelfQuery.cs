using Lab1.Class;
using System.Text.Json;
using System.Text.Json.Nodes;
namespace Lab3
{
    public partial class LINQToJSONQuery 
    {
        public void SelfQuery_1() 
        {
            var mode = GetDeserializationMode();
            var path = Path.Combine(DataPath, "ClientTour.json");
            using var sr = new StreamReader(path);

            var ClientTour = mode switch
            {
                1 => JsonSerializer.Deserialize<IEnumerable<ClientTour>>(sr.ReadToEnd())!
                    .Select(ct => new
                    {
                        Guide = ct.Tour.Route.Guide.Id,
                        ct.Tour.Route.Guide.Experience,
                        Client = ct.Client.Id
                    }),
                2 => JsonDocument.Parse(sr.ReadToEnd()).RootElement.EnumerateArray()
                    .Select(ct => new
                    {
                        Guide = ct.GetProperty("Tour").GetProperty("Route").GetProperty("Guide").GetProperty("Id").GetInt32(),
                        Experience = ct.GetProperty("Tour").GetProperty("Route").GetProperty("Guide").GetProperty("Experience").GetInt32(),
                        Client = ct.GetProperty("Client").GetProperty("Id").GetInt32()
                    }),
                3 => JsonNode.Parse(sr.ReadToEnd())!.AsArray()
                    .Select(ct => new
                    {
                        Guide = (int)ct!["Tour"]!["Route"]!["Guide"]!["Id"]!,
                        Experience = (int)ct!["Tour"]!["Route"]!["Guide"]!["Experience"]!,
                        Client = (int)ct!["Client"]!["Id"]!
                    }),
                _ => throw new ArgumentException("Unexpected argument value encountered")
            };

            var sq = (from ct in ClientTour
                      group ct.Client by new { ct.Guide, ct.Experience } into GuideClientGroup
                      let gc = GuideClientGroup.Count()
                      orderby GuideClientGroup.Key.Experience descending, gc descending
                      select new
                      {
                          GuideClientGroup.Key.Guide,
                          GuideClientGroup.Key.Experience,
                          TotalClient = gc
                      }).Take(3);

            ShowQuery("Self Query 1", sq, new Dictionary<string, string>
            {
                { "Guide", "Гід" },
                { "Experience", "Стаж" },
                { "TotalClient", "Кількість клієнтів" }
            });
        }
        public void SelfQuery_2()
        {
            var mode = GetDeserializationMode();
            var path = Path.Combine(DataPath, "ClientTour.json");
            using var sr1 = new StreamReader(path);

            var ClientTour = mode switch
            {
                1 => JsonSerializer.Deserialize<IEnumerable<ClientTour>>(sr1.ReadToEnd())!
                    .Select(ct => new
                    {
                        Route = ct.Tour.Route.Id,
                        Client = ct.Client.Id
                    }),
                2 => JsonDocument.Parse(sr1.ReadToEnd()).RootElement.EnumerateArray()
                    .Select(ct => new
                    {
                        Route = ct.GetProperty("Tour").GetProperty("Route").GetProperty("Id").GetInt32(),
                        Client = ct.GetProperty("Client").GetProperty("Id").GetInt32()
                    }),
                3 => JsonNode.Parse(sr1.ReadToEnd())!.AsArray()
                    .Select(ct => new
                    {
                        Route = (int)ct!["Tour"]!["Route"]!["Id"]!,
                        Client = (int)ct!["Client"]!["Id"]!
                    }),
                _ => throw new ArgumentException("Unexpected argument value encountered")
            };

            path = Path.Combine(DataPath, "PlaceRoute.json");
            using var sr2 = new StreamReader(path);

            var PlaceRoute = mode switch
            {
                1 => JsonSerializer.Deserialize<IEnumerable<PlaceRoute>>(sr2.ReadToEnd())!
                    .Select(pr => new
                    {
                        Route = pr.Route.Id,
                        Place = pr.Place.Id
                    }),
                2 => JsonDocument.Parse(sr2.ReadToEnd()).RootElement.EnumerateArray()
                    .Select(pr => new
                    {
                        Route = pr.GetProperty("Route").GetProperty("Id").GetInt32(),
                        Place = pr.GetProperty("Place").GetProperty("Id").GetInt32()
                    }),
                3 => JsonNode.Parse(sr2.ReadToEnd())!.AsArray()
                    .Select(pr => new
                    {
                        Route = (int)pr!["Route"]!["Id"]!,
                        Place = (int)pr!["Place"]!["Id"]!
                    }),
                _ => throw new ArgumentException("Unexpected argument value encountered")
            };

            var sq = (from rc in ClientTour
                      join rp in PlaceRoute on rc.Route equals rp.Route
                      group rc.Client by rp.Place into PlaceClientGroup
                      let pc = PlaceClientGroup.Count()
                      orderby pc descending, PlaceClientGroup.Key ascending
                      select new
                      {
                          Place = PlaceClientGroup.Key,
                          TotalClient = pc
                      }).Take(5);

            ShowQuery("Self Query 2", sq, new Dictionary<string, string>
            {
                { "Place", "Місце" },
                { "TotalClient", "Кількість відвідувань" }
            });
        }
        public void SelfQuery_3()
        {
            var mode = GetDeserializationMode();
            var path = Path.Combine(DataPath, "HotelRating.json");
            using var sr = new StreamReader(path);

            var HotelRating = mode switch
            {
                1 => JsonSerializer.Deserialize<IEnumerable<HotelRating>>(sr.ReadToEnd())!.Select(hr => hr.Rating),
                2 => JsonDocument.Parse(sr.ReadToEnd()).RootElement.EnumerateArray().Select(hr => hr.GetProperty("Rating").GetDecimal()),
                3 => JsonNode.Parse(sr.ReadToEnd())!.AsArray().Select(hr => (decimal)hr!["Rating"]!),
                _ => throw new ArgumentException("Unexpected argument value encountered")
            };

            var sq = from r in HotelRating
                     group r by r into RatingGroup
                     let tr = (decimal)(HotelRating.Count())
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
        public void SelfQuery_4()
        {
            var mode = GetDeserializationMode();
            var path = Path.Combine(DataPath, "Route.json");
            using var sr = new StreamReader(path);

            var Route = mode switch
            {
                1 => JsonSerializer.Deserialize<IEnumerable<Route>>(sr.ReadToEnd())!
                    .Select(r => new
                    {
                        r.Duration,
                        r.Id
                    }),
                2 => JsonDocument.Parse(sr.ReadToEnd()).RootElement.EnumerateArray()
                    .Select(r => new
                    {
                        Duration = r.GetProperty("Duration").GetInt32(),
                        Id = r.GetProperty("Id").GetInt32()
                    }),
                3 => JsonNode.Parse(sr.ReadToEnd())!.AsArray()
                    .Select(r => new
                    {
                        Duration = (int)r!["Duration"]!,
                        Id = (int)r!["Id"]!
                    }),
                _ => throw new ArgumentException("Unexpected argument value encountered")
            };

            var sq = from r in Route
                     group r.Id by r.Duration into DurationGroup
                     orderby DurationGroup.Key descending
                     let ids = DurationGroup.Select(dg => dg.ToString()).Aggregate((curr, next) => $"{curr} {next}")
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
        public void SelfQuery_5()
        {
            var mode = GetDeserializationMode();
            var path = Path.Combine(DataPath, "HotelRating.json");
            using var sr = new StreamReader(path);

            var HotelRating = mode switch
            {
                1 => JsonSerializer.Deserialize<IEnumerable<HotelRating>>(sr.ReadToEnd())!
                    .Select(hr => new
                    {
                        HotelCity = hr.Hotel.Location.City,
                        hr.Rating
                    }),
                2 => JsonDocument.Parse(sr.ReadToEnd()).RootElement.EnumerateArray()
                    .Select(hr => new
                    {
                        HotelCity = hr.GetProperty("Hotel").GetProperty("Location").GetProperty("City").GetString()!,
                        Rating = hr.GetProperty("Rating").GetDecimal()
                    }),
                3 => JsonNode.Parse(sr.ReadToEnd())!.AsArray()
                    .Select(hr => new
                    {
                        HotelCity = (string)hr!["Hotel"]!["Location"]!["City"]!,
                        Rating = (decimal)hr!["Rating"]!
                    }),
                _ => throw new ArgumentException("Unexpected argument value encountered")
            };

            var sq = (from hr in HotelRating
                      group hr.Rating by hr.HotelCity into CityHotelGroup
                      let ar = CityHotelGroup.Average(chg => chg)
                      orderby ar descending
                      select new
                      {
                          City = CityHotelGroup.Key,
                          AverageRating = $"{Math.Round(ar, 2)} / 5"
                      }).Take(5);

            ShowQuery("Self Query 5", sq, new Dictionary<string, string>
            {
                { "City", "Місто" },
                { "AverageRating", "Оцінка готелів" }
            });
        }
        public void SelfQuery_6()
        {
            var mode = GetDeserializationMode();
            var path = Path.Combine(DataPath, "ClientTour.json");
            using var sr1 = new StreamReader(path);

            var ClientTour = mode switch
            {
                1 => JsonSerializer.Deserialize<IEnumerable<ClientTour>>(sr1.ReadToEnd())!
                    .Select(ct => new
                    {
                        Client = ct.Client.Id,
                        Route = ct.Tour.Route.Id
                    }),
                2 => JsonDocument.Parse(sr1.ReadToEnd()).RootElement.EnumerateArray()
                    .Select(ct => new
                    {
                        Client = ct.GetProperty("Client").GetProperty("Id").GetInt32(),
                        Route = ct.GetProperty("Tour").GetProperty("Route").GetProperty("Id").GetInt32()
                    }),
                3 => JsonNode.Parse(sr1.ReadToEnd())!.AsArray()
                    .Select(ct => new
                    {
                        Client = (int)ct!["Client"]!["Id"]!,
                        Route = (int)ct!["Tour"]!["Route"]!["Id"]!
                    }),
                _ => throw new ArgumentException("Unexpected argument value encountered")
            };

            path = Path.Combine(DataPath, "Client.json");
            using var sr2 = new StreamReader(path);
            var Client = JsonSerializer.Deserialize<IEnumerable<Client>>(sr2.ReadToEnd())!;

            var sq = (from ct in ClientTour
                      let cq = Client.Count()
                      where cq - 100 < ct.Client && ct.Client <= cq
                      group ct.Client by ct.Route into ClientRouteGroup
                      let rc = ClientRouteGroup.Count()
                      orderby rc descending, ClientRouteGroup.Key ascending
                      select new
                      {
                          Route = ClientRouteGroup.Key,
                          Total = rc
                      }).Take(3);

            ShowQuery("Self Query 6", sq, new Dictionary<string, string>
            {
                { "Route", "Маршрут" },
                { "Total", "Кількість туристів" }
            });
        }
        public void SelfQuery_7() {
            var mode = GetDeserializationMode();
            var path = Path.Combine(DataPath, "HotelRoute.json");
            using var sr1 = new StreamReader(path);

            var HotelRoute = mode switch
            {
                1 => JsonSerializer.Deserialize<IEnumerable<HotelRoute>>(sr1.ReadToEnd())!
                    .Select(hr => new
                    {
                        Hotel = hr.Hotel.Id,
                        Route = hr.Route.Id,
                        hr.Route.Guide.Experience
                    }),
                2 => JsonDocument.Parse(sr1.ReadToEnd()).RootElement.EnumerateArray()
                    .Select(hr => new
                    {
                        Hotel = hr.GetProperty("Hotel").GetProperty("Id").GetInt32(),
                        Route = hr.GetProperty("Route").GetProperty("Id").GetInt32(),
                        Experience = hr.GetProperty("Route").GetProperty("Guide").GetProperty("Experience").GetInt32()
                    }),
                3 => JsonNode.Parse(sr1.ReadToEnd())!.AsArray()
                    .Select(hr => new
                    {
                        Hotel = (int)hr!["Hotel"]!["Id"]!,
                        Route = (int)hr!["Route"]!["Id"]!,
                        Experience = (int)hr!["Route"]!["Guide"]!["Experience"]!
                    }),
                _ => throw new ArgumentException("Unexpected argument value encountered")
            };

            path = Path.Combine(DataPath, "Tour.json");
            using var sr2 = new StreamReader(path);

            var Tour = mode switch
            {
                1 => JsonSerializer.Deserialize<IEnumerable<Tour>>(sr2.ReadToEnd())!.Select(t => t.Route.Id),
                2 => JsonDocument.Parse(sr2.ReadToEnd()).RootElement.EnumerateArray().Select(t => t.GetProperty("Route").GetProperty("Id").GetInt32()),
                3 => JsonNode.Parse(sr2.ReadToEnd())!.AsArray().Select(t => (int)t!["Route"]!["Id"]!),
                _ => throw new ArgumentException("Unexpected argument value encountered")
            };

            var sq = (from hr in HotelRoute
                      join t in Tour on hr.Route equals t
                      where hr.Experience < 2
                      group t by hr.Hotel into HotelVisitGroup
                      let hv = HotelVisitGroup.Count()
                      orderby hv descending, HotelVisitGroup.Key ascending
                      select new
                      {
                          Hotel = HotelVisitGroup.Key,

                          TotalVisit = hv
                      }).Take(5);

            ShowQuery("SelfQuery 7", sq, new Dictionary<string, string>
            {
                { "Hotel", "Готель" },
                { "TotalVisit", "Кількість відвідувань"}
            });
        }
    }
}
