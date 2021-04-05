namespace Dispatcher
{
    public interface IRequest<out T>
    {

    }

    public interface IRequest : IRequest<EmptyResult>
    {

    }
}
