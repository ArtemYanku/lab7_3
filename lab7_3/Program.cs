using System;
using System.Collections.Generic;

public class FunctionCache<TKey, TResult>
{
    private Dictionary<TKey, CachedResult> cache = new Dictionary<TKey, CachedResult>();

    public TResult GetOrAdd(TKey key, Func<TKey, TResult> function, TimeSpan expiration)
    {
        if (cache.ContainsKey(key) && !IsCacheExpired(key))
        {
            return cache[key].Result;
        }

        TResult result = function(key);
        cache[key] = new CachedResult(result, DateTime.Now.Add(expiration));
        return result;
    }

    private bool IsCacheExpired(TKey key)
    {
        DateTime expirationTime = cache[key].ExpirationTime;
        return DateTime.Now >= expirationTime;
    }

    private class CachedResult
    {
        public TResult Result { get; }
        public DateTime ExpirationTime { get; }

        public CachedResult(TResult result, DateTime expirationTime)
        {
            Result = result;
            ExpirationTime = expirationTime;
        }
    }
}

class Program
{
    static void Main()
    {
        FunctionCache<string, int> cache = new FunctionCache<string, int>();

        int result1 = cache.GetOrAdd("key1", CalculateValue, TimeSpan.FromSeconds(5));
        int result2 = cache.GetOrAdd("key1", CalculateValue, TimeSpan.FromSeconds(5));

        Console.WriteLine("Result 1: " + result1);
        Console.WriteLine("Result 2 (from cache): " + result2);
    }

    static int CalculateValue(string key)
    {
        Console.WriteLine("Calculating value for key: " + key);
        return key.Length;
    }
}
