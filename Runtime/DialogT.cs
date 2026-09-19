
namespace TeaSpoons.StackingDialogs
{
    using UnityEngine;
    using Cysharp.Threading.Tasks;
    using System;

    /// <summary>
    /// Component for the root GameObject of a dialog prefab.
    /// This class represents a parameterized dialog that expects data of type <typeparamref name="T"/> in order to be opened.
    /// </summary>
    [RequireComponent(typeof(CanvasGroup))]
    public abstract class DialogT<T> : DialogBase
    {
        public event Action<T> ParameterUpdated = delegate { };
        [Obsolete("OnUpdateParameter is obsolete, use ParameterUpdated instead.")]
        public event Action<T> OnUpdateParameter
        {
            add => ParameterUpdated += value;
            remove => ParameterUpdated -= value;
        }
        public T CurrentParameter { get; private set; }

        internal override void OnCreatedInternal()
        {
            base.OnCreatedInternal();
            ParameterUpdated(CurrentParameter);
        }

        /// <summary>
        /// Instantiate this prefab and add it to the default <see cref="DialogSpace"/>.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="space"/> is <c>null</c>.</exception>
        public void InstantiateAndOpen(T parameter)
        {
            InstantiateAndOpenAsync(parameter).Forget();
        }

        /// <summary>
        /// Instantiate this prefab and add it to the given <paramref name="space"/>.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="space"/> is <c>null</c>.</exception>
        public void InstantiateAndOpen(T parameter, DialogSpace space)
        {
            InstantiateAndOpenAsync(parameter, space).Forget();
        }

        /// <summary>
        /// Instantiate this prefab and add it to the space that is registered to the given <paramref name="spaceId"/>.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="spaceId"/> is <c>null</c>, or no space is registered to it.</exception>
        public void InstantiateAndOpen(T parameter, DialogSpaceId spaceId)
        {
            InstantiateAndOpenAsync(parameter, spaceId).Forget();
        }

        /// <summary>
        /// Instantiate this prefab and add it to a default <see cref="DialogSpace"/>.<br/>
        /// That's either the space registered with the referenced <see cref="DialogSpaceId"/>, or the <see cref="DialogSpace.DefaultSpace"/>.
        /// </summary>
        public async UniTask<DialogT<T>> InstantiateAndOpenAsync(T parameter)
        {
            return await InstantiateAndOpenAsync(parameter, GetDefaultDialogSpace());
        }

        /// <summary>
        /// Instantiate this prefab and add it to the space that is registered to the given <paramref name="spaceId"/>.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="spaceId"/> is <c>null</c>, or no space is registered to it.</exception>
        public async UniTask<DialogT<T>> InstantiateAndOpenAsync(T parameter, DialogSpaceId spaceId)
        {
            if (!spaceId) throw new ArgumentNullException(nameof(spaceId));

            return await InstantiateAndOpenAsync(parameter, spaceId.DialogSpace);
        }

        /// <summary>
        /// Instantiate this prefab and add it to the given <paramref name="space"/>.
        /// </summary>
        /// <returns>
        /// The instantiated prefab instance as soon as it's fully opened.
        /// </returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="space"/> is <c>null</c>.</exception>
        public async UniTask<DialogT<T>> InstantiateAndOpenAsync(T parameter, DialogSpace space)
        {
            try
            {
                var instance = (DialogT<T>)await InstantiateAsync(space);
                instance.CurrentParameter = parameter;
                instance.OnCreatedInternal();

                space.AddAsync(instance).Forget();

                return instance;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                throw;
            }
        }

        public void UpdateParameter(T parameter)
        {
            CurrentParameter = parameter;
            ParameterUpdated(parameter);
        }
    }
}
