namespace GePro.Business.Tests.Mapping
{
    public class MappingConfigurationTests
    {
        [Fact]
        public void AssertConfigurationIsValid_ProfilesDeBusiness_NoLanza()
        {
            var configuration = MapperFactory.CreateConfiguration();

            configuration.AssertConfigurationIsValid();
        }
    }
}
