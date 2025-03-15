namespace Lab1 {
    public partial class LINQToObjectsQuery
    {
        public void TaskQuery_1() {
            var tq = from g in Guide
                     join r in Route on g equals r.Guide
                     join t in Tour on r equals t.Route
                     join ct in ClientTour on t equals ct.Tour
                     join gr in GuideRating on g equals gr.Guide
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
        public void TaskQuery_2() {
            var tq = (from hr in HotelRoute
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
        public void TaskQuery_3() {
            var tq = from g in Guide
                     join t in Tour on g equals t.Route.Guide
                     join gr in GuideRating on g equals gr.Guide
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
            var tq = (from hro in HotelRoute
                      join hra in HotelRating on hro.Hotel equals hra.Hotel
                      join ct in ClientTour on hro.Route equals ct.Tour.Route
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
}
