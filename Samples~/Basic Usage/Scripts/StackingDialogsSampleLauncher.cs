namespace TeaSpoons.StackingDialogs.Samples
{
    using UnityEngine;

    /// <summary>
    /// Entry point for the Basic Usage sample scene: opens each sample dialog through the same
    /// <c>InstantiateAndOpen</c> API you'd call from your own game code, wired to the scene's buttons.
    /// </summary>
    public class StackingDialogsSampleLauncher : MonoBehaviour
    {
        [SerializeField] private InfoDialog infoDialogPrefab;
        [SerializeField] private ConfirmDialog confirmDialogPrefab;
        [SerializeField] private CounterDialog counterDialogPrefab;

        private void Awake()
        {
            CounterDialog.Prefab = counterDialogPrefab;
        }

        public void OpenInfoDialog() => infoDialogPrefab.InstantiateAndOpen();

        public void OpenConfirmDialog() => confirmDialogPrefab.InstantiateAndOpen();

        public void OpenCounterDialog() => counterDialogPrefab.InstantiateAndOpen(1);
    }
}
