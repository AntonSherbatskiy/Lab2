using System.Collections;
using System.Runtime.CompilerServices;
using EventDictionaryLib;
namespace Dictionary.Tests;

public class BucketTests
{
    [Fact]
    public void Length_Returns_Zero_If_Empty_Collection()
    {
        // Arrange
        var bucket = new Bucket<string, int>();

        // Act
        int length = bucket.Length;

        // Assert
        Assert.Equal(0, length);
    }

    [Fact]
    public void Add_IncreasesArrayLength_ByOne()
    {
        // Arrange
        var bucket = new Bucket<string, int>();
        int initialLength = bucket.Length;

        // Act
        var item = new KeyValuePair<string, int>("1", 1);
        bucket.Add(item);

        // Assert
        Assert.Equal(initialLength + 1, bucket.Length);
    }
    
    [Fact]
    public void Bucket_Contains_AddedItem()
    {
        // Arrange
        var bucket = new Bucket<string, int>();
        var item1 = new KeyValuePair<string, int>("1", 1);

        // Act
        bucket.Add(item1);

        // Assert
        Assert.Equal(item1, bucket._items[^1]);
    }

    [Fact]
    public void Remove_Item_WhenPresent_MustRemoveKey()
    {
        var bucket = new Bucket<string, int>();
        var key = "1";
        var item = new KeyValuePair<string, int>(key, 1);
        
        bucket.Add(item);
        bucket.Remove(key);
        
        Assert.False(bucket.Contains(key));
    }

    [Fact]
    public void Remove_Item_Must_Decrease_ItemsCount()
    {
        var bucket = new Bucket<string, int>();
        var item1 = new KeyValuePair<string, int>("1", 1);
        var item2 = new KeyValuePair<string, int>("2", 2);
        
        bucket.Add(item1);
        bucket.Add(item2);
        
        bucket.Remove(item1.Key);
        bucket.Remove(item2.Key);
        
        Assert.Equal(0, bucket.Length);
    }

    [Fact]
    public void Update_Existing_Key_Should_Update_Value()
    {
        var bucket = new Bucket<string, int>();
        var item = new KeyValuePair<string, int>("1", 1);
        var newValue = 2;
        
        bucket.Add(item);
        
        bucket.Update("1", newValue);
        
        Assert.Equal(newValue, bucket.Get(item.Key));
    }

    [Fact]
    public void Update_Existing_Key_Should_Not_Add_New_Item()
    {
        var bucket = new Bucket<string, int>();
        var item = new KeyValuePair<string, int>("1", 1);
        var newValue = 2;
        var initialCount = bucket.Length;
        
        bucket.Update(item.Key, newValue);
        
        Assert.Equal(initialCount, bucket.Length);
    }

    [Fact]
    public void Update_NonExisting_Key_Should_Not_Add_New_Item()
    {
        var bucket = new Bucket<string, int>();
        var item = new KeyValuePair<string, int>("1", 1);
        
        bucket.Add(item);
        
        bucket.Update("2", 2);
        
        Assert.Equal(item.Value, bucket.Get(item.Key));
    }

    [Fact]
    public void If_Key_Exists_Then_Bucket_Must_Contain_Key()
    {
        var bucket = new Bucket<string, int>();
        var item = new KeyValuePair<string, int>("1", 1);
        
        bucket.Add(item);
        
        Assert.True(bucket.Contains(item.Key));
    }

    [Fact]
    public void Contains_NonExisting_Key_Must_Return_False()
    {
        var bucket = new Bucket<string, int>();
        var item = new KeyValuePair<string, int>("1", 2);
        
        bucket.Add(item);
        
        Assert.False(bucket.Contains("2"));
    }

    [Fact]
    public void Get_Existing_Key_Should_Return_Value()
    {
        var bucket = new Bucket<string, int>();
        var item = new KeyValuePair<string, int>("1", 1);
        
        bucket.Add(item);
        
        Assert.Equal(item.Value, bucket.Get(item.Key));
    }

    [Fact]
    public void Get_NonExisting_Key_Must_Return_Default_Value()
    {
        var bucket = new Bucket<string, int>();

        var actual = bucket.Get("non existed");
        
        Assert.Equal(default, actual);
    }

    [Fact]
    public void GetEnumerator_Must_Return_Expected_Items()
    {
        var items = new List<KeyValuePair<string, int>>()
        {
            new KeyValuePair<string, int>("1", 1),
            new KeyValuePair<string, int>("2", 2),
            new KeyValuePair<string, int>("3", 3)
        };
        
        var bucket = new Bucket<string, int>()
        {
            items[0],
            items[1],
            items[2]
        };

        using var enumerator = bucket.GetEnumerator();
        var enumerated = new List<KeyValuePair<string, int>>();
        
        while (enumerator.MoveNext())
        {
            enumerated.Add(enumerator.Current);    
        }
        
        Assert.Equal(items, enumerated);
    }

    [Fact]
    public void GetEnumerator_Empty_Collection_Must_Return_No_Items()
    {
        var bucket = new Bucket<string, int>();
        using var enumerator = bucket.GetEnumerator();
        
        Assert.False(enumerator.MoveNext());
    }
    
    [Fact]
    public void IEnumerable_GetEnumerator_Must_Return_Expected_Items()
    {
        var items = new List<KeyValuePair<string, int>>()
        {
            new KeyValuePair<string, int>("1", 1),
            new KeyValuePair<string, int>("2", 2),
            new KeyValuePair<string, int>("3", 3)
        };
        
        var bucket = new Bucket<string, int>()
        {
            items[0],
            items[1],
            items[2]
        };

        var enumerator = ((IEnumerable)bucket).GetEnumerator();
        var enumerated = new List<KeyValuePair<string, int>>();
        
        while (enumerator.MoveNext())
        {
            enumerated.Add((KeyValuePair<string, int>)enumerator.Current);    
        }
        
        Assert.Equal(items, enumerated);
    }

    [Fact]
    public void IEnumerable_GetEnumerator_Empty_Collection_Must_Return_No_Items()
    {
        var bucket = new Bucket<string, int>();
        var enumerator = ((IEnumerable)bucket).GetEnumerator();
        
        Assert.False(enumerator.MoveNext());
    }
}