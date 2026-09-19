
namespace TeaSpoons.StackingDialogs
{
#if TEASPOONS_PACKAGE_CORE
    using TeaSpoons.PackageCore;
#endif
#if UNITASK
    using TeaSpoons.UniTaskToolbox;
    using Cysharp.Threading.Tasks;
#endif
    using UnityEngine;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// A space in a <see cref="Canvas"/> for dialogs to be instantiated into.
    /// Contains a stack structure for all dialogs in it. Only the topmost dialog will be visible at any given time.
    /// </summary>
    [DefaultExecutionOrder(int.MinValue)] // As we register the DialogSpaces in OnEnable we should run very early to be found during other MBs's Awake()s
    public class DialogSpace : MonoBehaviour
    {
        public static DialogSpace DefaultSpace { get; private set; }

        [SerializeField]
        private PlayModeEditable<bool> isDefaultSpace = false;

        [SerializeField]
        private PlayModeEditable<bool> isStacking = true;

        [SerializeField]
        private PlayModeEditable<DialogSpaceId> spaceId;

        public RectTransform Transform { get; private set; }

        /// <summary>
        /// Invoked when the stack is empty, before opening a first dialog.
        /// </summary>
        public event Action OpeningFirst = delegate { };
        [Obsolete("OnBeforeOpenFirst is obsolete, use OpeningFirst instead.")]
        public event Action OnBeforeOpenFirst
        {
            add => OpeningFirst += value;
            remove => OpeningFirst -= value;
        }
        /// <summary>
        /// Invoked after closing the last dialog on the stack.
        /// </summary>
        public event Action RemovedLast = delegate { };
        [Obsolete("OnAfterRemoveLast is obsolete, use RemovedLast instead.")]
        public event Action OnAfterRemoveLast
        {
            add => RemovedLast += value;
            remove => RemovedLast -= value;
        }

#if UNITASK
        public DialogAnimation ShowAnimation = null;
        public DialogAnimation HideAnimation = null;
#endif

        public bool HasDialogs => stack.Count > 0;

        private readonly List<DialogBase> stack = new();
        private DialogBase topmostDialog => stack[stack.Count - 1];
        private bool expectsDialog = false;

#if UNITASK
        private readonly ReusableCancelSource animationCancellation = new();
#endif

        private void Awake()
        {
            Transform = (RectTransform)transform;
        }

        private void OnEnable()
        {
            // spaceId is only created by serialization, so a space added from code has none.
            RegisterToSpaceId(spaceId?.Value);

            if (isDefaultSpace)
            {
                RegisterAsDefaultSpace();
            }

#if UNITY_EDITOR
            if (spaceId != null) spaceId.OnUpdate += OnUpdateSpaceId;
            isDefaultSpace.OnUpdate += OnUpdateIsDefaultSpace;
#endif
        }

        private void OnDisable()
        {
            spaceId?.Value?.UnsetDialogSpace(this);
            if (DefaultSpace = this)
            {
                DefaultSpace = null;
            }

#if UNITY_EDITOR
            if (spaceId != null) spaceId.OnUpdate -= OnUpdateSpaceId;
            isDefaultSpace.OnUpdate -= OnUpdateIsDefaultSpace;
#endif
        }

        /// <summary>
        /// Closes all dialogs.
        /// </summary>
        public void CloseAll()
        {
#if UNITASK
            CloseAllAsync().Forget();
#else
            if (!HasDialogs) return;

            while (stack.Count > 1)
            {
                stack[0].Destroy();
                stack.RemoveAt(0);
            }

            // Closing the last dialog raises RemovedLast.
            topmostDialog.Close();
#endif
        }

#if UNITASK
        /// <summary>
        /// Closes all dialogs.
        /// </summary>
        public async UniTask CloseAllAsync()
        {
            if (!HasDialogs) return;

            animationCancellation.Cancel();

            while (stack.Count > 1)
            {
                stack[0].Destroy();
                stack.RemoveAt(0);
            }

            await topmostDialog.CloseAsync();

            RemovedLast();
        }
#endif

        /// <summary>
        /// Closes only the topmost (currently visible) dialog.
        /// If another dialog is below it on the stack, that dialog is re-revealed.
        /// </summary>
        public void CloseTopmost()
        {
#if UNITASK
            CloseTopmostAsync().Forget();
#else
            if (!HasDialogs) return;

            topmostDialog.Close();
#endif
        }

#if UNITASK
        /// <summary>
        /// Closes only the topmost (currently visible) dialog.
        /// If another dialog is below it on the stack, that dialog is re-revealed.
        /// </summary>
        public async UniTask CloseTopmostAsync()
        {
            if (!HasDialogs) return;

            // DialogBase.CloseAsync() routes through ParentSpace.RemoveAsync(this), which plays the
            // hide animation, removes the dialog from the stack, and re-shows the new topmost dialog
            // (or fires RemovedLast when the stack becomes empty).
            await topmostDialog.CloseAsync();
        }
#endif

