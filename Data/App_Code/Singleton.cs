using System;

/// <summary>
/// Provides a global point of access to a single instance of a given class.
/// </summary>
/// <typeparam name="T">The type to provide a singleton instance for.</typeparam>
public static class Singleton<T>
    where T : new()
{
    /// <summary>
    /// Gets the singleton default.
    /// </summary>
    public static T Default
    {
        get
        {
            // Thread safe, lazy implementation of the singleton pattern.
            // See http://www.yoda.arachsys.com/csharp/singleton.html
            // for the full story.
            return SingletonInternal.instance;
        }
    }

    private class SingletonInternal
    {
        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit.
        static SingletonInternal() { }

        internal static T instance = new T();
    }
}
