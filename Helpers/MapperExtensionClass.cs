using Mapster;

namespace PGManagementService.Helpers
{
    public static class MapperExtensionClass
    {
        public static TDestination MapTo<TSource, TDestination>(this TSource source)
        {
            return source.Adapt<TDestination>();
        }
    }
}
