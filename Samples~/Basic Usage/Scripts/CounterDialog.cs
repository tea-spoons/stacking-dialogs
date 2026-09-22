namespace TeaSpoons.StackingDialogs.Samples
{
    using UnityEngine;
    using UnityEngine.UI;

    /// <summary>
    /// A parameterized dialog (<see cref="DialogT{T}"/> of <see cref="int"/>): opened with a starting
    /// count, and can push another CounterDialog on top with the count incremented - showing a
    /// parameterized dialog and stacking together.
    /// </summary>
    public class CounterDialog : DialogT<int>
    {
        // A serialized field referencing this same prefab would NOT work here: Unity's Instantiate
        // remaps a self-reference to point at the new clone instead of the original prefab asset, so
        // every clone would end up pointing at itself instead of the source prefab. The sample's
        // launcher sets this once instead, from its own (non-self-referential) prefab field.
        public static CounterDialog Prefab;

        [SerializeField] private Text countText;
        [SerializeField] private Button pushAnotherButton;
        [SerializeField] private Button closeButton;

        protected override void Awake()
        {
            base.Awake();

            pushAnotherButton.onClick.AddListener(PushAnother);
            closeButton.onClick.AddListener(Close);

            ParameterUpdated += count => countText.text = $"Count: {count}";
        }

        private void PushAnother()
        {
            Prefab.InstantiateAndOpen(CurrentParameter + 1);
        }
    }
}
