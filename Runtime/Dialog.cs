
namespace TeaSpoons.StackingDialogs
{
    using UnityEngine;
#if UNITASK
    using Cysharp.Threading.Tasks;
#endif
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
#if UNITASK
            InstantiateAndOpenAsync().Forget();
#else
            InstantiateAndOpen(GetDefaultDialogSpace());
#endif
        }

        /// <summary>
        /// Instantiate this prefab and add it to the given <paramref name="space"/>.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="space"/> is <c>null</c>.</exception>
        public void InstantiateAndOpen(DialogSpace space)
        {
#if UNITASK
            InstantiateAndOpenAsync(space).Forget();
#else
            var instance = (Dialog)InstantiateInSpace(space);
            instance.OnCreatedInternal();

            space.Add(instance);
#endif
        }

        /// <summary>
        /// Instantiate this prefab and add it to the space that is registered to the given <paramref name="spaceId"/>.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="spaceId"/> is <c>null</c>, or no space is registered to it.</exception>
        public void InstantiateAndOpen(DialogSpaceId spaceId)
        {
#if UNITASK
            InstantiateAndOpenAsync(spaceId).Forget();
#else
            if (!spaceId) throw new ArgumentNullException(nameof(spaceId));

            InstantiateAndOpen(spaceId.DialogSpace);
#endif
        }

#if UNITASK
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
#endif
    }
}
