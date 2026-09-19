#if !TEASPOONS_PACKAGE_CORE
namespace TeaSpoons.StackingDialogs
{
    using UnityEngine;

    /// <summary>
    /// A serializable class with a single value that triggers an event if the value is changed through the inspector.
    /// Stand-in for <c>TeaSpoons.PackageCore.PlayModeEditable</c>, which the package uses instead when package-core is
    /// in the project. Both serialize the same way (a single <c>value</c> field), so scenes and prefabs work with either.
    /// </summary>
    [System.Serializable]
    internal sealed class PlayModeEditable<T>
    {
#if UNITY_EDITOR
        internal static class PropertyNames
        {
            public const string Value = nameof(value);
            public const string InvokeOnUpdateEvent = nameof(PlayModeEditable<object>.InvokeOnUpdateEvent);
        }
#endif

        public delegate void UpdateDelegate(T previousValue, T newValue);

        [SerializeField]
        private T value;
        public T Value => value;

#if UNITY_EDITOR
        public UpdateDelegate OnUpdate { get; set; }
#endif

        public PlayModeEditable(T value)
        {
            this.value = value;
        }

#if UNITY_EDITOR
        internal void InvokeOnUpdateEvent(object previousValue, object newValue)
        {
            OnUpdate?.Invoke((T)previousValue, (T)newValue);
        }
#endif

        public static implicit operator PlayModeEditable<T>(T value)
        {
            return new PlayModeEditable<T>(value);
        }

        public static implicit operator T(PlayModeEditable<T> editable)
        {
            return editable.value;
        }
    }
}
#endif
