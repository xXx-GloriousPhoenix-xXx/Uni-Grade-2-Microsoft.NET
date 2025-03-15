using Lab1;
using System.Text;
using System.Reflection;
using System.Xml.Serialization;
namespace Lab2
{
    public partial class LINQToXMLQuery(string path, LINQToObjectsQuery qgen) : QueryProcessor
    {
        public LINQToObjectsQuery Data { get; set; } = qgen;
        public string DataPath { get; set; } = path;
        public void InstanceToXML()
        {
            var lp = Data.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.PropertyType.IsGenericType && p.PropertyType.GetGenericTypeDefinition() == typeof(List<>));
            foreach (var p in lp)
            {
                var l = p.GetValue(Data) as System.Collections.IEnumerable;
                if (l is null) continue;
                Type et = p.PropertyType.GetGenericArguments()[0];
                var xs = new XmlSerializer(typeof(List<>).MakeGenericType(et));
                using (var sw = new StreamWriter(Path.Combine(DataPath, $"{et.Name}.xml")))
                {
                    xs.Serialize(sw, l);
                }
            }
        }
        
    }
    public static class Test
    {
        public static void Main()
        {
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
            var path = "XML";
            var xmlp = new LINQToXMLQuery(path, qgen);
            xmlp.InstanceToXML();

            xmlp.CallMethod();
        }
    }
}