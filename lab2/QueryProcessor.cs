using System.Reflection;

namespace Lab1
{
    public abstract class QueryProcessor {
        public static void ShowQuery<T>(string title, IEnumerable<T> query, Dictionary<string, string> field_name) {
            Console.WriteLine($"———————{title}:———————");
            var last_item = query.LastOrDefault();
            if (last_item is null)
            {
                Console.WriteLine("Результатів не знайдено");
            }
            else
            {
                foreach (var item in query)
                {
                    foreach (var prop in item!.GetType().GetProperties())
                    {
                        string name = field_name[prop.Name];
                        Console.WriteLine($"{name}: {prop.GetValue(item)}");
                    }
                    if (!item.Equals(last_item))
                    {
                        Console.WriteLine();
                    }
                }
            }
            Console.WriteLine("———————————————————————————\n");
        }
        public void CallMethod() {
            Console.WriteLine("Input type of method (Self/Task):");
            var method_type = Console.ReadLine();
            Console.Clear();

            var method_dictionary = new Dictionary<string, List<int>>()
            {
                { "Self", new List<int>() { 1, 7} },
                { "Task", new List<int>() { 1, 4} }
            };

            if (method_type is null || !method_dictionary.TryGetValue(method_type!, out List<int>? dictionary_value))
            {
                CallMethod();
                return;
            }

            Console.WriteLine("Enter the number of method ({0})", string.Join("-", dictionary_value));
            var method_number = Console.ReadLine();
            Console.Clear();

            if (string.IsNullOrWhiteSpace(method_number) || !int.TryParse(method_number, out int value) || value < dictionary_value.Min() || value > dictionary_value.Max())
            {
                CallMethod();
                return;
            }

            var method_name = $"{method_type}Query_{value}";
            var method = GetType().GetMethod(method_name);
            if (method is null)
            {
                CallMethod();
                return;
            }
            method.Invoke(this, null);
        }
    }
}
