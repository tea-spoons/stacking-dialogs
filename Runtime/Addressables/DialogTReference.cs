#if ADDRESSABLES && UNITASK && ADDRESSABLES_TOOLBOX

#if ADDRESSABLES
namespace TeaSpoons.StackingDialogs
{
    using TeaSpoons.AddressablesToolbox;
    using Cysharp.Threading.Tasks;

    /// <summary>
    /// A <see cref="SmartComponentReference{DialogT{T}}"/> that can instantiate and return any <see cref="DialogT{TParameter}"/>.
    /// </summary>
    [System.Serializable]
    public class DialogTReference<T> : SmartComponentReference<DialogT<T>>
    {
        public async UniTask<DialogT<T>> InstantiateAndOpenAsync(T parameter)
        {
            var prefab = await GetAssetAsync();

            var instance = await prefab.InstantiateAndOpenAsync(parameter);

            return instance;
        }

        public async UniTask<DialogT<T>> InstantiateAndOpenAsync(T parameter, DialogSpaceId spaceId)
        {
            var prefab = await GetAssetAsync();

            var instance = await prefab.InstantiateAndOpenAsync(parameter, spaceId);

            return instance;
        }

        public async UniTask<DialogT<T>> InstantiateAndOpenAsync(T parameter, DialogSpace space)
        {
            var prefab = await GetAssetAsync();

            var instance = await prefab.InstantiateAndOpenAsync(parameter, space);

            return instance;
        }
    }

    /// <summary>
    /// A <see cref="SmartComponentReference{TDialog}"/> that can instantiate and return a specific <see cref="DialogT{TParameter}"/>.
    /// </summary>
    [System.Serializable]
    public class DialogTReference<TDialog, TParameter> : SmartComponentReference<TDialog>
        where TDialog : DialogT<TParameter>
    {
        public async UniTask<TDialog> InstantiateAndOpenAsync(TParameter parameter)
        {
            var prefab = await GetAssetAsync();

            var instance = (TDialog)await prefab.InstantiateAndOpenAsync(parameter);

            return instance;
        }

        public async UniTask<TDialog> InstantiateAndOpenAsync(TParameter parameter, DialogSpaceId spaceId)
        {
            var prefab = await GetAssetAsync();

            var instance = (TDialog)await prefab.InstantiateAndOpenAsync(parameter, spaceId);

            return instance;
        }

        public async UniTask<TDialog> InstantiateAndOpenAsync(TParameter parameter, DialogSpace space)
        {
            var prefab = await GetAssetAsync();

            var instance = (TDialog)await prefab.InstantiateAndOpenAsync(parameter, space);

            return instance;
        }
    }
}
#endif
#endif