        /// <summary>
        /// Makes the stack act like it's getting a dialog added to it, but that dialog might still be loading.
        /// If the stack was empty, this triggers <see cref="OpeningFirst"/> right away, rather than waiting for the loaded dialog to actually be added.
        /// </summary>
        internal void PrepareForDialog()
        {
            if (HasDialogs)
            {
                topmostDialog.DisableInteractions();
            }
            else
            {
                OpeningFirst();
            }
            expectsDialog = true;
        }

#if UNITASK
        /// <summary>
        /// Adds the given <paramref name="dialog"/> to the stack.
        /// Hides whatever dialog is currently visible and shows the new one.
        /// </summary>
        /// <remarks>
        /// The dialog is added to the stack immediately, even if another dialog has to close first.
        /// </remarks>
        internal async UniTask AddAsync(DialogBase dialog)
        {
            animationCancellation.Cancel();

            if (isStacking)
            {
                var isFirstDialog = !HasDialogs;
                DialogBase previousTopmostDialog = null;
                if (HasDialogs)
                {
                    previousTopmostDialog = topmostDialog;
                }

                var dialogWasExpected = expectsDialog;
                expectsDialog = false;

                stack.Add(dialog);

                if (isFirstDialog)
                {
                    if (!dialogWasExpected)
                    {
                        OpeningFirst();
                    }
                }
                else
                {
                    await previousTopmostDialog.HideAsync(animationCancellation.Token, HideAnimation);
                }
            }

            await dialog.ShowAsync(animationCancellation.Token, ShowAnimation);
        }

        /// <summary>
        /// Removes the given <paramref name="dialog"/> from the stack.
        /// If the dialog is currently visible, it will be "animated away". If not, it will silently vanish.
        /// </summary>
        /// <remarks>
        /// The dialog is removed from the stack after it has closed.
        /// </remarks>
        internal async UniTask RemoveAsync(DialogBase dialog)
        {
            if (!isStacking)
            {
                await dialog.HideAsync(animationCancellation.Token, HideAnimation);
                return;
            }

            var dialogIndex = stack.IndexOf(dialog);
            if (dialogIndex < 0)
            {
                // TODO any kind of feedback? exception?
                return;
            }

            animationCancellation.Cancel();

            var closedDialogWasTopmost = dialogIndex == stack.Count - 1;
            if (closedDialogWasTopmost)
            {
                await topmostDialog.HideAsync(animationCancellation.Token, HideAnimation);
            }

            // The index can change until this point
            dialogIndex = stack.IndexOf(dialog);
            closedDialogWasTopmost = dialogIndex == stack.Count - 1;

            if (dialogIndex >= 0)
            {
                stack.RemoveAt(dialogIndex);
            }

            if (closedDialogWasTopmost && HasDialogs)
            {
                await topmostDialog.ShowAsync(animationCancellation.Token, ShowAnimation);
            }
            else if (!HasDialogs)
            {
                RemovedLast();
            }
        }

#else
        /// <summary>
        /// Adds the given <paramref name="dialog"/> to the stack. Hides the dialog that is currently visible and shows the new one.
        /// </summary>
        internal void Add(DialogBase dialog)
        {
            if (isStacking)
            {
                var isFirstDialog = !HasDialogs;
                var previousTopmostDialog = isFirstDialog ? null : topmostDialog;

                var dialogWasExpected = expectsDialog;
                expectsDialog = false;

                stack.Add(dialog);

                if (isFirstDialog)
                {
                    if (!dialogWasExpected)
                    {
                        OpeningFirst();
                    }
                }
                else
                {
                    previousTopmostDialog.Hide();
                }
            }

            dialog.Show();
        }

        /// <summary>
        /// Removes the given <paramref name="dialog"/> from the stack. If it was the visible one, the dialog below it is shown again.
        /// </summary>
        internal void Remove(DialogBase dialog)
        {
            if (!isStacking)
            {
                dialog.Hide();
                return;
            }

            var dialogIndex = stack.IndexOf(dialog);
            if (dialogIndex < 0)
            {
                return;
            }

            var closedDialogWasTopmost = dialogIndex == stack.Count - 1;
            if (closedDialogWasTopmost)
            {
                dialog.Hide();
            }

            stack.RemoveAt(dialogIndex);

            if (closedDialogWasTopmost && HasDialogs)
            {
                topmostDialog.Show();
            }
            else if (!HasDialogs)
            {
                RemovedLast();
            }
        }
#endif

        private void OnDestroy()
        {
#if UNITASK
            animationCancellation.Dispose();
#endif

            if (this == DefaultSpace)
            {
                DefaultSpace = null;
            }
        }

        private void RegisterToSpaceId(DialogSpaceId spaceId)
        {
            if (spaceId == null) return;
            
            if (!spaceId.TrySetDialogSpace(this))
            {
                Debug.LogError("Could not register to assigned DialogSpaceId.");
            }
        }

#if UNITY_EDITOR
        private void OnUpdateSpaceId(DialogSpaceId previousValue, DialogSpaceId newValue)
        {
            if (previousValue)
            {
                previousValue.UnsetDialogSpace(this);
            }
            RegisterToSpaceId(newValue);
        }

        private void OnUpdateIsDefaultSpace(bool previousValue, bool newValue)
        {
            if (newValue)
            {
                RegisterAsDefaultSpace();
            }
            else
            {
                DefaultSpace = null;
            }
        }
#endif

        private void RegisterAsDefaultSpace()
        {
            if (DefaultSpace == null)
            {
                DefaultSpace = this;
            }
            else
            {
                isDefaultSpace = false;
                Debug.LogError("Multiple default DialogSpaces exist at the same time.");
            }
        }
    }
}
