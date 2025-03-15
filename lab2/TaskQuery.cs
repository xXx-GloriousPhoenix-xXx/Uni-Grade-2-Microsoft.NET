using System.Xml.Linq;
using System.Xml.Serialization;
using Lab1.Class;

namespace Lab2 {
    public partial class LINQToXMLQuery 
    {
        public void TaskQuery_1()
        {
            var mode = GetDeserializationMode();
            var path = Path.Combine(DataPath, "ClientTour.xml");
            var ClientTour = mode switch
            {
                1 => (new XmlSerializer(typeof(List<ClientTour>))
                    .Deserialize(new StreamReader(path)) as IEnumerable<ClientTour>)!
                    .Select(ct => new
                    {
                        Guide = ct.Tour.Route.Guide.Id,
                        Route = ct.Tour.Route.Id,
                        Client = ct.Client.Id
                    }),
                2 => XDocument
                    .Load(path)
                    .Descendants("ClientTour")
                    .Select(ct => new
                    {
                        Guide = (int)ct.Element("Tour")!.Element("Route")!.Element("Guide")!.Element("Id")!,
                        Route = (int)ct.Element("Tour")!.Element("Route")!.Element("Id")!,
                        Client = (int)ct.Element("Client")!.Element("Id")!
                    }),
                _ => throw new ArgumentException("Unexpected argument value encountered")
            };
            path = Path.Combine(DataPath, "GuideRating.xml");
            var GuideRating = mode switch
            {
                1 => (new XmlSerializer(typeof(List<GuideRating>))
                    .Deserialize(new StreamReader(path)) as IEnumerable<GuideRating>)!
                    .Select(gr => new
                    {
                        Guide = gr.Guide.Id,
                        gr.Rating
                    }),
                2 => XDocument
                    .Load(path)
                    .Descendants("GuideRating")
                    .Select(gr => new
                    {
                        Guide = (int)gr.Element("Guide")!.Element("Id")!,
                        Rating = (decimal)gr.Element("Rating")!
                    }),
                _ => throw new ArgumentException("Unexpected argument value encountered")
            };
            path = Path.Combine(DataPath, "Client.xml");
            var Client = XDocument.Load(path).Descendants("Client");
            var tq = from ct in ClientTour
                     join gr in GuideRating on ct.Guide equals gr.Guide
                     group new { ct.Client, gr.Rating } by gr.Guide into GuideGroup
                     let gc = GuideGroup.Select(gg => gg.Client).Distinct().Count()
                     let tc = (decimal)Client.Count()
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
            var path = Path.Combine(DataPath, "HotelRoute.xml");
            var HotelRoute = mode switch
            {
                1 => (new XmlSerializer(typeof(List<HotelRoute>))
                    .Deserialize(new StreamReader(path)) as IEnumerable<HotelRoute>)!
                    .Select(hr => new
                    {
                        Hotel = hr.Hotel.Id,
                        Route = hr.Route.Id,
                    }),
                2 => XDocument
                    .Load(path)
                    .Descendants("HotelRoute")
                    .Select(hr => new
                    {
                        Hotel = (int)hr.Element("Hotel")!.Element("Id")!,
                        Route = (int)hr.Element("Route")!.Element("Id")!
                    }),
                _ => throw new ArgumentException("Unexpected argument value encountered")
            };
            path = Path.Combine(DataPath, "Tour.xml");
            var Tour = mode switch
            {
                1 => (new XmlSerializer(typeof(List<Tour>))
                    .Deserialize(new StreamReader(path)) as IEnumerable<Tour>)!
                    .Select(t => t.Route.Id),
                2 => XDocument
                    .Load(path)
                    .Descendants("Tour")
                    .Select(t => (int)t.Element("Route")!.Element("Id")!),
                _ => throw new ArgumentException("Unexpected argument value encountered")
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
        public void TaskQuery_3() {
            var mode = GetDeserializationMode();
            var path = Path.Combine(DataPath, "Tour.xml");
            var Tour = mode switch
            {
                1 => (new XmlSerializer(typeof(List<Tour>))
                    .Deserialize(new StreamReader(path)) as IEnumerable<Tour>)!
                    .Select(t => new
                    {
                        Tour = t.Id,
                        Guide = t.Route.Guide.Id
                    }),
                2 => XDocument
                    .Load(path)
                    .Descendants("Tour")
                    .Select(t => new
                    {
                        Tour = (int)t.Element("Id")!,
                        Guide = (int)t.Element("Route")!.Element("Guide")!.Element("Id")!
                    }),
                _ => throw new ArgumentException("Unexpected argument value encoutnered")
            };
            path = Path.Combine(DataPath, "GuideRating.xml");
            var GuideRating = mode switch
            {
                1 => (new XmlSerializer(typeof(List<GuideRating>))
                    .Deserialize(new StreamReader(path)) as IEnumerable<GuideRating> as IEnumerable<GuideRating>)!
                    .Select(gr => new
                    {
                        Guide = gr.Guide.Id,
                        gr.Rating
                    }),
                2 => XDocument
                    .Load(path)
                    .Descendants("GuideRating")
                    .Select(gr => new
                    {
                        Guide = (int)gr.Element("Guide")!.Element("Id")!,
                        Rating = (decimal)gr.Element("Rating")!
                    }),
                _ => throw new ArgumentException("Unexpected argument value encoutnered")
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
        public void TaskQuery_4() {
            var mode = GetDeserializationMode();
            var path = Path.Combine(DataPath, "HotelRating.xml");
            var HotelRating = mode switch
            {
                1 => (new XmlSerializer(typeof(List<HotelRating>))
                    .Deserialize(new StreamReader(path)) as IEnumerable<HotelRating>)!
                    .Select(hr => new
                    {
                        Hotel = hr.Hotel.Id,
                        hr.Rating
                    }),
                2 => XDocument
                    .Load(path)
                    .Descendants("HotelRating")
                    .Select(hr => new
                    {
                        Hotel = (int)hr.Element("Hotel")!.Element("Id")!,
                        Rating = (decimal)hr.Element("Rating")!
                    }),
                _ => throw new ArgumentException("Unexpected argument value encountered")
            };
            path = Path.Combine(DataPath, "HotelRoute.xml");
            var HotelRoute = mode switch
            {
                1 => (new XmlSerializer(typeof(List<HotelRoute>))
                    .Deserialize(new StreamReader(path)) as IEnumerable<HotelRoute>)!
                    .Select(hr => new
                    {
                        Hotel = hr.Hotel.Id,
                        Route = hr.Route.Id,
                        hr.Route.Duration
                    }),
                2 => XDocument
                    .Load(path)
                    .Descendants("HotelRoute")
                    .Select(hr => new
                    {
                        Hotel = (int)hr.Element("Hotel")!.Element("Id")!,
                        Route = (int)hr.Element("Route")!.Element("Id")!,
                        Duration = (int)hr.Element("Route")!.Element("Duration")!
                    }),
                _ => throw new ArgumentException("Unexpected argument value encountered")
            };
            path = Path.Combine(DataPath, "ClientTour.xml");
            var ClientTour = mode switch
            {
                1 => (new XmlSerializer(typeof(List<ClientTour>))
                    .Deserialize(new StreamReader(path)) as IEnumerable<ClientTour>)!
                    .Select(ct => new
                    {
                        Client = ct.Client.Id,
                        Route = ct.Tour.Route.Id
                    }),
                2 => XDocument
                    .Load(path)
                    .Descendants("ClientTour")
                    .Select(ct => new
                    {
                        Client = (int)ct.Element("Client")!.Element("Id")!,
                        Route = (int)ct.Element("Tour")!.Element("Route")!.Element("Id")!
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
