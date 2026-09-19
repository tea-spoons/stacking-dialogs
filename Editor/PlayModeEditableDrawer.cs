#if !TEASPOONS_PACKAGE_CORE
namespace TeaSpoons.StackingDialogs.Editor
{
    using System.Reflection;
    using UnityEditor;
    using UnityEngine;

    /// <summary>
    /// Draws a <see cref="PlayModeEditable{T}"/> as its value and raises its update event when the value is changed.
    /// Stand-in for the drawer of package-core, which is used instead when the project has that package.
    /// </summary>
    [CustomPropertyDrawer(typeof(PlayModeEditable<>), true)]
    internal class PlayModeEditableDrawer : PropertyDrawer
    {
        private static readonly object[] methodCallParameters = new object[2];

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            var valueProperty = property.FindPropertyRelative(PlayModeEditable<object>.PropertyNames.Value);
            var previousValue = valueProperty.boxedValue;

            EditorGUI.BeginChangeCheck();
            EditorGUI.PropertyField(position, valueProperty, label);
            if (EditorGUI.EndChangeCheck())
            {
                InvokeOnUpdateEvent(property, previousValue, valueProperty.boxedValue);
            }

            EditorGUI.EndProperty();
        }

        private static void InvokeOnUpdateEvent(SerializedProperty property, object previousValue, object newValue)
        {
            if (property.TryGetTargetObject(out object target))
            {
                var method = target.GetType().GetMethod(PlayModeEditable<object>.PropertyNames.InvokeOnUpdateEvent, BindingFlags.Instance | BindingFlags.NonPublic);

                methodCallParameters[0] = previousValue;
                methodCallParameters[1] = newValue;
                method.Invoke(target, methodCallParameters);
            }
        }
    }
}
#endif
