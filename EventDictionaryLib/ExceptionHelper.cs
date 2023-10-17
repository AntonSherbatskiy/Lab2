namespace EventDictionaryLib;

public static class ExceptionHelper
{
    public static void CheckNull<T>(T? item, string message)
    {
        if (item == null)
        {
            throw new ArgumentNullException(message);
        }
    }
    
    public static void CheckRange(int index, int min, int max, string message)
    {
        if (index < min || index > max)
        {
            throw new ArgumentOutOfRangeException(message);
        }
    }

    public static void ThrowNotFoundKey<T>(T key)
    {
        throw new KeyNotFoundException($"Key: {key} does not exists");
    }

    public static void ThrowExistsKey<T>(T key)
    {
        throw new ApplicationException($"Key: {key} is already exists");
    }
}