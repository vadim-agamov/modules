namespace Modules.Actions
{
    public struct Result<T>
    {
        public T Value { get; private set; }
        public bool Success { get; private set; }

        public static Result<T> Succeed(T value)
        {
            return new Result<T>
            {
                Value = value,
                Success = true
            };
        }
        
        public static Result<T> Failed()
        {
            return new Result<T>
            {
                Success = false
            };
        }
    }
    
    public struct Void { }
}