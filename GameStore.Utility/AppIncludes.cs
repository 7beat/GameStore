using GameStore.Models;

namespace GameStore.Utility
{
    public static class AppIncludes
    {
        public static ProductInclude Product { get; }
    }

    public struct ProductInclude
    {
        public readonly string Genre => nameof(Product.Genre);
        public readonly string Platform => nameof(Product.Platform);
    }
}
