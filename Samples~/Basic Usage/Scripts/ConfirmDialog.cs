namespace TeaSpoons.StackingDialogs.Samples
{
    using UnityEngine;
    using UnityEngine.UI;

    /// <summary>
    /// A yes/no dialog. Confirming opens an <see cref="InfoDialog"/> on top without closing this one,
    /// so both sit on the stack at once - only the InfoDialog is visible until it closes.
    /// </summary>
    public class ConfirmDialog : Dialog
    {
        [SerializeField] private Button yesButton;
        [SerializeField] private Button noButton;
        [SerializeField] private InfoDialog confirmedDialogPrefab;

        protected override void Awake()
        {
            base.Awake();

            yesButton.onClick.AddListener(() => confirmedDialogPrefab.InstantiateAndOpen());
            noButton.onClick.AddListener(Close);
        }
    }
}
