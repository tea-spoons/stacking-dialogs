
#if UNITY_6000_0_OR_NEWER
#define INSTANTIATE_ASYNC
#endif

namespace TeaSpoons.StackingDialogs
{
    using UnityEngine;
    using UnityEngine.UI;
#if UNITASK
    using Cysharp.Threading.Tasks;
    using System.Threading;
#endif
    using System;

    /// <summary>
    /// Abstract base component for the root GameObject of a dialog prefab.
    /// Extended by <see cref="Dialog"/> and <see cref="Dialog{T}"/>.
    /// </summary>
    [RequireComponent(typeof(CanvasGroup))]
    public abstract class DialogBase : MonoBehaviour
    {
        public RectTransform Transform { get; private set; }

        /// <summary>
        /// The <see cref="UnityEngine.CanvasGroup"/> of this dialog.
        /// </summary>
        /// <remarks>
        /// Can be used for fading in and out (see <see cref="CanvasGroup.alpha"/>).
        /// </remarks>
        public CanvasGroup CanvasGroup { get; private set; }

        /// <summary>
        /// <c>true</c> if this dialog was ever visible and interactable before.
        /// </summary>
        /// <remarks>
        /// Can be used to treat the dialog differently during its first appearance, for example for animations or content loading.
        /// </remarks>
        public bool WasEnabledBefore { get; private set; } = false;

        /// <summary>
        /// The <see cref="DialogSpace"/> that this dialog is assigned to.
        /// </summary>
        public DialogSpace ParentSpace { get; private set; }

        /// <summary>
        /// <c>true</c> when the dialog is open and interactable.<br/>
        /// <c>false</c> when it is currently in an opening or closing animation.
        /// </summary>
        public bool AreInteractionsEnabled { get; private set; } = false;

        /// <summary>
        /// The raycast blocker used to block input during animations.
        /// </summary>
        protected RaycastBlocker Blocker { get; private set; }

        [field:Tooltip("If no other space id is given, the dialog will open in the space referenced here.\n" +
            "If this is unset, the default dialog space will be used.")]
        [field:SerializeField]
        protected DialogSpaceId defaultSpaceId { get; private set; }

        /// <summary>
        /// Flag that describes that a dialog was created not through <see cref="InstantiateAndOpen()"/>, but through other means,
        /// like <see cref="Object.Instantiate(Object)"/> or by dragging it into the scene in the editor.
        /// </summary>
        private bool wasManuallyAdded = true;

        private bool closed = false;

        private RectTransform rectTransform => (RectTransform)transform;

#if UNITASK
        private bool isHidingInProcess = false;
#endif

        protected virtual void Awake()
        {
            Transform = GetComponent<RectTransform>();

            CanvasGroup = GetComponent<CanvasGroup>();

            ParentSpace = GetComponentInParent<DialogSpace>();

            CreateBlocker();

            DisableInteractions();
        }

        private void Start()
        {
            if (wasManuallyAdded)
            {
                gameObject.SetActive(false);

#if UNITASK
                ParentSpace?.AddAsync(this).Forget();
#else
                ParentSpace?.Add(this);
#endif

                OnCreatedInternal();
            }
        }

#if UNITASK
        public async UniTask WaitUntilInteractionsEnabled()
        {
            await UniTask.WaitUntil(() => AreInteractionsEnabled, cancellationToken: destroyCancellationToken);
        }
#endif

        /// <summary>
        /// Creates the raycast blocker GameObject.
        /// Override to customize or disable the default blocker.
        /// </summary>
        protected virtual void CreateBlocker()
        {
            var blockerObject = new GameObject("RaycastBlocker");
            blockerObject.transform.SetParent(Transform, false);

            Blocker = blockerObject.AddComponent<RaycastBlocker>();

            // Configure RectTransform (created automatically by Image)
            var rectTransform = blockerObject.GetComponent<RectTransform>();
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
            rectTransform.SetAsLastSibling();

            Blocker.Image.color = Color.clear;
            Blocker.Image.raycastTarget = true;
        }

        /// <summary>
        /// Called when the dialog should block or unblock user input.
        /// </summary>
        /// <param name="blocked"><c>true</c> to block input, <c>false</c> to allow input.</param>
        protected virtual void SetBlocked(bool blocked)
        {
            if (Blocker == null)
            {
                Debug.LogWarning($"[{GetType().Name}] SetBlocked called but no Blocker exists. " +
                    "Override CreateBlocker() to create one or override SetBlocked() for custom behavior.");
                return;
            }

            Blocker.SetBlocked(blocked);
        }

        /// <summary>
        /// Closes the dialog and informs its <see cref="DialogSpace"/> about it, so another dialog might be re-enabled.
        /// </summary>
        public void Close()
        {
#if UNITASK
            CloseAsync().Forget();
#else
            if (closed) return;

            closed = true;
            OnClose();

            if (ParentSpace)
            {
                ParentSpace.Remove(this);
            }

            Destroy();
#endif
        }

#if UNITASK
        /// <summary>
        /// Closes the dialog and informs its <see cref="DialogSpace"/> about it, so another dialog might be re-enabled.
        /// </summary>
        public async UniTask CloseAsync()
        {
            if (closed) return;

            closed = true;
            OnClose();

            if (ParentSpace)
            {
                await ParentSpace.RemoveAsync(this);
            }

            Destroy();
        }
#endif

