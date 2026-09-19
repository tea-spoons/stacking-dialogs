using UnityEngine;

namespace TeaSpoons.StackingDialogsTests
{
    using StackingDialogs;
    using UnityEngine.UI;

    public class StackingTestWindow : DialogT<int>
    {
        [SerializeField]
        private Button closeButton;

        [SerializeField]
        private Text windowText;

        public static int WindowCounter { get; private set; }

        protected override void OnCreated()
        {
            base.OnCreated();

            WindowCounter++;
            gameObject.name = "Stacking Window #" + (WindowCounter);

            var windowNumber = CurrentParameter;
            
            windowText.text = "Window " + WindowCounter;
            closeButton.onClick.AddListener(Close);

            // move each additional window a bit more to the lower right
            float windowOffset = 30 * (windowNumber-1);
            (transform as RectTransform).Translate(windowOffset, -windowOffset, 0);

            Debug.Log("Created stacking window #" + windowNumber);
        }

        protected override void OnClose()
        {
            base.OnClose();

            WindowCounter--;
        }
    }
}
