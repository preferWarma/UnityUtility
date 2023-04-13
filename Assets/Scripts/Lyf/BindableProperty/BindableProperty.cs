using System;

namespace Lyf.BindableProperty
{
    // 来源: QFramework: 链接:https://qframework.cn
    public interface IUnRegister
    {
        void UnRegister();
    }
    
    public interface IReadonlyBindableProperty<T>
        {
            T Value { get; }
            
            IUnRegister RegisterWithInitValue(Action<T> action);
            void UnRegister(Action<T> onValueChanged);
            IUnRegister Register(Action<T> onValueChanged);
        }
    public interface IBindableProperty<T> : IReadonlyBindableProperty<T>
    {
        new T Value { get; set; }
        void SetValueWithoutEvent(T newValue);
    }

    

    public class BindableProperty<T> : IBindableProperty<T>
    {
        public BindableProperty(T defaultValue = default)
        {
            _mValue = defaultValue;
        }

        private T _mValue;

        public T Value
        {
            get => GetValue();
            set
            {
                if (value == null && _mValue == null) return;
                if (value != null && value.Equals(_mValue)) return;

                SetValue(value);
                _mOnValueChanged?.Invoke(value);
            }
        }

        protected virtual void SetValue(T newValue)
        {
            _mValue = newValue;
        }

        protected virtual T GetValue()
        {
            return _mValue;
        }

        public void SetValueWithoutEvent(T newValue)
        {
            _mValue = newValue;
        }

        private Action<T> _mOnValueChanged = _ => { };

        public IUnRegister Register(Action<T> onValueChanged)
        {
            _mOnValueChanged += onValueChanged;
            return new BindablePropertyUnRegister<T>
            {
                BindableProperty = this,
                OnValueChanged = onValueChanged
            };
        }

        public IUnRegister RegisterWithInitValue(Action<T> onValueChanged)
        {
            onValueChanged(_mValue);
            return Register(onValueChanged);
        }

        public static implicit operator T(BindableProperty<T> property)
        {
            return property.Value;
        }

        public override string ToString()
        {
            return Value.ToString();
        }

        public void UnRegister(Action<T> onValueChanged)
        {
            _mOnValueChanged -= onValueChanged;
        }
    }

    public class BindablePropertyUnRegister<T> : IUnRegister
    {
        public BindableProperty<T> BindableProperty { get; set; }

        public Action<T> OnValueChanged { get; set; }

        public void UnRegister()
        {
            BindableProperty.UnRegister(OnValueChanged);

            BindableProperty = null;
            OnValueChanged = null;
        }
    }

}