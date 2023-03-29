using IdGen;

namespace BuildingBlocks.IdsGenerator;

// Ref: https://github.com/RobThree/IdGen
// https://github.com/RobThree/IdGen/issues/34
// https://www.callicoder.com/distributed-unique-id-sequence-number-generator/
public static class SnowflakeIdGenerator
{
    private static readonly IdGenerator Generator;
    static SnowflakeIdGenerator()
    {
        // Read `GENERATOR_ID` from .env file in service root folder or system environment variables
        var generatorId = DotNetEnv.Env.GetInt("GENERATOR_ID", 0);

        // Let's say we take current time as our epoch
        var epoch = DateTime.Now;

        // Create an ID with 45 bits for timestamp, 2 for generator-id
        // and 16 for sequence
        var structure = new IdStructure(45, 2, 16);

        // Prepare options
        var options = new IdGeneratorOptions(structure, new DefaultTimeSource(epoch));

        // Create an IdGenerator with it's generator-id set to 0, our custom epoch
        // and id-structure
        Generator = new IdGenerator(generatorId, options);
    }

    public static long NewId()
    {
        return Generator.CreateId();
    }
}
