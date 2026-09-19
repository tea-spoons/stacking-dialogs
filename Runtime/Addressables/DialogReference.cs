#if ADDRESSABLES && UNITASK && ADDRESSABLES_TOOLBOX

#if ADDRESSABLES
namespace TeaSpoons.StackingDialogs
{
    using TeaSpoons.AddressablesToolbox;
    using Cysharp.Threading.Tasks;

    /// <summary>
    /// A <see cref="SmartComponentReference{Dialog}"/> that can instantiate and return any <see cref="Dialog"/>.
    /// </summary>
    [System.Serializable]
    public class DialogReference : SmartComponentReference<Dialog>
    {
        public async UniTask<Dialog> InstantiateAndOpenAsync()
        {
            var prefab = await GetAssetAsync();

            var instance = await prefab.InstantiateAndOpenAsync();

            return instance;
        }

        public async UniTask<Dialog> InstantiateAndOpenAsync(DialogSpaceId spaceId)
        {
            var prefab = await GetAssetAsync();

            var instance = await prefab.InstantiateAndOpenAsync(spaceId);

            return instance;
        }

        public async UniTask<Dialog> InstantiateAndOpenAsync(DialogSpace space)
        {
            var prefab = await GetAssetAsync();

            var instance = await prefab.InstantiateAndOpenAsync(space);

            return instance;
        }
    }

    /// <summary>
    /// A <see cref="SmartComponentReference{TDialog}"/> that can instantiate and return a specific <see cref="Dialog"/>.
    /// </summary>
    [System.Serializable]
    public class DialogReference<TDialog> : SmartComponentReference<TDialog>
        where TDialog : Dialog
    {
        public async UniTask<TDialog> InstantiateAndOpenAsync()
        {
            var prefab = await GetAssetAsync();

            var instance = (TDialog)await prefab.InstantiateAndOpenAsync();

            return instance;
        }

        public async UniTask<TDialog> InstantiateAndOpenAsync(DialogSpaceId spaceId)
        {
            var prefab = await GetAssetAsync();

            var instance = (TDialog)await prefab.InstantiateAndOpenAsync(spaceId);

            return instance;
        }

        public async UniTask<TDialog> InstantiateAndOpenAsync(DialogSpace space)
        {
            var prefab = await GetAssetAsync();

            var instance = (TDialog)await prefab.InstantiateAndOpenAsync(space);

            return instance;
        }
    }
}
#endif
#endif
