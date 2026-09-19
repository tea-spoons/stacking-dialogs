using UnityEngine;

namespace TeaSpoons.StackingDialogsTests
{
    using StackingDialogs;
    using UnityEngine.UI;

    public class NonStackingTestTwoWindow : DialogT<int>
    {
        [SerializeField]
        private Button closeButton;

        [SerializeField]
        private Text windowText;
        

        protected override void OnCreated()
        {
            base.OnCreated();

            var windowNumber = CurrentParameter;
            gameObject.name = "Non-Stacking II Window #" + (windowNumber);

            windowText.text = "Window " + windowNumber;
            closeButton.onClick.AddListener(Close);

            // move each new window for some degrees around original origin
            var radius = 30;
            var degrees = 45 * Mathf.Deg2Rad * windowNumber;
            var offset = radius * new Vector2(Mathf.Cos(degrees),Mathf.Sin(degrees));
            (transform as RectTransform).Translate(offset.x, offset.y, 0);

            Debug.Log("Created non-stacking II window #" + windowNumber);
        }
    }
}
