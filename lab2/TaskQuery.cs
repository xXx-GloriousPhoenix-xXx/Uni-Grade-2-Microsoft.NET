using System.Xml.Linq;
using Lab1.Class;

namespace Lab2 {
    public partial class LINQToXMLQuery 
    {
        public void TaskQuery_1()
        {
            var Client = XDocument.Load(Path.Combine(DataPath, "Client.xml"));
            var Guide = XDocument.Load(Path.Combine(DataPath, "Guide.xml"));
            var Route = XDocument.Load(Path.Combine(DataPath, "Route.xml"));
            var Tour = XDocument.Load(Path.Combine(DataPath, "Tour.xml"));
            var ClientTour = XDocument.Load(Path.Combine(DataPath, "ClientTour.xml"));
            var GuideRating = XDocument.Load(Path.Combine(DataPath, "GuideRating.xml"));

            var tq = from ct in ClientTour.Descendants("ClientTour")
                     join t in Tour.Descendants("Tour")
                          on (int)ct.Element("Tour")! equals (int)t.Element("Id")!
                     join r in Route.Descendants("Route")
                          on (int)t.Element("Route")! equals (int)r.Element("Id")!
                     join g in Guide.Descendants("Guide")
                          on (int)r.Element("Guide")! equals (int)g.Element("Id")!
                     join gr in GuideRating.Descendants("GuideRating")
                          on (int)g.Element("Id")! equals (int)gr.Element("Guide")!
                     group new { ct, gr } by g into GuideGroup
                     let gc = GuideGroup.Select(gg => gg.ct.Element("Client")!.Value).Distinct().Count()
                     let tc = (decimal)Client.Descendants("Client").Count()
                     let rs = Route.Descendants("Route").Where(r => (int)r.Element("Guide")! == (int)GuideGroup.Key.Element("Id")!)
                     let cr = Math.Round(gc / tc, 2)
                     let ar = Math.Round(GuideGroup.Average(gg => (decimal)gg.gr.Element("Rating")!), 2)
                     where ar > 0.9m * 5m && cr < 0.1m
                     select new
                     {
                         Route = string.Join(", ", rs.Select(r => r.Element("Id")!.Value)),
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
        public void TaskQuery_2() {
            var Hotel = XDocument.Load(Path.Combine(DataPath, "Hotel.xml"));
            var HotelRoute = XDocument.Load(Path.Combine(DataPath, "HotelRoute.xml"));
            var Route = XDocument.Load(Path.Combine(DataPath, "Route.xml"));
            var Tour = XDocument.Load(Path.Combine(DataPath, "Tour.xml"));

            var tq = (from h in Hotel.Descendants("Hotel")
                      join hr in HotelRoute.Descendants("HotelRoute")
                           on (int)h.Element("Id")! equals (int)hr.Element("Hotel")!
                      join r in Route.Descendants("Route")
                           on (int)hr.Element("Route")! equals (int)r.Element("Id")!
                      join t in Tour.Descendants("Tour")
                           on (int)r.Element("Id")! equals (int)t.Element("Route")!
                      group t by h.Element("Id") into HotelVisit
                      let v = HotelVisit.Count()
                      orderby v descending, (int)HotelVisit.Key ascending
                      select new
                      {
                          Hotel = HotelVisit.Key.Value,
                          TotalVisit = v
                      }).First();
            ShowQuery("Task Query 2", [tq], new Dictionary<string, string> //
            {
                { "Hotel", "Готель"},
                { "TotalVisit", "Кількість груп"}
            });
        }
        public void TaskQuery_3() {
            var Guide = XDocument.Load(Path.Combine(DataPath, "Guide.xml"));
            var Route = XDocument.Load(Path.Combine(DataPath, "Route.xml"));
            var Tour = XDocument.Load(Path.Combine(DataPath, "Tour.xml"));
            var GuideRating = XDocument.Load(Path.Combine(DataPath, "GuideRating.xml"));

            var tq = from t in Tour.Descendants("Tour")
                     join r in Route.Descendants("Route")
                          on (int)t.Element("Route")! equals (int)r.Element("Id")!
                     join g in Guide.Descendants("Guide")
                          on (int)r.Element("Guide")! equals (int)g.Element("Id")!
                     join gr in GuideRating.Descendants("GuideRating")
                          on (int)g.Element("Id")! equals (int)gr.Element("Guide")!
                     group new { t, gr } by g into GuideGroup
                     let gt = GuideGroup.Select(gg => gg.t.Element("Id")!.Value).Distinct().Count()
                     let ar = Math.Round(GuideGroup.Average(gg => (decimal)gg.gr.Element("Rating")!), 2)
                     where gt > 10 && ar > 4.5m
                     orderby (int)GuideGroup.Key.Element("Id")! ascending, gt descending, ar descending
                     select new
                     {
                         Guide = GuideGroup.Key.Element("Id")!.Value,
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
            var HotelRating = XDocument.Load(Path.Combine(DataPath, "HotelRating.xml"));
            var Hotel = XDocument.Load(Path.Combine(DataPath, "Hotel.xml"));
            var HotelRoute = XDocument.Load(Path.Combine(DataPath, "HotelRoute.xml"));
            var Route = XDocument.Load(Path.Combine(DataPath, "Route.xml"));
            var Tour = XDocument.Load(Path.Combine(DataPath, "Tour.xml"));
            var ClientTour = XDocument.Load(Path.Combine(DataPath, "ClientTour.xml"));

            var tq = (from hra in HotelRating.Descendants("HotelRating")
                      join h in Hotel.Descendants("Hotel")
                           on (int)hra.Element("Hotel")! equals (int)h.Element("Id")!
                      join hro in HotelRoute.Descendants("HotelRoute")
                           on (int)h.Element("Id")! equals (int)hro.Element("Hotel")!
                      join r in Route.Descendants("Route")
                           on (int)hro.Element("Route")! equals (int)r.Element("Id")!
                      join t in Tour.Descendants("Tour")
                           on (int)r.Element("Id")! equals (int)t.Element("Route")!
                      join ct in ClientTour.Descendants("ClientTour")
                           on (int)t.Element("Id")! equals (int)ct.Element("Tour")!
                      group new { hra, ct } by (int)hro.Element("Route")! into HotelRatingGroup
                      let ar = HotelRatingGroup.Average(hrg => (decimal)hrg.hra.Element("Rating")!)
                      let rc = HotelRatingGroup.Select(hrg => (int)hrg.ct.Element("Client")!).Distinct().Count()
                      let rd = Route.Descendants("Route")
                                    .Where(r => (int)r.Element("Id")! == HotelRatingGroup.Key)
                                    .Select(r => (int)r.Element("Duration")!).Single()
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
