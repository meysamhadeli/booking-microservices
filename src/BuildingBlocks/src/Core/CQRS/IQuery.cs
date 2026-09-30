using MediatR;

namespace Woo.Core.CQRS;

public interface IQuery<out T> : IRequest<T>
    where T : notnull
{
}