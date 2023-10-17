using EventDictionaryLib;
using FluentAssertions;

namespace Dictionary.Tests;

public class EventDictionaryTests
{
    [Fact]
    public void EventDictionary_Must_Not_Be_ReadOnly()
    {
        var dict = new EventDictionary<string, int>();
        var isReadOnly = dict.IsReadOnly;

        Assert.False(isReadOnly);
    }

    [Fact]
    public void EventDictionary_InitialCount_Equals_0()
    {
        var dict = new EventDictionary<string, int>();
        var actual = dict.Count;
        var expected = 0;

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Add_Item_With_NotExisting_Key_Successfully_Adds()
    {
        var dict = new EventDictionary<string, int>();
        var item = new KeyValuePair<string, int>("1", 1);

        dict.Add(item);

        Assert.Contains(item, dict);
    }

    [Fact]
    public void Added_Item_Value_Is_Equals_Self()
    {
        var dict = new EventDictionary<string, int>();
        var item = new KeyValuePair<string, int>("1", 1);

        dict.Add(item);

        Assert.Equal(item.Value, dict[item.Key]);
    }

    [Fact]
    public void Add_Existing_Key_Throws_ApplicationException()
    {
        var dict = new EventDictionary<string, int>();
        var item1 = new KeyValuePair<string, int>("1", 1);
        var item2 = new KeyValuePair<string, int>("1", 2);

        var exception = Record.Exception(() =>
        {
            dict.Add(item1);
            dict.Add(item2);
        });

        Assert.IsType<ApplicationException>(exception);
    }

    [Fact]
    public void Add_Many_Items_Must_Resize_Capacity()
    {
        var dict = new EventDictionary<string, int>();
        var itemsCount = 25;

        for (int i = 0; i < itemsCount; i++)
        {
            dict.Add(new KeyValuePair<string, int>(i.ToString(), i));
        }
        
        Assert.True(dict.Capacity > itemsCount);
    }

    [Fact]
    public void Extracted_Keys_Are_Equal_To_Dictionary_Keys()
    {
        var dict = new EventDictionary<string, int>();

        var items = new List<KeyValuePair<string, int>>()
        {
            new("1", 1),
            new("2", 2),
            new("3", 3)
        };

        var keys = new List<string>()
        {
            items[0].Key,
            items[1].Key,
            items[2].Key
        };
        
        dict.Add(items[0]);
        dict.Add(items[1]);
        dict.Add(items[2]);
        
        dict.Keys.Should().BeEquivalentTo(keys);
    }

    [Fact]
    public void Extracted_Values_Are_Equal_To_Dictionary_Values()
    {
        var dict = new EventDictionary<string, int>();
        
        var items = new List<KeyValuePair<string, int>>()
        {
            new("1", 1),
            new("2", 2),
            new("3", 3)
        };

        var values = new List<int>()
        {
            items[0].Value,
            items[1].Value,
            items[2].Value
        };
        
        dict.Add(items[0]);
        dict.Add(items[1]);
        dict.Add(items[2]);
        
        dict.Values.Should().BeEquivalentTo(values);
    }

    [Fact]
    public void Clear_EmptyDictionary_Sets_Default_Properties()
    {
        var dict = new EventDictionary<string, int>();
        var initialCapacity = 10;
        var initialCount = 0;
        
        dict.Clear();
        
        Assert.Equal(initialCapacity, dict.Capacity);
        Assert.Equal(initialCount, dict.Count);
    }

    [Fact]
    public void Clear_NonEmptyDictionary_Clears_AllData()
    {
        var dict = new Dictionary<string, int>
        {
            { "1", 1 },
            { "2", 2 },
            { "3", 3 }
        };

        dict.Clear();
        
        Assert.Empty(dict);
        Assert.Empty(dict.Keys);
        Assert.Empty(dict.Values);
    }
    
    [Fact]
    public void TryGetValue_ExistingKey_ReturnsTrue_And_SetsValue()
    {
        var dict = new EventDictionary<string, int>
        {
            new("1", 1)
        };

        var result = dict.TryGetValue("1", out var value);

        Assert.True(result);
        Assert.Equal(1, value);
    }
    
    [Fact]
    public void TryGetValue_NonExistingKey_ReturnsFalse_And_DefaultValue()
    {
        var dictionary = new EventDictionary<string, int>();

        var result = dictionary.TryGetValue("not_exist_key", out var value);

        Assert.False(result);
        Assert.Equal(default, value);
    }
    
    [Fact]
    public void TryGetValue_Throws_ArgumentNullException_If_NullKey()
    {
        var dictionary = new EventDictionary<string, int>();

        Assert.Throws<ArgumentNullException>(() => dictionary.TryGetValue(null, out _));
    }
    
    [Fact]
    public void CopyTo_CopyElementsToArray_Contains_All_Copied_Elements()
    {
        var dict = new EventDictionary<string, int>
        {
            { "1", 1 } ,
            { "2", 2 }
        };

        var array = new KeyValuePair<string, int>[2];

        dict.CopyTo(array, 0);

        dict.Should().BeEquivalentTo(array);
    }
    
    [Fact]
    public void CopyTo_Throws_OutOfRangeException_If_InsufficientSpace()
    {
        var dict = new EventDictionary<string, int>()
        {
            { "1", 1 },
            { "2", 2 }
        };

        var array = new KeyValuePair<string, int>[1];

        Assert.Throws<ArgumentOutOfRangeException>(() => dict.CopyTo(array, 0));
    }

    [Fact]
    public void CopyTo_Throws_ArgumentNullException_If_DestinationArray_IsNull()
    {
        var dict = new EventDictionary<string, int>();

        KeyValuePair<string, int>[] array = null;

        Assert.Throws<ArgumentNullException>(() => dict.CopyTo(array, 0));
    }

    [Fact]
    public void Remove_ExistingKey_ReturnsTrue_And_Remove_Item()
    {
        var dict = new EventDictionary<string, int>();
        var item = new KeyValuePair<string, int>("1", 1);
        
        dict.Add(item);

        var result = dict.Remove(item.Key);
        
        Assert.True(result);
        Assert.DoesNotContain(item, dict);
    }
    
    [Fact]
    public void Remove_Non_ExistingKey_Returns_False()
    {
        var dict = new EventDictionary<string, int>();
        
        var result = dict.Remove("non_exists");
        
        Assert.False(result);
    }
    
    [Fact]
    public void Remove_Throws_ArgumentNullException_If_Key_IsNull()
    {
        var dict = new EventDictionary<string, int>();
        
        Assert.Throws<ArgumentNullException>(() => dict.Remove(null));
    }

    [Fact]
    public void Remove_By_KeyAndValue_Should_Remove_By_Key()
    {
        var dict = new EventDictionary<string, int>();
        var item = new KeyValuePair<string, int>("1", 1);
        
        dict.Add(item);

        var result = dict.Remove(item);
        
        Assert.True(result);
        Assert.DoesNotContain(item, dict);
    }

    [Fact]
    public void Remove_By_KeyAndValue_Throws_ArgumentNullException_If_NullKey()
    {
        var dict = new EventDictionary<string, int>();
        
        Assert.Throws<ArgumentNullException>(() => dict.Remove(new KeyValuePair<string, int>()));
    }
    
    [Fact]
    public void Update_ExistingKey_UpdatesValue()
    {
        var dict = new EventDictionary<string, int>();
        var item = new KeyValuePair<string, int>("1", 1);
        
        dict.Add(item);
        dict.Update(item.Key, 100);
        
        Assert.Equal(100, dict[item.Key]);
    }
    
    [Fact]
    public void Update_Throws_KeyNotFoundException_If_Key_Not_Exists()
    {
        var dict = new EventDictionary<string, int>();

        Assert.Throws<KeyNotFoundException>(() => dict.Update("not_exist", 100));
    }
    
    [Fact]
    public void Update_ThrowsArgumentNullException_If_Key_IsNull()
    {
        var dict = new EventDictionary<string, int>();

        Assert.Throws<ArgumentNullException>(() => dict.Update(null, 100));
    }
    
    [Fact]
    public void Indexer_Get_ExistingKey_Returns_CorrectValue()
    {
        var dict = new EventDictionary<string, int>
        {
            { "1", 1 }
        };

        var value = dict["1"];

        Assert.Equal(1, value);
    }
    
    [Fact]
    public void Indexer_Get_NonExistingKey_Throws_KeyNotFoundException()
    {
        var dict = new EventDictionary<string, int>();

        Assert.Throws<KeyNotFoundException>(() => { var value = dict["nonExistingKey"]; });
    }
    
    [Fact]
    public void Indexer_Set_ExistingKey_UpdatesValue()
    {
        var dict = new EventDictionary<string, int>
        {
            { "1", 1 }
        };

        dict["1"] = 100;

        Assert.Equal(100, dict["1"]);
    }
    
    [Fact]
    public void Indexer_Set_NonExistingKey_AddsNewElement()
    {
        var dict = new EventDictionary<string, int>();

        dict["1"] = 1;

        Assert.Equal(1, dict["1"]);
    }
    
    [Fact]
    public void Constructor_Dictionary_NotEmpty_Initializes_WithItems()
    {
        var dict = new Dictionary<string, int>
        {
            { "key1", 42 },
            { "key2", 100 },
            { "key3", 200 }
        };

        var eventDict = new EventDictionary<string, int>(dict);

        dict.Should().BeEquivalentTo(eventDict);
    }

    [Fact]
    public void OnAdd_Event_Raised()
    {
        var dict = new EventDictionary<string, int>();
        var eventInvoked = false;

        dict.OnAdd += item => eventInvoked = true;
        dict.Add("1", 1);
        
        Assert.True(eventInvoked);
    }
    
    [Fact]
    public void OnUpdate_Event_Raised()
    {
        var dict = new EventDictionary<string, int>()
        {
            { "1", 1 }
        };
        var eventInvoked = false;

        dict.OnUpdate += (key, oldValue, newValue) => eventInvoked = true;
        dict.Update("1", 100);
        
        Assert.True(eventInvoked);
    }

    [Fact]
    public void OnDelete_Event_Raised()
    {
        var dict = new EventDictionary<string, int>()
        {
            { "1", 1 }
        };
        var eventInvoked = false;

        dict.OnRemove += item => eventInvoked = true;
        dict.Remove("1");
        
        Assert.True(eventInvoked);
    }
}