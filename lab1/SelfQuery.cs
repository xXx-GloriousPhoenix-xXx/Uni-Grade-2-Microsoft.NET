namespace Lab1 {
    public partial class LINQToObjectsQuery 
    {
        /// <summary>
        /// Most experienced guides
        /// </summary>
        public void SelfQuery_1() {
            var sq = (from r in Route
                      join ct in ClientTour on r equals ct.Tour.Route
                      group ct by r.Guide into GuideClientGroup
                      let gc = GuideClientGroup.Count()
                      orderby GuideClientGroup.Key.Experience descending, gc descending
                      select new
                      {
                          Guide = GuideClientGroup.Key.Id,
                          AgeExperience = GuideClientGroup.Key.Experience,
                          TotalClient = gc
                      }).Take(3);
            ShowQuery("Self Query 1", sq, new Dictionary<string, string>
            {
                { "Guide", "Гід" },
                { "AgeExperience", "Стаж" },
                { "TotalClient", "Кількість клієнтів" }
            });
        }
        /// <summary>
        /// Most popular visited place
        /// </summary>
        public void SelfQuery_2() {
            var sq = (from ct in ClientTour
                      join t in Tour on ct.Tour equals t
                      join r in Route on t.Route equals r
                      join pr in PlaceRoute on r equals pr.Route
                      join p in Place on pr.Place equals p
                      group ct by p into PlaceClientGroup
                      let pc = PlaceClientGroup.Count()
                      orderby pc descending, PlaceClientGroup.Key.Id ascending
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
        public void SelfQuery_3() {
            var sq = from hr in HotelRating
                     group hr by hr.Rating into RatingGroup
                     let tr = (decimal)(HotelRating.Count)
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
        public void SelfQuery_4() {
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
        /// Best cities by average hotel rating
        /// </summary>
        public void SelfQuery_5() {
            var sq = (from l in Location
                      join hr in HotelRating on l equals hr.Hotel.Location
                      group hr by l.City into CityHotelGroup
                      let ar = CityHotelGroup.Average(chg => chg.Rating)
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
        /// <summary>
        /// Most popular route among last 100 clients
        /// </summary>
        public void SelfQuery_6() {
            var sq = (from ct in ClientTour
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
            ShowQuery("Self Query 6", sq, new Dictionary<string, string>
            {
                { "Route", "Маршрут" },
                { "Total", "Кількість турів" }
            });
        }
        /// <summary>
        /// Least popular hotel among guides with less than 1 year experience
        /// </summary>
        public void SelfQuery_7() {
            var sq = (from hr in HotelRoute
                      join t in Tour.Where(t => t.Route.Guide.Experience < 2) on hr.Route equals t.Route
                      group t by hr.Hotel into HotelVisitGroup
                      let hv = HotelVisitGroup.Count()
                      orderby hv descending, HotelVisitGroup.Key.Id ascending
                      select new
                      {
                          Hotel = HotelVisitGroup.Key.Id,
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
