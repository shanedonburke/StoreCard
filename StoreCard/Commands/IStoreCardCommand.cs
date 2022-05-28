namespace StoreCard.Commands;

public interface IStoreCardCommand<out TResult>
{
    public TResult Execute();
}
