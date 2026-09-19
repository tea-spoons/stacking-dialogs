
namespace TeaSpoons.StackingDialogs
{
    using UnityEngine;
    using Cysharp.Threading.Tasks;
    using System;

    /// <summary>
    /// Component for the root GameObject of a dialog prefab.
    /// This class represents a non-parameterized dialog that can be opened without suppliying specific data.
    /// </summary>
    [RequireComponent(typeof(CanvasGroup))]
    public class Dialog : DialogBase
    {
        /// <summary>
        /// Instantiate this prefab and add it to the default <see cref="DialogSpace"/>.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="space"/> is <c>null</c>.</exception>
        public void InstantiateAndOpen()
        {
            InstantiateAndOpenAsync().Forget();
        }

        /// <summary>
        /// Instantiate this prefab and add it to the given <paramref name="space"/>.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="space"/> is <c>null</c>.</exception>
        public void InstantiateAndOpen(DialogSpace space)
        {
            InstantiateAndOpenAsync(space).Forget();
        }

        /// <summary>
        /// Instantiate this prefab and add it to the space that is registered to the given <paramref name="spaceId"/>.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="spaceId"/> is <c>null</c>, or no space is registered to it.</exception>
        public void InstantiateAndOpen(DialogSpaceId spaceId)
        {
            InstantiateAndOpenAsync(spaceId).Forget();
        }

        /// <summary>
        /// Instantiate this prefab and add it to a default <see cref="DialogSpace"/>.<br/>
        /// That's either the space registered with the referenced <see cref="DialogSpaceId"/>, or the <see cref="DialogSpace.DefaultSpace"/>.
        /// </summary>
        public async UniTask<Dialog> InstantiateAndOpenAsync()
        {
            return await InstantiateAndOpenAsync(GetDefaultDialogSpace());
        }

        /// <summary>
        /// Instantiate this prefab and add it to the space that is registered to the given <paramref name="spaceId"/>.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="spaceId"/> is <c>null</c>, or no space is registered to it.</exception>
        public async UniTask<Dialog> InstantiateAndOpenAsync(DialogSpaceId spaceId)
        {
            if (!spaceId) throw new ArgumentNullException(nameof(spaceId));

            return await InstantiateAndOpenAsync(spaceId.DialogSpace);
        }

        /// <summary>
        /// Instantiate this prefab and add it to the given <paramref name="space"/>.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="space"/> is <c>null</c>.</exception>
        public async UniTask<Dialog> InstantiateAndOpenAsync(DialogSpace space)
        {
            var instance = (Dialog)await InstantiateAsync(space);
            instance.OnCreatedInternal();

            space.AddAsync(instance).Forget();

            return instance;
        }
    }
}
