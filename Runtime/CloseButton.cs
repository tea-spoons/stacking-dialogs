
namespace TeaSpoons.StackingDialogs
{
    using UnityEngine;
    using UnityEngine.UI;

    [RequireComponent(typeof(Button))]
    public class CloseButton : MonoBehaviour
    {
        private void Awake()
        {
            var dialog = GetComponentInParent<DialogBase>();

            var button = GetComponent<Button>();
            button.onClick.AddListener(dialog.Close);
        }
    }
}