        internal virtual void OnCreatedInternal()
        {
#pragma warning disable CS0618 // Type or member is obsolete
            OnOpen();
#pragma warning restore CS0618 // Type or member is obsolete
            OnCreated();
        }

        [Obsolete("OnOpen is obsolete. Override OnCreated instead.")]
        protected virtual void OnOpen()
        {

        }

        protected virtual void OnCreated()
        {
            
        }

        protected virtual void OnClose()
        {

        }

        /// <summary>
        /// Instantiate this prefab and with the given <paramref name="space"/> as parent.
        /// </summary>
        /// <remarks>
        /// NOTE: Does not call <c>space.Add</c>, so the dialog is not opened.
        /// This allows you to manually define when to open the dialog, and what to do before that.
        /// </remarks>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="space"/> is <c>null</c>.</exception>
#if UNITASK
        protected async UniTask<DialogBase> InstantiateAsync(DialogSpace space)
        {
            if (!space) throw new ArgumentNullException(nameof(space));

            space.PrepareForDialog();

#if INSTANTIATE_ASYNC
            var instances = await InstantiateAsync(this, space.Transform);
            var instance = instances[0];
            // Fix the anchors, since InstantiateAsync doesn't behave like Instantiate here
            instance.rectTransform.anchorMin = rectTransform.anchorMin;
            instance.rectTransform.anchorMax = rectTransform.anchorMax;
            instance.rectTransform.offsetMin = rectTransform.offsetMin;
            instance.rectTransform.offsetMax = rectTransform.offsetMax;
#else
            // TODO This will cause a warning because there's no await in this method now
            var instance = Instantiate(this, space.Transform);
#endif
            instance.wasManuallyAdded = false;

            instance.gameObject.SetActive(false);

            return instance;
        }
#else
        protected DialogBase InstantiateInSpace(DialogSpace space)
        {
            if (!space) throw new ArgumentNullException(nameof(space));

            space.PrepareForDialog();

            var instance = Instantiate(this, space.Transform);
            instance.wasManuallyAdded = false;

            instance.gameObject.SetActive(false);

            return instance;
        }
#endif

        internal void Destroy()
        {
            if (!closed)
            {
                closed = true;
                OnClose();
            }

            if (this && gameObject)
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// Returns the default dialog space
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception">
        ///   Thrown if the references <see cref="DialogSpaceId"/> has no <see cref="DialogSpace"/> registered,
        ///   or if no id is referenced, and the <see cref="DialogSpace.DefaultSpace"/> is not set.
        /// </exception>
        protected DialogSpace GetDefaultDialogSpace()
        {
            DialogSpace space;

            if (defaultSpaceId)
            {
                space = defaultSpaceId.DialogSpace;
                if (!space)
                {
                    throw new Exception($"The dialog's provided default space id ({defaultSpaceId.name}) has no registered dialog space.");
                }
                return space;
            }

            space = DialogSpace.DefaultSpace;
            if (!space)
            {
                throw new Exception($"There is no default dialog space to open the dialog in.");
            }
            return space;
        }

#if UNITASK
        internal async UniTask ShowAsync(CancellationToken cancel, DialogAnimation animation = null)
        {
            gameObject.SetActive(true);

            if (animation == null)
            {
                animation = DialogAnimations.DefaultShowAnimation;
            }

            if (animation != null)
            {
                await animation(this, cancel);
            }

            EnableInteractions();
        }

        internal async UniTask HideAsync(CancellationToken cancel, DialogAnimation animation = null)
        {
            if (isHidingInProcess)
            {
                // Trying to hide dialog, although we're already about to get hidden... waiting for it...
                await UniTask.WaitUntil(predicate: () => gameObject == null || !isHidingInProcess, cancellationToken: cancel);
                return;
            }

            isHidingInProcess = true;

            DisableInteractions();

            if (animation == null)
            {
                animation = DialogAnimations.DefaultHideAnimation;
            }

            if (animation != null)
            {
                await animation(this, cancel);
            }

            if (this && gameObject)
            {
                gameObject.SetActive(false);
            }

            isHidingInProcess = false;
        }

#else
        internal void Show()
        {
            gameObject.SetActive(true);
            EnableInteractions();
        }

        internal void Hide()
        {
            DisableInteractions();

            if (this && gameObject)
            {
                gameObject.SetActive(false);
            }
        }
#endif

        internal void EnableInteractions()
        {
            AreInteractionsEnabled = true;
            WasEnabledBefore = true;
            SetBlocked(false);
        }

        internal void DisableInteractions()
        {
            AreInteractionsEnabled = false;
            SetBlocked(true);
        }
    }
}
