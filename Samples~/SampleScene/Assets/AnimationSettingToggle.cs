using UnityEngine;

namespace TeaSpoons.StackingDialogs
{
    using System.Threading;
    using Cysharp.Threading.Tasks;
    using UniTaskToolbox.Tweens;
    using UnityEngine.UI;

    public class AnimationSettingToggle : MonoBehaviour
    {
        [SerializeField]
        private Toggle toggle;

        private const float animationDuration = 0.15f;

        private void Awake()
        {
            toggle.onValueChanged.AddListener(ToggleAnimations);

            var defaultSetting = true;
            ToggleAnimations(defaultSetting);
            toggle.SetIsOnWithoutNotify(defaultSetting);
        }

        private void ToggleAnimations(bool isOn)
        {
            Debug.Log("Toggle: " + isOn);
            DialogAnimations.DefaultHideAnimation = isOn ? FadeOutAsync : null;
            DialogAnimations.DefaultShowAnimation = isOn ? FadeInAsync : null;
        }

        public static async UniTask FadeInAsync(DialogBase dialog, CancellationToken cancel)
        {
            await Tweens.Interpolate(0f, 1f,
                animationDuration,
                alpha => dialog.CanvasGroup.alpha = alpha,
                update: Tweens.UpdateRealtime,
                cancellation: cancel);
        }

        public static async UniTask FadeOutAsync(DialogBase dialog, CancellationToken cancel)
        {
            await Tweens.Interpolate(1f, 0f,
                animationDuration,
                alpha => dialog.CanvasGroup.alpha = alpha,
                update: Tweens.UpdateRealtime,
                cancellation: cancel);
        }
    }
}
