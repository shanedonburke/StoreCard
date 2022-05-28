namespace StoreCard.Commands;

internal interface IStoreCardCommand<out TResult>
{
    public TResult Execute();
}
