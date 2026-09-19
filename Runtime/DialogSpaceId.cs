
namespace TeaSpoons.StackingDialogs
{
#if TEASPOONS_PACKAGE_CORE
    using TeaSpoons.PackageCore;
#endif
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// A <see cref="ScriptableObject"/> that represents a <see cref="DialogSpace"/>.<br/>
    /// Can be used to define a target <see cref="DialogSpace"/> to open a dialog in before that space exists, for example in a prefab.
    /// </summary>
    [CreateAssetMenu(menuName = MenuPaths.Root + "Stacking Dialogs/Dialog Space Id")]
    public class DialogSpaceId : ScriptableObject
    {
        /// <summary>
        /// This dictionary maps the registered dialog spaces onto strings defined by existing dialog space ids.
        /// We do this instead of just storing the reference to the dialog space here
        /// because that approach completely breaks down if the dialogs are addressable and the space ids are not.
        /// </summary>
        private static readonly Dictionary<string, DialogSpace> registeredSpaces = new();

        public DialogSpace DialogSpace
        {
            get => registeredSpaces.GetValueOrDefault(key);
            private set => registeredSpaces[key] = value;
        }

        private string key => name;

        internal bool TrySetDialogSpace(DialogSpace space)
        {
            if (space == null) throw new System.ArgumentNullException(nameof(space));

            if (DialogSpace != null && DialogSpace != space)
            {
                return false;
            }

            DialogSpace = space;
            return true;
        }

        internal void UnsetDialogSpace(DialogSpace space)
        {
            if (space == null) throw new System.ArgumentNullException(nameof(space));

            if (DialogSpace == space)
            {
                DialogSpace = null;
            }
        }
    }
}
