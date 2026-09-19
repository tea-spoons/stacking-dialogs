namespace TeaSpoons.StackingDialogs.Tests
{
    public class TestDialog : Dialog
    {
        public int CreatedCount { get; private set; }

        protected override void OnCreated()
        {
            CreatedCount++;
        }
    }
}
