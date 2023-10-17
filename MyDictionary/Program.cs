using System.Diagnostics;
using EventDictionaryLib;

namespace MyDictionary;

public class Program
{
    static void Main()
    {
        var randomizer = new Random();
        var dict = new EventDictionary<string, int>();
        dict.Add("1", 1);
        
        var itemsCount = 25;
        
        dict.OnAdd += item => 
            ExtendedConsole.WriteLine($"Added item: {item.Value} with key: {item.Key}. Current elements count: {dict.Count}",
                foregroundColor: ConsoleColor.Green);
        
        dict.OnRemove += key =>
            ExtendedConsole.WriteLine($"Item with key: {key} removed. Current elements count: {dict.Count}",
                foregroundColor: ConsoleColor.Red);
        
        dict.OnUpdate += (key, oldValue, newValue) =>
            ExtendedConsole.WriteLine($"Element with key: {key}, old value: {oldValue} updated to {newValue}",
                foregroundColor: ConsoleColor.Yellow);
        
        Console.WriteLine(new string('-', 30) + "ADD TEST" + new string('-', 30));
        TestAdd(dict, itemsCount);
        
        Console.WriteLine("\n" + new string('-', 30) + "REMOVE TEST" + new string('-', 30));
        TestRemove(itemsCount, dict);
        
        Console.WriteLine("\n" + new string('-', 30) + "UPDATE TEST" + new string('-', 30));
        TestUpdate(itemsCount, dict);
        
        Console.WriteLine("\n" + new string('-', 30) + "GET TEST" + new string('-', 30));
        TestGet(itemsCount, dict);
        
        Console.WriteLine("\n" + new string('-', 30) + "Dictionary" + new string('-', 30));
        Print(dict);
        
        Console.ReadLine();
    }

    private static void Print<TKey, TValue>(IDictionary<TKey, TValue> dictionary)
    {
        foreach (var item in dictionary)
        {
            Console.WriteLine($"Item key: {item.Key}, value: {item.Value}");
        }
    }

    private static void TestGet(int itemsCount, EventDictionary<string, int> dict)
    {
        var randomizer = new Random();
        
        for (int i = 0; i < 10; i++)
        {
            var randomKey = randomizer.Next(itemsCount + 10).ToString();
            int? value = null;

            try
            {
                value = dict[randomKey];
            }
            catch (KeyNotFoundException e)
            {
                ExtendedConsole.WriteLine(e.Message, backgroundColor: ConsoleColor.Red);
            }
            
            if (value != null)
            {
                ExtendedConsole.WriteLine($"Get value: {value} with key: {randomKey}",
                    foregroundColor: ConsoleColor.Green);
            }
            
        }
    }

    private static void TestUpdate(int itemsCount, EventDictionary<string, int> dict)
    {
        var randomizer = new Random();
        
        for (int i = 0; i < 10; i++)
        {
            var randomKey = randomizer.Next(itemsCount + 10).ToString();

            try
            {
                dict.Update(randomKey, 999);
            }
            catch (KeyNotFoundException e)
            {
                ExtendedConsole.WriteLine(e.Message, backgroundColor: ConsoleColor.Red);
            }
        }
    }

    private static void TestRemove(int itemsCount, EventDictionary<string, int> dict)
    {
        var randomizer = new Random();
        
        for (int i = 0; i < 10; i++)
        {
            var randomKey = randomizer.Next(itemsCount + 10).ToString();
            var removed = dict.Remove(randomKey);

            if (!removed)
            {
                ExtendedConsole.WriteLine($"Dictionary does not contain key: {randomKey}",
                    backgroundColor: ConsoleColor.Red);
            }
        }
    }

    private static void TestAdd(EventDictionary<string, int> dict, int itemsCount)
    {
        try
        {
            Fill(dict, itemsCount);

            dict.Add("2", 222);
        }
        catch (ApplicationException e)
        {
            ExtendedConsole.WriteLine(e.Message, backgroundColor: ConsoleColor.Red);
        }
    }

    static void Fill(EventDictionary<string, int> dictionary, int itemsCount)
    {
        for (int i = 0; i < itemsCount; i++)
        {
            var item = i + 1;
            dictionary[(i + 1).ToString()] = item;
        }
    }
}