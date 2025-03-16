using Lab1.Class;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Lab3
{
    public partial class LINQToJSONQuery
    {
        public void TaskQuery_1()
        {
            var mode = GetDeserializationMode();
            var path = Path.Combine(DataPath, "ClientTour.json");
            using var sr1 = new StreamReader(path);

            var ClientTour = mode switch
            {
                1 => JsonSerializer.Deserialize<IEnumerable<ClientTour>>(sr1.ReadToEnd())!
                    .Select(ct => new
                    {
                        Guide = ct.Tour.Route.Guide.Id,
                        Route = ct.Tour.Route.Id,
                        Client = ct.Client.Id
                    }),
                2 => JsonDocument.Parse(sr1.ReadToEnd()).RootElement.EnumerateArray()
                    .Select(ct => new
                    {
                        Guide = ct.GetProperty("Tour").GetProperty("Route").GetProperty("Guide").GetProperty("Id").GetInt32(),
                        Route = ct.GetProperty("Tour").GetProperty("Route").GetProperty("Id").GetInt32(),
                        Client = ct.GetProperty("Client").GetProperty("Id").GetInt32()
                    }),
                3 => JsonNode.Parse(sr1.ReadToEnd())!.AsArray()
                    .Select(ct => new
                    {
                        Guide = (int)ct!["Tour"]!["Route"]!["Guide"]!["Id"]!,
                        Route = (int)ct!["Tour"]!["Route"]!["Id"]!,
                        Client = (int)ct!["Client"]!["Id"]!
                    }),
                _ => throw new ArgumentException("Unexpected argument value encountered")
            };

            path = Path.Combine(DataPath, "GuideRating.json");
            using var sr2 = new StreamReader(path);

            var GuideRating = mode switch
            {
                1 => JsonSerializer.Deserialize<IEnumerable<GuideRating>>(sr2.ReadToEnd())!
                    .Select(gr => new
                    {
                        Guide = gr.Guide.Id,
                        gr.Rating
                    }),
                2 => JsonDocument.Parse(sr2.ReadToEnd()).RootElement.EnumerateArray()
                    .Select(gr => new
                    {
                        Guide = gr.GetProperty("Guide").GetProperty("Id").GetInt32(),
                        Rating = gr.GetProperty("Rating").GetDecimal()
                    }),
                3 => JsonNode.Parse(sr2.ReadToEnd())!.AsArray()
                    .Select(gr => new
                    {
                        Guide = (int)gr!["Guide"]!["Id"]!,
                        Rating = (decimal)gr!["Rating"]!
                    }),
                _ => throw new ArgumentException("Unexpected argument value encountered")
            };

            path = Path.Combine(DataPath, "Client.json");
            using var sr3 = new StreamReader(path);
            var Client = JsonSerializer.Deserialize<IEnumerable<Client>>(sr3.ReadToEnd());

            var tq = from ct in ClientTour
                     join gr in GuideRating on ct.Guide equals gr.Guide
                     group new { ct.Client, gr.Rating } by gr.Guide into GuideGroup
                     let gc = GuideGroup.Select(gg => gg.Client).Distinct().Count()
                     let tc = (decimal)Client!.Count()
                     let rs = ClientTour.Where(ct => ct.Guide == GuideGroup.Key).DistinctBy(ct => ct.Route).OrderBy(ct => ct.Route)
                     let cr = Math.Round(gc / tc, 2)
                     let ar = Math.Round(GuideGroup.Average(gg => gg.Rating), 2)
                     where ar > 0.9m * 5m && cr < 0.1m
                     select new
                     {
                         Route = string.Join(", ", rs.Select(rs => rs.Route)),
                         ChoiceRatio = cr,
                         AverageRating = $"{Math.Round(ar, 2)} / 5"
                     };

            ShowQuery("Task Query 1", tq, new Dictionary<string, string>
            {
                { "Route", "Маршрути" },
                { "ChoiceRatio", "Вірогідність обрання" },
                { "AverageRating", "Оцінка гідів" }
            });
        }
        public void TaskQuery_2()
        {
            var mode = GetDeserializationMode();
            var path = Path.Combine(DataPath, "HotelRoute.json");
            using var sr1 = new StreamReader(path);

            var HotelRoute = mode switch
            {
                1 => JsonSerializer.Deserialize<IEnumerable<HotelRoute>>(sr1.ReadToEnd())!
                    .Select(hr => new
                    {
                        Hotel = hr.Hotel.Id,
                        Route = hr.Route.Id
                    }),
                2 => JsonDocument.Parse(sr1.ReadToEnd()).RootElement.EnumerateArray()
                    .Select(hr => new
                    {
                        Hotel = hr.GetProperty("Hotel").GetProperty("Id").GetInt32(),
                        Route = hr.GetProperty("Route").GetProperty("Id").GetInt32()
                    }),
                3 => JsonNode.Parse(sr1.ReadToEnd())!.AsArray()
                    .Select(hr => new
                    {
                        Hotel = (int)hr!["Hotel"]!["Id"]!,
                        Route = (int)hr!["Route"]!["Id"]!
                    }),
                _ => throw new ArgumentException("Unexpected arument value encountered")
            };

            path = Path.Combine(DataPath, "Tour.json");
            using var sr2 = new StreamReader(path);

            var Tour = mode switch
            {
                1 => JsonSerializer.Deserialize<IEnumerable<Tour>>(sr2.ReadToEnd())!.Select(t => t.Route.Id),
                2 => JsonDocument.Parse(sr2.ReadToEnd()).RootElement.EnumerateArray().Select(t => t.GetProperty("Route").GetProperty("Id").GetInt32()),
                3 => JsonNode.Parse(sr2.ReadToEnd())!.AsArray().Select(t => (int)t!["Route"]!["Id"]!),
                _ => throw new ArgumentException("Unexpected arument value encountered")
            };

            var tq = (from hr in HotelRoute
                      join t in Tour on hr.Route equals t
                      group t by hr.Hotel into HotelVisit
                      let v = HotelVisit.Count()
                      orderby v descending, HotelVisit.Key ascending
                      select new
                      {
                          Hotel = HotelVisit.Key,
                          TotalVisit = v
                      }).First();

            ShowQuery("Task Query 2", [tq], new Dictionary<string, string>
            {
                { "Hotel", "Готель" },
                { "TotalVisit", "Кількість груп" }
            });
        }
        public void TaskQuery_3()
        {
            var mode = GetDeserializationMode();
            var path = Path.Combine(DataPath, "Tour.json");
            using var sr1 = new StreamReader(path);

            var Tour = mode switch
            {
                1 => JsonSerializer.Deserialize<IEnumerable<Tour>>(sr1.ReadToEnd())!
                    .Select(t => new
                    {
                        Tour = t.Id,
                        Guide = t.Route.Guide.Id
                    }),
                2 => JsonDocument.Parse(sr1.ReadToEnd()).RootElement.EnumerateArray()
                    .Select(t => new
                    {
                        Tour = t.GetProperty("Id").GetInt32(),
                        Guide = t.GetProperty("Route").GetProperty("Guide").GetProperty("Id").GetInt32()
                    }),
                3 => JsonNode.Parse(sr1.ReadToEnd())!.AsArray()
                    .Select(t => new
                    {
                        Tour = (int)t!["Id"]!,
                        Guide = (int)t!["Route"]!["Guide"]!["Id"]!
                    }),
                _ => throw new ArgumentException("Unexpected argument value encountered")
            };

            path = Path.Combine(DataPath, "GuideRating.json");
            using var sr2 = new StreamReader(path);

            var GuideRating = mode switch
            {
                1 => JsonSerializer.Deserialize<IEnumerable<GuideRating>>(sr2.ReadToEnd())!
                    .Select(gr => new
                    {
                        Guide = gr.Guide.Id,
                        gr.Rating
                    }),
                2 => JsonDocument.Parse(sr2.ReadToEnd()).RootElement.EnumerateArray()
                    .Select(gr => new
                    {
                        Guide = gr.GetProperty("Guide").GetProperty("Id").GetInt32(),
                        Rating = gr.GetProperty("Rating").GetDecimal()
                    }),
                3 => JsonNode.Parse(sr2.ReadToEnd())!.AsArray()
                    .Select(gr => new
                    {
                        Guide = (int)gr!["Guide"]!["Id"]!,
                        Rating = (decimal)gr!["Rating"]!
                    }),
                _ => throw new ArgumentException("Unexpected argument value encountered")
            };

            var tq = from t in Tour
                     join gr in GuideRating on t.Guide equals gr.Guide
                     group new { t.Tour, gr.Rating } by gr.Guide into GuideGroup
                     let gt = GuideGroup.Select(gg => gg.Tour).Distinct().Count()
                     let ar = Math.Round(GuideGroup.Average(gg => gg.Rating), 2)
                     where gt > 10 && ar > 4.5m
                     orderby GuideGroup.Key ascending, gt descending, ar descending
                     select new
                     {
                         Guide = GuideGroup.Key,
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
        public void TaskQuery_4()
        {
            var mode = GetDeserializationMode();
            var path = Path.Combine(DataPath, "HotelRating.json");
            using var sr1 = new StreamReader(path);

            var HotelRating = mode switch
            {
                1 => JsonSerializer.Deserialize<IEnumerable<HotelRating>>(sr1.ReadToEnd())!
                    .Select(hr => new
                    {
                        Hotel = hr.Hotel.Id,
                        hr.Rating
                    }),
                2 => JsonDocument.Parse(sr1.ReadToEnd()).RootElement.EnumerateArray()
                    .Select(hr => new
                    {
                        Hotel = hr.GetProperty("Hotel").GetProperty("Id").GetInt32(),
                        Rating = hr.GetProperty("Rating").GetDecimal()
                    }),
                3 => JsonNode.Parse(sr1.ReadToEnd())!.AsArray()
                    .Select(hr => new
                    {
                        Hotel = (int)hr!["Hotel"]!["Id"]!,
                        Rating = (decimal)hr!["Rating"]!
                    }),
                _ => throw new ArgumentException("Unexpected argument value encountered")
            };

            path = Path.Combine(DataPath, "HotelRoute.json");
            using var sr2 = new StreamReader(path);

            var HotelRoute = mode switch
            {
                1 => JsonSerializer.Deserialize<IEnumerable<HotelRoute>>(sr2.ReadToEnd())!
                    .Select(hr => new
                    {
                        Hotel = hr.Hotel.Id,
                        Route = hr.Route.Id,
                        hr.Route.Duration
                    }),
                2 => JsonDocument.Parse(sr2.ReadToEnd()).RootElement.EnumerateArray()
                    .Select(hr => new
                    {
                        Hotel = hr.GetProperty("Hotel").GetProperty("Id").GetInt32(),
                        Route = hr.GetProperty("Route").GetProperty("Id").GetInt32(),
                        Duration = hr.GetProperty("Route").GetProperty("Duration").GetInt32()
                    }),
                3 => JsonNode.Parse(sr2.ReadToEnd())!.AsArray()
                    .Select(hr => new
                    {
                        Hotel = (int)hr!["Hotel"]!["Id"]!,
                        Route = (int)hr!["Route"]!["Id"]!,
                        Duration = (int)hr!["Route"]!["Duration"]!
                    }),
                _ => throw new ArgumentException("Unexpected argument value encountered")
            };

            path = Path.Combine(DataPath, "ClientTour.json");
            using var sr3 = new StreamReader(path);

            var ClientTour = mode switch
            {
                1 => JsonSerializer.Deserialize<IEnumerable<ClientTour>>(sr3.ReadToEnd())!
                    .Select(ct => new
                    {
                        Client = ct.Client.Id,
                        Route = ct.Tour.Route.Id
                    }),
                2 => JsonDocument.Parse(sr3.ReadToEnd()).RootElement.EnumerateArray()
                    .Select(ct => new
                    {
                        Client = ct.GetProperty("Client").GetProperty("Id").GetInt32(),
                        Route = ct.GetProperty("Tour").GetProperty("Route").GetProperty("Id").GetInt32()
                    }),
                3 => JsonNode.Parse(sr3.ReadToEnd())!.AsArray()
                    .Select(ct => new
                    {
                        Client = (int)ct!["Client"]!["Id"]!,
                        Route = (int)ct!["Tour"]!["Route"]!["Id"]!
                    }),
                _ => throw new ArgumentException("Unexpected argument value encountered")
            };

            var tq = (from ct in ClientTour
                      join hro in HotelRoute on ct.Route equals hro.Route
                      join hra in HotelRating on hro.Hotel equals hra.Hotel
                      group new { hra, ct } by hro.Route into HotelRatingGroup
                      let ar = HotelRatingGroup.Average(hrg => hrg.hra.Rating)
                      let rc = HotelRatingGroup.Select(hrg => hrg.ct.Client).Distinct().Count()
                      let rd = HotelRoute.Where(hr => hr.Route == HotelRatingGroup.Key).Select(hro => hro.Duration).First()
                      where ar > 4m && rc > 50 && rd > 7
                      orderby rc descending, ar descending
                      select new
                      {
                          Route = HotelRatingGroup.Key,
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
}
