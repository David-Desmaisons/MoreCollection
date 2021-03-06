# MoreCollection
[![build](https://img.shields.io/appveyor/ci/David-Desmaisons/morecollection.svg)](https://ci.appveyor.com/project/David-Desmaisons/morecollection)

C# utility extension methods for collection and collections implementation

## IEnumerable Extensions

Cartesian

    public static IEnumerable<TResult> Cartesian<TResult, TSource1, TSource2>(this IEnumerable<TSource1> first,
                    IEnumerable<TSource2> second, Func< TSource1, TSource2, TResult> Agregator )

FirstOrDefault

    public static T FirstOrDefault<T>(this IEnumerable<T> enumerable, T defaultValue, Func<T, bool> predicate)

    public static T FirstOrDefault<T>(this IEnumerable<T> enumerable, T defaultValue)

ForEach

    public static IEnumerable<T> ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)

    public static IEnumerable<T> ForEach<T>(this IEnumerable<T> enumerable, Action<T,int> action)

    public static bool ForEach<T>(this IEnumerable<T> enumerable, Action<T> action,
                                  CancellationToken iCancellationToken)

ForEachCartesian

    public static void ForEachCartesian<TSource1, TSource2>(this IEnumerable<TSource1> first,
                        IEnumerable<TSource2> second, Action<TSource1, TSource2> Do)

Index

    public static int Index<T>(this IEnumerable<T> enumerable, Func<T, bool> Selector)

    public static int Index<T>(this IEnumerable<T> enumerable, T value)

Indexes

    public static IEnumerable<int> Indexes<T>(this IEnumerable<T> enumerable, Func<T, bool> Selector)

    public static IEnumerable<int> Indexes<T>(this IEnumerable<T> enumerable, T value)

Zip

    public static IEnumerable<TResult> Zip<TResult, TSource1, TSource2, TSource3>(
                          this IEnumerable<TSource1> enumerable,
                          IEnumerable<TSource2> enumerable2, IEnumerable<TSource3> enumerable3,
                          Func<TSource1, TSource2, TSource3, TResult> Agregate)
                          
ZipForEach
        
    public static void ZipForEach<TSource1, TSource2>(this IEnumerable<TSource1> enumerable,
                       IEnumerable<TSource2> enumerable2,  Action<TSource1, TSource2> action)



## IList Extensions

AddRange

    public static IList<T> AddRange<T>(this IList<T> list, IEnumerable<T> enumerable)

Move

    public static IList<T> Move<T>(this IList<T> list, int oldIndex, int newIndex)


## IDictionary Extensions

GetOrAdd

    public static CollectionResult<TValue> GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dic, TKey key, Func<TKey, TValue> Fac)
    
GetOrAddEntity

    public static TValue GetOrAddEntity<TKey, TValue>(this IDictionary<TKey, TValue> dic, TKey key, Func<TKey, TValue> Fac)

UpdateOrAdd

    public static TValue UpdateOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dic, TKey key, 
                                        Func<TKey, TValue> Fac, Action<TKey, TValue> Updater)
                                        
    public static TValue UpdateOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dic, TKey key,  Func<TKey, TValue> Fac,
                                        Func<TKey, TValue, TValue> Updater)
