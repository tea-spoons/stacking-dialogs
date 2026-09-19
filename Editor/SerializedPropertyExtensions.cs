#if !TEASPOONS_PACKAGE_CORE
namespace TeaSpoons.StackingDialogs.Editor
{
    using System;
    using System.Collections;
    using System.Linq;
    using System.Reflection;
    using UnityEditor;

    /// <summary>
    /// Stand-in for <c>TeaSpoons.PackageCore.Editor.SerializedPropertyExtensions</c>, which is used instead when the
    /// project has package-core.
    /// </summary>
    internal static class SerializedPropertyExtensions
    {
        /// <summary>
        /// Finds the value of the <see cref="SerializedProperty"/> as a regular managed object.
        /// </summary>
        public static bool TryGetTargetObject(this SerializedProperty property, out object targetObject)
        {
            if (property == null) throw new ArgumentNullException(nameof(property));

            targetObject = property.serializedObject.targetObject;
            if (targetObject == null)
            {
                return false;
            }

            var path = property.propertyPath.Split('.');
            for (var i = 0; i < path.Length; i++)
            {
                if (IsArray(path, i))
                {
                    i++;
                    targetObject = GetArrayItemAtIndex(targetObject, ExtractArrayIndex(path[i]));
                }
                else
                {
                    var field = GetFieldIncludingInherited(targetObject.GetType(), path[i]);
                    if (field == null)
                    {
                        targetObject = null;
                        return false;
                    }

                    targetObject = field.GetValue(targetObject);
                }

                if (targetObject == null)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Finds the value of the <see cref="SerializedProperty"/> as a regular managed object of type <typeparamref name="T"/>.
        /// </summary>
        public static bool TryGetTargetObject<T>(this SerializedProperty property, out T targetObject)
        {
            if (property.TryGetTargetObject(out object uncast) && uncast is T cast)
            {
                targetObject = cast;
                return true;
            }

            targetObject = default;
            return false;
        }

        private static FieldInfo GetFieldIncludingInherited(Type type, string name)
        {
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy;

            FieldInfo field = null;
            while (type != null && field == null)
            {
                field = type.GetField(name, flags);
                type = type.BaseType;
            }

            return field;
        }

        private static object GetArrayItemAtIndex(object target, int index)
        {
            if (target is Array array)
            {
                return index >= 0 && index < array.Length ? array.GetValue(index) : null;
            }

            return target is ICollection collection ? collection.Cast<object>().ElementAtOrDefault(index) : null;
        }

        // The pattern for arrays in a propertyPath is "Array.data[index]".
        private static bool IsArray(string[] path, int index)
        {
            return index + 1 < path.Length && path[index] == "Array" && path[index + 1].StartsWith("data[");
        }

        private static int ExtractArrayIndex(string s)
        {
            const int start = 5; // "data[".Length
            return int.Parse(s.Substring(start, s.Length - start - 1));
        }
    }
}
#endif
