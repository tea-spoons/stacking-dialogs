namespace TeaSpoons.StackingDialogsTests
{
    using Cysharp.Threading.Tasks;
    using StackingDialogs;
    using UnityEngine;
    using UnityEngine.UI;

    public class NonStackingTestTwoOpenWindowButton : MonoBehaviour
    {
#if ADDRESSABLES
        [SerializeField]
        private DialogTReference<NonStackingTestTwoWindow, int> testWindow;

        [SerializeField]
        private Text buttonText;

        private int openedWindows = 0;

        private void Awake()
        {
            GetComponent<Button>().onClick.AddListener(OpenWindow);
        }

        private void Update()
        {
            buttonText.text = "Open Window " + (openedWindows + 1);
        }

        private void OpenWindow()
        {
            // Open new window
            testWindow.InstantiateAndOpenAsync(openedWindows + 1).Forget();
            openedWindows++;
        }
#else
        private void Awake()
        {
            Debug.LogError("Example requires addressables-toolbox package in your '.\\Package' folder! https://github.com/tea-spoons/addressables-toolbox");
        }
#endif
    }
}
