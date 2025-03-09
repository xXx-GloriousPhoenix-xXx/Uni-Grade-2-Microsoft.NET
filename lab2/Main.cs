using Lab1;
using System.Text;
using System.Xml;
using System.Reflection;
namespace Lab2
{
    public partial class LINQToXMLQuery(string path, LINQToObjectsQuery qgen) : QueryProcessor
    {
        public LINQToObjectsQuery Data { get; set; } = qgen;
        public string DataPath { get; set; } = path;
        public static List<string> GetPropertiesNames() => typeof(LINQToObjectsQuery)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.PropertyType.IsGenericType && p.PropertyType.GetGenericTypeDefinition() == typeof(List<>))
                .Select(p => p.Name)
   
             .ToList();
        public void InstanceToXML()
        {
            var settings = new XmlWriterSettings
            {
                Indent = true
            };
            var classes = GetPropertiesNames();
            foreach (var c in classes)
            {
                var property = typeof(LINQToObjectsQuery).GetProperty(c) ?? throw new MissingMemberException("Цього проперті не знайшлось!");
                var instance = property.GetValue(qgen) as System.Collections.IEnumerable ?? throw new ArgumentNullException("Інтансів цього проперті не знайшлось!");
                using (var xw = XmlWriter.Create(Path.Combine(DataPath, $"{c}.xml"), settings))
                {
                    xw.WriteStartDocument();
                    xw.WriteStartElement(c + "s");

                    foreach (var i in instance)
                    {
                        xw.WriteStartElement(c);
                        foreach (var p in i.GetType().GetProperties())
                        {
                            var value = p.GetValue(i) ?? throw new MissingMemberException("Проперті елемента не знайшлось!");
                            if (p.PropertyType == typeof(string) || p.PropertyType == typeof(decimal) || p.PropertyType.IsPrimitive)
                            {
                                xw.WriteElementString(p.Name, value.ToString());
                            }
                            else
                            {
                                var id = p.PropertyType.GetProperty("Id") ?? throw new MissingMemberException("Проперті Id не знайшлося!");
                                var id_val = id.GetValue(value) ?? throw new ArgumentNullException("Значення проперті Id null!");
                                xw.WriteElementString(p.Name, id_val.ToString());
                            }
                        }
                        xw.WriteEndElement();
                    }

                    xw.WriteEndElement();
                    xw.WriteEndDocument();
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

            while (true)
            {
                var cfg = new Config(client, guide, route, hotel_weight, guide_weight);
                var qgen = new LINQToObjectsQuery(cfg);
                var path = "XML";
                var xmlp = new LINQToXMLQuery(path, qgen);
                xmlp.InstanceToXML();

                xmlp.CallMethod();
            }  
        }
    }
}