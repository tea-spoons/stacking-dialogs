
namespace TeaSpoons.StackingDialogs
{
    using Cysharp.Threading.Tasks;
    using System.Threading;

    public delegate UniTask DialogAnimation(DialogBase dialog, CancellationToken cancel);
}
