using System.Diagnostics;

namespace Lab1
{
    public abstract class QueryProcessor {
        public static string ElapsedTimeProcessor(Stopwatch w) {
            var t = w.Elapsed;
            if (t.TotalNanoseconds < 2000) return $"{Math.Round(t.TotalNanoseconds)} нс";
            if (t.TotalMicroseconds < 2000) return $"{Math.Round(t.TotalMicroseconds)} мкс";
            if (t.TotalMilliseconds < 2000) return $"{Math.Round(t.TotalMilliseconds)} мс";
            else return $"{Math.Round(t.TotalSeconds, 3)} с";
        }
        public static void ShowQuery<T>(string title, IEnumerable<T> query, Dictionary<string, string> field_name) {
            var w = new Stopwatch();
            w.Start();

            Console.WriteLine($"———————{title}:———————");
            var list = query.ToList();
            if (list.Count == 0)
            {
                Console.WriteLine("Результатів не знайдено");
            }
            else
            {
                foreach (var item in list)
                {
                    foreach (var prop in item!.GetType().GetProperties())
                    {
                        string name = field_name[prop.Name];
                        Console.WriteLine($"{name}: {prop.GetValue(item)}");
                    }
                    if (!item.Equals(list.Last()))
                    {
                        Console.WriteLine();
                    }
                }
            }
            Console.WriteLine("———————————————————————————");

            w.Stop();

            Console.WriteLine("Часу витрачено: {0}", ElapsedTimeProcessor(w));
            Console.WriteLine("———————————————————————————\n");
        }
        public bool CallMethod() {
            Console.WriteLine("Введіть тип черги (Self/Task) або Quit");
            var method_type = Console.ReadLine();
            Console.Clear();

            var method_dictionary = new Dictionary<string, List<int>>()
            {
                { "Self", new List<int>() { 1, 7} },
                { "Task", new List<int>() { 1, 4} }
            };

            if (method_type is null || !method_dictionary.TryGetValue(method_type!, out List<int>? dictionary_value))
            {
                if (method_type is not null && method_type == "Quit")
                {
                    return true;
                }
                CallMethod();
                return false;
            }

            Console.WriteLine("Введіть номер черги ({0})", string.Join("-", dictionary_value));
            var method_number = Console.ReadLine();
            Console.Clear();

            if (string.IsNullOrWhiteSpace(method_number) || !int.TryParse(method_number, out int value) || value < dictionary_value.Min() || value > dictionary_value.Max())
            {
                CallMethod();
                return false;
            }

            var method_name = $"{method_type}Query_{value}";
            var method = GetType().GetMethod(method_name);
            if (method is null)
            {
                CallMethod();
                return false;
            }
            method.Invoke(this, null);
            return false;
        }
    }
}
