namespace TeaSpoons.StackingDialogs.Editor.Tests
{
    using NUnit.Framework;
    using UnityEditor;
    using UnityEngine;

    /// <summary>
    /// The three DialogSpace settings are stored as <c>name: { value: ... }</c> in scenes and prefabs, whichever
    /// PlayModeEditable is used (package-core's or the package's own). Changing that would drop the settings of
    /// existing projects.
    /// </summary>
    public class SerializedLayoutTests
    {
        private GameObject root;
        private SerializedObject serializedObject;

        [SetUp]
        public void SetUp()
        {
            root = new GameObject("DialogSpace", typeof(RectTransform), typeof(Canvas));
            serializedObject = new SerializedObject(root.AddComponent<DialogSpace>());
        }

        [TearDown]
        public void TearDown()
        {
            serializedObject.Dispose();
            Object.DestroyImmediate(root);
        }

        [Test]
        public void IsDefaultSpace_IsStoredAsABoolValue()
        {
            Assert.AreEqual(SerializedPropertyType.Boolean, serializedObject.FindProperty("isDefaultSpace.value").propertyType);
        }

        [Test]
        public void IsStacking_IsStoredAsABoolValue()
        {
            Assert.AreEqual(SerializedPropertyType.Boolean, serializedObject.FindProperty("isStacking.value").propertyType);
        }

        [Test]
        public void SpaceId_IsStoredAsAnObjectReferenceValue()
        {
            Assert.AreEqual(SerializedPropertyType.ObjectReference, serializedObject.FindProperty("spaceId.value").propertyType);
        }

        [Test]
        public void IsStacking_IsOnByDefault()
        {
            Assert.IsTrue(serializedObject.FindProperty("isStacking.value").boolValue);
        }

#if !TEASPOONS_PACKAGE_CORE
        // The stand-in the drawer uses to reach the object that raises the update event.
        [Test]
        public void TheDrawerCanReachThePlayModeEditableOfAProperty()
        {
            Assert.IsTrue(serializedObject.FindProperty("isStacking").TryGetTargetObject<PlayModeEditable<bool>>(out var editable));
            Assert.IsTrue(editable.Value);
        }

        [Test]
        public void TheDrawerFindsNothingForAPropertyWithoutAField()
        {
            // m_Script is a serialized property of every MonoBehaviour but has no field to read a value from.
            Assert.IsFalse(serializedObject.FindProperty("m_Script").TryGetTargetObject<object>(out _));
        }
#endif
    }
}
