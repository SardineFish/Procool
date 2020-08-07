namespace Procool.Input
{
    public struct InputState<T>
    {
        public T Value { get; private set; }

        public void Set(T value)
            => Value = value;

        public T Get() => Value;

        public T Consume()
        {
            var value = Value;
            Value = default;
            return value;
        }
        
    }
}