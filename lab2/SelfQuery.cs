using Lab1.Class;
using System.Linq;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Lab2
{
    public partial class LINQToXMLQuery 
    {
        public static int GetDeserializationMode() {
            Console.Clear();
            Console.WriteLine("Введіть спосіб десеріалізації (Des/Load)");
            var result = Console.ReadLine();
            Console.Clear();
            if (string.IsNullOrEmpty(result))
            {
                return GetDeserializationMode();
            }
            if (result == "Des")
            {
                return 1;
            }
            if (result == "Load")
            {
                return 2;
            }
            throw new ArgumentException("Unrecognized data entered");
        }
        public void SelfQuery_1() {
            var mode = GetDeserializationMode();
            var path = Path.Combine(DataPath, "ClientTour.xml");
            var ClientTour = mode switch
            {
                1 => (new XmlSerializer(typeof(List<ClientTour>))
                .Deserialize(new StreamReader(path)) as IEnumerable<ClientTour>)!
                .Select(ct => new
                {
                    Guide = ct.Tour.Route.Guide.Id,
                    ct.Tour.Route.Guide.Experience,
                    Client = ct.Client.Id
                }) as IEnumerable<dynamic>,
                2 => XDocument.Load(path)
                .Descendants("ClientTour")
                .Select(ct => new
                {
                    Guide = (int)ct.Element("Tour")!.Element("Route")!.Element("Guide")!.Element("Id")!,
                    Experience = (int)ct.Element("Tour")!.Element("Route")!.Element("Guide")!.Element("Experience")!,
                    Client = (int)ct.Element("Client")!.Element("Id")!
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
        public void SelfQuery_2() {
            var mode = GetDeserializationMode();
            var path = Path.Combine(DataPath, "ClientTour.xml");
            var ClientTour = mode switch
            {
                1 => (new XmlSerializer(typeof(List<ClientTour>))
                .Deserialize(new StreamReader(path)) as IEnumerable<ClientTour>)!
                .Select(ct => new
                {
                    Route = ct.Tour.Route.Id,
                    Client = ct.Client.Id
                }) as IEnumerable<dynamic>,
                2 => XDocument.Load(path)
                .Descendants("ClientTour")
                .Select(ct => new
                {
                    Route = ct.Element("Tour")!.Element("Route")!.Element("Id")!.Value,
                    Client = ct.Element("Client")!.Element("Id")!.Value
                }),
                _ => throw new ArgumentException("Unexpected argument value encountered")
            };
            path = Path.Combine(DataPath, "PlaceRoute.xml");
            var PlaceRoute = mode switch
            {
                1 => (new XmlSerializer(typeof(List<PlaceRoute>))
                .Deserialize(new StreamReader(path)) as IEnumerable<PlaceRoute>)!
                .Select(pr => new
                {
                    Place = (int)pr.Place.Id,
                    Route = (int)pr.Route.Id
                }) as IEnumerable<dynamic>,
                2 => XDocument.Load(path)
                .Descendants("PlaceRoute")
                .Select(pr => new
                {
                    Route = pr.Element("Route")!.Element("Id")!.Value,
                    Place = pr.Element("Place")!.Element("Id")!.Value
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
        public void SelfQuery_3() {
            var mode = GetDeserializationMode();
            var path = Path.Combine(DataPath, "HotelRating.xml");
            var HotelRating = mode switch
            {
                1 => (new XmlSerializer(typeof(List<HotelRating>))
                    .Deserialize(new StreamReader(path)) as IEnumerable<HotelRating>)!
                    .Select(hr => hr.Rating),
                2 => XDocument
                    .Load(path)
                    .Descendants("HotelRating")
                    .Select(hr => (decimal)hr.Element("Rating")!),
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
            var path = Path.Combine(DataPath, "Route.xml");
            var Route = mode switch
            {
                1 => (new XmlSerializer(typeof(List<Route>))
                    .Deserialize(new StreamReader(path)) as IEnumerable<Route>)!
                    .Select(r => new
                    {
                        r.Duration,
                        r.Id
                    }) as IEnumerable<dynamic>,
                2 => XDocument
                    .Load(path)
                    .Descendants("Route")
                    .Select(r => new
                    {
                        Duration = (int)r.Element("Duration")!,
                        Id = (int)r.Element("Id")!
                    }),
                _ => throw new ArgumentException("Unexpected argument value encountered")
            };
            var sq = from r in Route
                     group r.Id by r.Duration into DurationGroup
                     orderby DurationGroup.Key descending
                     let ids = DurationGroup.Aggregate((curr, next) => curr + " " + next)
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
            var path = Path.Combine(DataPath, "HotelRating.xml");
            var HotelRating = mode switch
            {
                1 => (new XmlSerializer(typeof(List<HotelRating>))
                    .Deserialize(new StreamReader(path))! as IEnumerable<HotelRating>)!
                    .Select(hr => new
                    {
                        HotelCity = hr.Hotel.Location.City,
                        hr.Rating
                    }),
                2 => XDocument
                    .Load(path)
                    .Descendants("HotelRating")
                    .Select(hr => new
                    {
                        HotelCity = hr.Element("Hotel")!.Element("Location")!.Element("City")!.Value,
                        Rating = (decimal)hr.Element("Rating")!
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
            var path = Path.Combine(DataPath, "ClientTour.xml");
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
            path = Path.Combine(DataPath, "Client.xml");
            var Client = XDocument
                        .Load(Path.Combine(DataPath, "Client.xml"))
                        .Descendants("Client");
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
        public void SelfQuery_7()
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
                        hr.Route.Guide.Experience
                    }),
                2 => XDocument
                    .Load(path)
                    .Descendants("HotelRoute")
                    .Select(hr => new
                    {
                        Hotel = (int)hr.Element("Hotel")!.Element("Id")!,
                        Route = (int)hr.Element("Route")!.Element("Id")!,
                        Experience = (int)hr.Element("Route")!.Element("Guide")!.Element("Experience")!
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
