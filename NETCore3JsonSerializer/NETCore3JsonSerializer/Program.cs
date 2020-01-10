using System;
using System.Collections.Generic;
using System.Text.Json;

namespace NETCore3JsonSerializer
{
    class Program
    {
        static void Main(string[] args)
        {
            MyModel myModel = new MyModel();
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };

            var modelJson = JsonSerializer.Serialize(myModel, options);
            Console.WriteLine(modelJson);
        }
    }

    public class MyModel
    {
        public string MyStRIng { get; set; }
        public int MyInt { get; set; }
        public bool MyBoolean { get; set; }
        public decimal MyDecimal { get; set; }
        public DateTime MyDateTime1 { get; set; }
        public DateTime MyDateTime2 { get; set; }
        public List<string> MyStringList { get; set; }
        public Dictionary<string, Person> MyDictionary { get; set; }
        public MyModel MyAnotherModel { get; set; }
    }

    public class Person
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
