using Lab1;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Collections;
using Lab1.Class;
using System.Text.Json.Nodes;
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

            if (string.IsNullOrEmpty(result)) 
            {
                return GetDeserializationMode();
            }

            result = result.ToLower();
            if (result == "des") return 1;
            if (result == "jdoc") return 2;
            if (result == "jnode") return 3;
            return GetDeserializationMode();
        }
    }
    public class Handler {
        public LINQToJSONQuery JSONP { get; set; }
        public string DataPath { get; set; }
        public Handler() {
            var jsonp = GenerateData();
            JSONP = jsonp;
            DataPath = jsonp.DataPath;
        }
        public static LINQToJSONQuery GenerateData() {
            var client = 4800;
            var guide = 12;
            var route = 50;
            var hotel_weight = new decimal[] { 1, 2, 3, 3, 4, 4, 4, 4, 5, 5, 5, 5, 5, 5, 5, 5, 5 };
            var guide_weight = new decimal[] { 1, 2, 3, 3, 4, 4, 4, 4, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5 };

            var cfg = new Config(client, guide, route, hotel_weight, guide_weight);
            var qgen = new LINQToObjectsQuery(cfg);
            var path = "C:\\Users\\Admin\\Desktop\\JSON";
            //var path = "C:\\Users\\Yuri\\OneDrive\\Desktop\\JSON";
            var jsonp = new LINQToJSONQuery(path, qgen);
            jsonp.InstanceToJSON();

            return jsonp;
        }
        public void RegenerateData() => JSONP = GenerateData();
        public static int? GetOption() {
            Console.OutputEncoding = Encoding.UTF8;
            Console.WriteLine("Оберіть операцію:");
            Console.WriteLine("1 - Створити файл з консолі");
            Console.WriteLine("2 - Прочитати створений файл");
            Console.WriteLine("3 - Виконати запит LINQ to JSON");
            Console.WriteLine("4 - Перегенерувати файли з даними");
            Console.WriteLine("0 - Вийти з програми");
            var input = Console.ReadLine();
            _ = int.TryParse(input, out int option);
            return option;
        }
        public static Person GetPerson() {
            Console.OutputEncoding = Encoding.UTF8;
            int id;
            while (true)
            {
                Console.WriteLine("Введідь Id:");
                var l = Console.ReadLine();
                if (int.TryParse(l, out int val))
                {
                    id = val;
                    break;
                }
            }
            Console.WriteLine("Введість Surname:");
            var s = Console.ReadLine();
            Console.WriteLine("Введість Name:");
            var n = Console.ReadLine();
            Console.Clear();
            return new Person(id, s!, n!);
        }
        public void CreateFile() {
            Console.OutputEncoding = Encoding.UTF8;
            int q;
            while (true)
            {
                Console.WriteLine("Введіть кількість записів в файлі");
                var l = Console.ReadLine();
                Console.Clear();
                if (int.TryParse(l, out int val))
                {
                    q = val;
                    break;
                }
            }
            var p = Path.Combine(DataPath, "Test.json");
            var dl = new List<Person>();
            for (var i = 0; i < q; i++)
            {
                Console.WriteLine($"#{i + 1}");
                var d = GetPerson();
                dl.Add(d);
            }
            var s = new JsonSerializerOptions { WriteIndented = false };
            string json = JsonSerializer.Serialize(dl, s);
            File.WriteAllText(p, json);
        }
        public void ReadFile() {
            var mode = LINQToJSONQuery.GetDeserializationMode();
            var p = Path.Combine(JSONP.DataPath, "Test.json");
            using var sr = new StreamReader(p);

            var data = mode switch
            {
                1 => JsonSerializer.Deserialize<IEnumerable<Person>>(sr.ReadToEnd())!
                    .Select(p => new
                    {
                        p.Id,
                        p.Surname,
                        p.Name
                    }),
                2 => JsonDocument.Parse(sr.ReadToEnd()).RootElement.EnumerateArray()
                    .Select(p => new
                    {
                        Id = p.GetProperty("Id").GetInt32()!,
                        Surname = p.GetProperty("Surname").GetString()!,
                        Name = p.GetProperty("Name").GetString()!
                    }),
                3 => JsonNode.Parse(sr.ReadToEnd())!.AsArray()
                    .Select(p => new
                    {
                        Id = (int)p!["Id"]!,
                        Surname = (string)p!["Surname"]!,
                        Name = (string)p!["Name"]!
                    }),
                _ => throw new ArgumentException("Unexpected argument value encountered")
            };

            foreach (var el in data)
            {
                Console.WriteLine($"Id:\t\t{el.Id}");
                Console.WriteLine($"Surname:\t{el.Surname}");
                Console.WriteLine($"Name:\t\t{el.Name}");
                Console.WriteLine();
            }
        }
        public void HandleOption() {
            int? option;
            do
            {
                option = GetOption();
                Console.Clear();
                if (option is null) continue;
                switch (option)
                {
                    case 1:
                        CreateFile();
                        break;
                    case 2:
                        ReadFile();
                        break;
                    case 3:
                        JSONP.CallMethod();
                        break;
                    case 4:
                        GenerateData();
                        break;
                    default:
                        break;
                }
            } while (option != 0);
        }
    }

    public static class Test {
        public static void Main() {
            var h = new Handler();
            h.HandleOption();
        }
    }
}