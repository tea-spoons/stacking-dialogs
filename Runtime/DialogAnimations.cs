
namespace TeaSpoons.StackingDialogs
{
    using Cysharp.Threading.Tasks;

    public static class DialogAnimations
    {
        public static DialogAnimation DefaultShowAnimation;
        public static DialogAnimation DefaultHideAnimation;
        public static readonly DialogAnimation None = (_, _) => UniTask.CompletedTask;
    }
}
