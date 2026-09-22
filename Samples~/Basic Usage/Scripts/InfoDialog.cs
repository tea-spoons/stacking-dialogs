namespace TeaSpoons.StackingDialogs.Samples
{
    using UnityEngine;
    using UnityEngine.UI;

    /// <summary>
    /// A plain, non-parameterized dialog: shows a fixed message and closes itself on click.
    /// </summary>
    public class InfoDialog : Dialog
    {
        [SerializeField] private Text messageText;
        [SerializeField] private Button okButton;

        protected override void Awake()
        {
            base.Awake();

            okButton.onClick.AddListener(Close);
        }

        protected override void OnCreated()
        {
            messageText.text = "This is an InfoDialog: a plain Dialog with no parameters.";
        }
    }
}
