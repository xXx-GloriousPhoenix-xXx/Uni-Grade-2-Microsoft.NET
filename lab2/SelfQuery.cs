using Lab1.Class;
using System.Xml.Linq;

namespace Lab2
{
    public partial class LINQToXMLQuery 
    {
        public void SelfQuery_1() {
            var ClientTour = XDocument.Load(Path.Combine(DataPath, "ClientTour.xml"));
            var Tour = XDocument.Load(Path.Combine(DataPath, "Tour.xml"));
            var Route = XDocument.Load(Path.Combine(DataPath, "Route.xml"));
            var Guide = XDocument.Load(Path.Combine(DataPath, "Guide.xml"));

            var sq = (from ct in ClientTour.Descendants("ClientTour")
                      join t in Tour.Descendants("Tour")
                          on (int)ct.Element("Tour")! equals (int)t.Element("Id")!
                      join r in Route.Descendants("Route")
                          on (int)t.Element("Route")! equals (int)r.Element("Id")!
                      join g in Guide.Descendants("Guide")
                          on (int)r.Element("Guide")! equals (int)g.Element("Id")!
                      group ct by g into GuideClientGroup
                      let gc = GuideClientGroup.Count()
                      orderby (int)GuideClientGroup.Key.Element("Experience")! descending, gc descending
                      select new
                      {
                          Guide = GuideClientGroup.Key.Element("Id")!.Value,
                          AgeExperience = GuideClientGroup.Key.Element("Experience")!.Value,
                          ClientExperience = gc
                      }).Take(3);
            ShowQuery("Self Query 1", sq, new Dictionary<string, string>
            {
                { "Guide", "Гід" },
                { "AgeExperience", "Стаж" },
                { "ClientExperience", "Кількість клієнтів" }
            });
        }
        public void SelfQuery_2() {
            var ClientTour = XDocument.Load(Path.Combine(DataPath, "ClientTour.xml"));
            var Tour = XDocument.Load(Path.Combine(DataPath, "Tour.xml"));
            var Route = XDocument.Load(Path.Combine(DataPath, "Route.xml"));
            var PlaceRoute = XDocument.Load(Path.Combine(DataPath, "PlaceRoute.xml"));
            var Place = XDocument.Load(Path.Combine(DataPath, "Place.xml"));

            var sq = (from ct in ClientTour.Descendants("ClientTour")
                      join t in Tour.Descendants("Tour")
                           on (int)ct.Element("Tour")! equals (int)t.Element("Id")!
                      join r in Route.Descendants("Route")
                           on (int)t.Element("Route")! equals (int)r.Element("Id")!
                      join pr in PlaceRoute.Descendants("PlaceRoute")
                           on (int)r.Element("Id")! equals (int)pr.Element("Route")!
                      join p in Place.Descendants("Place")
                           on (int)pr.Element("Place")! equals (int)p.Element("Id")!
                      group ct by p into PlaceClientGroup
                      let pc = PlaceClientGroup.Count()
                      orderby pc descending, PlaceClientGroup.Key.Value ascending
                      select new
                      {
                          Place = PlaceClientGroup.Key.Element("Id")!.Value,
                          TotalClient = pc
                      }).Take(5);
            ShowQuery("Self Query 2", sq, new Dictionary<string, string>
            {
                { "Place", "Місце" },
                { "TotalClient", "Кількість відвідувань" }
            });
        }
        public void SelfQuery_3() {
            var HotelRating = XDocument.Load(Path.Combine(DataPath, "HotelRating.xml"));

            var sq = from hr in HotelRating.Descendants("HotelRating")
                     group hr by hr.Element("Rating")!.Value into RatingGroup
                     let tr = (decimal)HotelRating.Descendants("HotelRating").Count()
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
        public void SelfQuery_4() {
            var Route = XDocument.Load(Path.Combine(DataPath, "Route.xml"));

            var sq = from r in Route.Descendants("Route")
                     group r by r.Element("Duration")!.Value into DurationGroup
                     orderby int.Parse(DurationGroup.Key) descending
                     let ids = DurationGroup.Select(dg => dg.Element("Id")!.Value.ToString()).Aggregate((curr, next) => curr + " " + next)
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
        public void SelfQuery_5() {
            var Location = XDocument.Load(Path.Combine(DataPath, "Location.xml"));
            var Hotel = XDocument.Load(Path.Combine(DataPath, "Hotel.xml"));
            var HotelRating = XDocument.Load(Path.Combine(DataPath, "HotelRating.xml"));

            var sq = (from l in Location.Descendants("Location")
                      join h in Hotel.Descendants("Hotel")
                           on (int)l.Element("Id")! equals (int)h.Element("Location")!
                      join hr in HotelRating.Descendants("HotelRating")
                           on (int)h.Element("Id")! equals (int)hr.Element("Hotel")!
                      group hr by l.Element("City")!.Value into CityHotelGroup
                      let ar = CityHotelGroup.Average(chg => decimal.Parse(chg.Element("Rating")!.Value))
                      orderby ar descending, CityHotelGroup.Key ascending
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
        public void SelfQuery_6() {
            var ClientTour = XDocument.Load(Path.Combine(DataPath, "ClientTour.xml"));
            var Client = XDocument.Load(Path.Combine(DataPath, "Client.xml"));
            var Tour = XDocument.Load(Path.Combine(DataPath, "Tour.xml"));
            var Route = XDocument.Load(Path.Combine(DataPath, "Route.xml"));

            var sq = (from ct in ClientTour.Descendants("ClientTour")
                      join c in Client.Descendants("Client")
                           on (int)ct.Element("Client")! equals (int)c.Element("Id")!
                      join t in Tour.Descendants("Tour")
                           on (int)ct.Element("Tour")! equals (int)t.Element("Id")!
                      join r in Route.Descendants("Route")
                           on (int)t.Element("Route")! equals (int)r.Element("Id")!
                      let cq = Client.Descendants("Client").Count()
                      let tc = (int)ct.Element("Client")!
                      where cq - 100 < tc && tc <= cq
                      group ct by r into ClientRouteGroup
                      let rc = ClientRouteGroup.Count()
                      orderby rc descending, (int)ClientRouteGroup.Key.Element("Id")! ascending
                      select new
                      {
                          Route = ClientRouteGroup.Key.Element("Id")!.Value,
                          Total = rc
                      }).Take(3);
            ShowQuery("Self Query 6", sq, new Dictionary<string, string>
            {
                { "Route", "Маршрут" },
                { "Total", "Кількість турів" }
            });
        }
        public void SelfQuery_7() {
            var HotelRoute = XDocument.Load(Path.Combine(DataPath, "HotelRoute.xml"));
            var Route = XDocument.Load(Path.Combine(DataPath, "Route.xml"));
            var Tour = XDocument.Load(Path.Combine(DataPath, "Tour.xml"));
            var Guide = XDocument.Load(Path.Combine(DataPath, "Guide.xml"));

            var sq = (from hr in HotelRoute.Descendants("HotelRoute")
                      join r in Route.Descendants("Route")
                           on (int)hr.Element("Route")! equals (int)r.Element("Id")!
                      join g in Guide.Descendants("Guide")
                           on (int)r.Element("Guide")! equals (int)g.Element("Id")!
                      join t in Tour.Descendants("Tour")
                           on (int)r.Element("Id")! equals (int)t.Element("Route")!
                      where (int)g.Element("Experience")! < 2
                      group t by hr.Element("Hotel")!.Value into HotelVisitGroup
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
