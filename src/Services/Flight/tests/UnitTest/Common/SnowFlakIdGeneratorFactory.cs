using BuildingBlocks.IdsGenerator;

namespace Unit.Test.Common;

public static class SnowFlakIdGeneratorFactory
{
    public static void Create()
    {
        SnowFlakIdGenerator.Configure(1);
    }
}
