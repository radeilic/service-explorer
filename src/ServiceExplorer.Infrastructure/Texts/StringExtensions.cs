using Fernandezja.ColorHashSharp;

namespace ServiceExplorer.Infrastructure.Texts;

public static class StringExtensions
{
    private static ColorHash _colorHash = new ColorHash();

    public static string ToHexColor(this string value) => $"#{_colorHash.Hex(value)}";

}
