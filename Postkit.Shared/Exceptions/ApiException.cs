namespace Postkit.Shared.Exceptions
{
    public abstract class ApiException : Exception
    {
        public abstract int StatusCode { get;}
        public virtual object? Errors => null;
        protected ApiException(string message) : base(message) { }
    }
}
