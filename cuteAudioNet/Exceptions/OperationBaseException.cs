namespace cuteAudioNet.Exceptions
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

    public interface IDbGetItemOrCollectionFailException {
         string NameCollection { get; init; }
         string NameFunction { get; init; }
    }

    public class DbGetItemOrCollectionFailException : Exception, IDbGetItemOrCollectionFailException
    {
        

        public DbGetItemOrCollectionFailException()
        {
        }

        public DbGetItemOrCollectionFailException(string? message) : base(message)
        {
            
        }

        public string NameCollection { get; init; }
        public string NameFunction { get; init; }
    }

    public class DbGetCollectionIsNull : DbGetItemOrCollectionFailException
    {
        
        public DbGetCollectionIsNull()
        {
        }

        public DbGetCollectionIsNull(string? message, string NameCollection, string? NameFunction) : base(message)
        {

        }
    }

    public class DbGetItemIsNull : DbGetItemOrCollectionFailException
    {

        public DbGetItemIsNull()
        {
        }

        public DbGetItemIsNull(string? message, string NameCollection, string? NameFunction) : base(message)
        {

        }
    }
}
