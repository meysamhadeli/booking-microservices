namespace Flight.Identity.Consumers.RegisterNewUser.V1;

using System.Threading.Tasks;
using BuildingBlocks.Contracts.EventBus.Messages;
using BuildingBlocks.Web;
using Humanizer;
using MassTransit;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

public class RegisterNewUserConsumerHandler : IConsumer<UserCreated>
{
    private readonly AppOptions _options;
    private readonly ILogger<RegisterNewUserConsumerHandler> _logger;

    public RegisterNewUserConsumerHandler(IOptions<AppOptions> options,
        ILogger<RegisterNewUserConsumerHandler> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public Task Consume(ConsumeContext<UserCreated> context)
    {
        _logger.LogInformation($"this is a test consumer for {nameof(UserCreated).Underscore()} in {_options.Name}");
        _logger.LogInformation($"we got this message: {context?.Message}");
        return Task.CompletedTask;
    }
}
