using System;

namespace ECF.Domain.Common
{
    public interface IObservableValue<T> where T: struct, IComparable
    {
        T Value { get; set; }
        event Action<T> Changed;
    }
    
    public class ObservableValue<T> : IObservableValue<T> where T: struct, IComparable
    {
        private T value;
        
        public ObservableValue(T value)
        {
            this.value = value;
        }
        
        public T Value
        {
            get => value;
            set
            {
                if (this.value.CompareTo(value) == 0) return;
                this.value = value;
                Changed?.Invoke(this.value);
            }
        }
        
        public event Action<T> Changed;
    }
}