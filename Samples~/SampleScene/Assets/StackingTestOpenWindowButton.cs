namespace TeaSpoons.StackingDialogsTests
{
    using Cysharp.Threading.Tasks;
    using StackingDialogs;
    using UnityEngine;
    using UnityEngine.UI;

    public class StackingTestOpenWindowButton : MonoBehaviour
    {
#if ADDRESSABLES
        [SerializeField]
        private DialogTReference<StackingTestWindow, int> testWindow;

        [SerializeField]
        private Text buttonText;

        private void Awake()
        {
            GetComponent<Button>().onClick.AddListener(OpenWindow);
        }

        private void Update()
        {
            buttonText.text = "Open Window " + (StackingTestWindow.WindowCounter + 1);
        }

        private void OpenWindow()
        {
            testWindow.InstantiateAndOpenAsync(StackingTestWindow.WindowCounter + 1).Forget();
        }
#else
        private void Awake()
        {
            Debug.LogError("Example requires addressables-toolbox package in your '.\\Package' folder! https://github.com/tea-spoons/addressables-toolbox");
        }
#endif
    }
}
