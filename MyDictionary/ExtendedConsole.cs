namespace MyDictionary;

public static class ExtendedConsole
{
    public static void WriteLine(string line, ConsoleColor backgroundColor = ConsoleColor.Black,
        ConsoleColor foregroundColor = ConsoleColor.White)
    {
        Console.BackgroundColor = backgroundColor;
        Console.ForegroundColor = foregroundColor;

        Console.WriteLine(line);
        
        Console.ResetColor();
    }
}