using AutoMapper;
using GePro.Business.Mapping;

namespace GePro.Business.Tests.Mapping
{
    /// <summary>Construye la configuración de AutoMapper con los Profiles reales de GePro.Business.</summary>
    internal static class MapperFactory
    {
        public static MapperConfiguration CreateConfiguration()
        {
            var expression = new MapperConfigurationExpression();
            expression.AddMaps(typeof(GenderProfile).Assembly);

            return new MapperConfiguration(expression);
        }

        public static IMapper CreateMapper()
        {
            var configuration = CreateConfiguration();
            return configuration.CreateMapper();
        }
    }
}
