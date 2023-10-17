using EventDictionaryLib;

namespace Dictionary.Tests;

public class ExceptionHelperTests
{
    [Fact]
    public void Throw_ArgumentNullException_If_Item_Null()
    {
        string item = null;

        var exception = Record.Exception(() => ExceptionHelper.CheckNull(item, string.Empty));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void Not_Throw_ArgumentNullException_If_Item_Not_Null()
    {
        string item = "_";
        
        var exception = Record.Exception(() => ExceptionHelper.CheckNull(item, string.Empty));
        
        Assert.Null(exception);
    }

    [Fact]
    public void Throw_KeyNotFoundException_With_Any_Key()
    {
        var key = 1;

        var exception = Record.Exception(() => ExceptionHelper.ThrowNotFoundKey(key));

        Assert.IsType<KeyNotFoundException>(exception);
    }

    [Fact]
    public void Throw_ApplicationException_If_Any_Key_Exists()
    {
        var key = 1;

        var exception = Record.Exception(() => ExceptionHelper.ThrowExistsKey(key));

        Assert.IsType<ApplicationException>(exception);
    }

    [Fact]
    public void Throw_ArgumentOutOfRangeException_If_Index_OutOfRange()
    {
        var max = 5;
        var min = 0;
        var index = 100;

        var exception = Record.Exception(() => ExceptionHelper.CheckRange(index, min, max, string.Empty));

        Assert.IsType<ArgumentOutOfRangeException>(exception);
    }

    [Fact]
    public void Not_Throw_OutOfRangeException_If_Index_Is_Not_OutOfRange()
    {
        var max = 5;
        var min = 0;
        var index = 3;

        var exception = Record.Exception(() => ExceptionHelper.CheckRange(index, min, max, string.Empty));
        
        Assert.Null(exception);
    }
}