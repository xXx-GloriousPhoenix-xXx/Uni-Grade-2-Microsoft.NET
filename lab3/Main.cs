using Lab1;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Collections;
namespace Lab3
{
    public partial class LINQToJSONQuery(string path, LINQToObjectsQuery qgen) : QueryProcessor
    {
        public LINQToObjectsQuery Data { get; set; } = qgen;
        public string DataPath { get; set; } = path;
        public void InstanceToJSON() {
            var lp = Data.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.PropertyType.IsGenericType && p.PropertyType.GetGenericTypeDefinition() == typeof(List<>));
            var s = new JsonSerializerOptions { WriteIndented = false };
            foreach (var p in lp)
            {
                var l = p.GetValue(Data) as IEnumerable;
                if (l is null) continue;
                Type et = p.PropertyType.GetGenericArguments()[0];
                string json = JsonSerializer.Serialize(l, s);
                var path = Path.Combine(DataPath, $"{et.Name}.json");
                File.WriteAllText(path, json);
            }
        }
        public static int GetDeserializationMode() {
            Console.Clear();
            Console.WriteLine("Введіть спосіб десеріалізації (Des/JDoc/JNode)");
            var result = Console.ReadLine();
            Console.Clear();

            if (string.IsNullOrEmpty(result)) return GetDeserializationMode();
            if (result == "Des") return 1;
            if (result == "JDoc") return 2;
            if (result == "JNode") return 3;
            return GetDeserializationMode();
        }
    }
    public static class Test {
        public static void Main() {
            #region Settings
            Console.OutputEncoding = Encoding.UTF8;
            var client = 4800;
            var guide = 12;
            var route = 50;
            var hotel_weight = new decimal[] { 1, 2, 3, 3, 4, 4, 4, 4, 5, 5, 5, 5, 5, 5, 5, 5, 5 };
            var guide_weight = new decimal[] { 1, 2, 3, 3, 4, 4, 4, 4, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5 };
            #endregion

            var cfg = new Config(client, guide, route, hotel_weight, guide_weight);
            var qgen = new LINQToObjectsQuery(cfg);
            var path = "JSON";
            var jsonp = new LINQToJSONQuery(path, qgen);
            jsonp.InstanceToJSON();

            while (true)
            {
                var result = jsonp.CallMethod();
                if (result) break;
            }
        }
    }
}