//This is just a test for extension methods

namespace Helpers;
public static class StringExtenstions
{
    public static bool IsEmpty(this string name)
    {
        return string.IsNullOrEmpty(name);

    }
}

