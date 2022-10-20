using Flight.Aircrafts.Features.CreateAircraft;
using Flight.Aircrafts.Features.CreateAircraft.Commands.V1;
using FluentAssertions;
using FluentValidation.TestHelper;
using Unit.Test.Common;
using Unit.Test.Fakes;
using Xunit;

namespace Unit.Test.Aircraft.Features.CreateAircraftTests;

[Collection(nameof(UnitTestFixture))]
public class CreateAircraftCommandValidatorTests
{
    [Fact]
    public void is_valid_should_be_false_when_have_invalid_parameter()
    {
        // Arrange
        var command = new FakeValidateCreateAircraftCommand().Generate();
        var validator = new CreateAircraftCommandValidator();

        // Act
        var result = validator.TestValidate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(x => x.Model);
        result.ShouldHaveValidationErrorFor(x => x.ManufacturingYear);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }
}
