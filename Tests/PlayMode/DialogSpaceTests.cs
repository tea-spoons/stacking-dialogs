namespace TeaSpoons.StackingDialogs.Tests
{
    using System;
    using System.Collections;
    using System.Linq;
    using NUnit.Framework;
    using UnityEngine;
    using UnityEngine.TestTools;

    /// <summary>
    /// The same tests run with and without UniTask: with it dialogs open through the animated async path, without it
    /// through the plain Instantiate path. Either way the stack must behave the same.
    /// </summary>
    public class DialogSpaceTests
    {
        private GameObject root;
        private DialogSpace space;
        private int openingFirstCount;
        private int removedLastCount;

        [SetUp]
        public void SetUp()
        {
            openingFirstCount = 0;
            removedLastCount = 0;

            root = new GameObject("DialogSpace", typeof(RectTransform), typeof(Canvas));
            space = root.AddComponent<DialogSpace>();
            space.OpeningFirst += () => openingFirstCount++;
            space.RemovedLast += () => removedLastCount++;
        }

        [TearDown]
        public void TearDown()
        {
            UnityEngine.Object.Destroy(root);
        }

#if UNITY_EDITOR
        // Raises the update event of a setting the way the inspector drawer does when the value is edited in Play Mode.
        private static void EditInInspector(DialogSpace target, string field, object previousValue, object newValue)
        {
            var editable = typeof(DialogSpace).GetField(field, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(target);
            editable.GetType().GetMethod("InvokeOnUpdateEvent", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .Invoke(editable, new[] { previousValue, newValue });
        }

        [Test]
        public void EditingIsDefaultSpaceInTheInspectorRegistersAndUnregistersTheDefaultSpace()
        {
            Assert.IsNull(DialogSpace.DefaultSpace);

            EditInInspector(space, "isDefaultSpace", false, true);
            Assert.AreSame(space, DialogSpace.DefaultSpace);

            EditInInspector(space, "isDefaultSpace", true, false);
            Assert.IsNull(DialogSpace.DefaultSpace);
        }
#endif

        [UnityTest]
        public IEnumerator OpeningADialogShowsItAndEnablesItsInteractions()
        {
            CreateTemplate<TestDialog>().InstantiateAndOpen(space);

            yield return WaitFor(() => Dialogs().Length == 1 && Dialogs()[0].AreInteractionsEnabled);

            var dialog = Dialogs()[0];
            Assert.IsTrue(dialog.gameObject.activeSelf);
            Assert.IsTrue(space.HasDialogs);
            Assert.AreEqual(1, openingFirstCount);
            Assert.AreEqual(1, dialog.CreatedCount);
        }

        [UnityTest]
        public IEnumerator OpeningASecondDialogHidesTheFirstOne()
        {
            var template = CreateTemplate<TestDialog>();

            template.InstantiateAndOpen(space);
            yield return WaitFor(() => Dialogs().Length == 1 && Dialogs()[0].AreInteractionsEnabled);

            template.InstantiateAndOpen(space);
            yield return WaitFor(() => Dialogs().Length == 2 && Dialogs()[1].AreInteractionsEnabled);

            Assert.IsFalse(Dialogs()[0].gameObject.activeSelf, "Only the topmost dialog is visible.");
            Assert.IsTrue(Dialogs()[1].gameObject.activeSelf);
            Assert.AreEqual(1, openingFirstCount, "The stack was only empty once.");
        }

        [UnityTest]
        public IEnumerator ClosingTheTopmostDialogRevealsThePreviousOne()
        {
            var template = CreateTemplate<TestDialog>();
            template.InstantiateAndOpen(space);
            yield return WaitFor(() => Dialogs().Length == 1 && Dialogs()[0].AreInteractionsEnabled);
            template.InstantiateAndOpen(space);
            yield return WaitFor(() => Dialogs().Length == 2 && Dialogs()[1].AreInteractionsEnabled);
            var first = Dialogs()[0];

            space.CloseTopmost();

            yield return WaitFor(() => Dialogs().Length == 1 && first.gameObject.activeSelf && first.AreInteractionsEnabled);
            Assert.AreEqual(0, removedLastCount, "A dialog is still on the stack.");
        }

        [UnityTest]
        public IEnumerator ClosingTheLastDialogEmptiesTheStackAndRaisesRemovedLast()
        {
            CreateTemplate<TestDialog>().InstantiateAndOpen(space);
            yield return WaitFor(() => Dialogs().Length == 1 && Dialogs()[0].AreInteractionsEnabled);

            Dialogs()[0].Close();

            yield return WaitFor(() => !space.HasDialogs && Dialogs().Length == 0);
            Assert.AreEqual(1, removedLastCount);
        }

        [UnityTest]
        public IEnumerator CloseAllRemovesEveryDialog()
        {
            var template = CreateTemplate<TestDialog>();
            template.InstantiateAndOpen(space);
            yield return WaitFor(() => Dialogs().Length == 1 && Dialogs()[0].AreInteractionsEnabled);
            template.InstantiateAndOpen(space);
            yield return WaitFor(() => Dialogs().Length == 2 && Dialogs()[1].AreInteractionsEnabled);

            space.CloseAll();

            yield return WaitFor(() => !space.HasDialogs && Dialogs().Length == 0);
            Assert.GreaterOrEqual(removedLastCount, 1);
        }

        [UnityTest]
        public IEnumerator ADialogWithAParameterReceivesIt()
        {
            var template = CreateTemplate<TestParameterDialog>();

            template.InstantiateAndOpen(42, space);

            yield return WaitFor(() => space.GetComponentsInChildren<TestParameterDialog>(true).Length == 1);
            Assert.AreEqual(42, space.GetComponentInChildren<TestParameterDialog>(true).CurrentParameter);
        }

#if !UNITASK
        // Without UniTask InstantiateAndOpen runs synchronously, so the documented exception reaches the caller.
        // With UniTask it is fire and forget: the same error is reported through the log instead of being thrown.
        [Test]
        public void OpeningWithoutASpaceIsRejected()
        {
            var template = CreateTemplate<TestDialog>();

            Assert.Throws<ArgumentNullException>(() => template.InstantiateAndOpen((DialogSpace)null));
        }
#endif

        // A stand-in for a prefab: an inactive scene object that gets cloned into the space.
        private static T CreateTemplate<T>() where T : DialogBase
        {
            var template = new GameObject("Template", typeof(RectTransform), typeof(CanvasGroup));
            template.SetActive(false);
            return template.AddComponent<T>();
        }

        private TestDialog[] Dialogs()
        {
            return space.GetComponentsInChildren<TestDialog>(true);
        }

        private static IEnumerator WaitFor(Func<bool> condition)
        {
            for (var frame = 0; frame < 300 && !condition(); frame++)
            {
                yield return null;
            }

            Assert.IsTrue(condition(), "Timed out waiting for the dialogs to settle.");
        }
    }
}
