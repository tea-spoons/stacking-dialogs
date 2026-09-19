namespace TeaSpoons.StackingDialogs
{
    using UnityEngine;
    using UnityEngine.UI;

    /// <summary>
    /// Marks a GameObject as a raycast blocker for dialogs.
    /// </summary>
    [RequireComponent(typeof(Image))]
    public class RaycastBlocker : MonoBehaviour
    {
        private Image image;

        public Image Image => image ??= GetComponent<Image>();

        /// <summary>
        /// Sets whether the blocker should block raycasts.
        /// </summary>
        public void SetBlocked(bool blocked)
        {
            Image.raycastTarget = blocked;
        }
    }
}
