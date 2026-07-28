
namespace cuteAudioNet.APIModels.Exceptions
{

    public interface IOperationException {
         Type? ClassThrow { get; }
         Type? Item { get; }
    }
    public class OperationBaseException<T, TItem> : Exception, IOperationException
    {
        public Type? ClassThrow => typeof(T);
        public Type? Item => typeof(TItem);
        public OperationBaseException(string? message) : base(message)
        {
        }
    }

    public class CreateItemBaseFail<T,TItem> : OperationBaseException<T, TItem>
    {
        public CreateItemBaseFail(string? message) : base(message)
        {
            
        }

        public (Type Class, Type Item) GetCulprits => (ClassThrow, Item);
    }

    public class UpdateItemBaseFail<T, TItem> : OperationBaseException<T, TItem>
    {

        public UpdateItemBaseFail(string? message) : base(message)
        {
        }
        public (Type Class, Type Item) GetCulprits => (ClassThrow, Item);

    }

    public class RemoveItemBaseFail<T, TItem> : OperationBaseException<T, TItem>
    {

        public RemoveItemBaseFail(string? message) : base(message)
        {
        }
        public (Type Class, Type Item) GetCulprits => (ClassThrow, Item);

    }

}
