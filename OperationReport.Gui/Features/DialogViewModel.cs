using CommunityToolkit.Mvvm.ComponentModel;
using FluentResults;

namespace AetherSystem.OperationReport.Gui.Features;

public abstract class DialogViewModel<TResult> : ObservableObject
{
    internal Action<Result<TResult>>? CompleteHandler { get; set; }
    
    protected void Complete(TResult result)
    {
        CompleteHandler?.Invoke(Result.Ok(result));
    }

    protected void SetCancel()
    {
        CompleteHandler?.Invoke(Result.Fail("Operation canceled"));
    }

    protected void SetFail(string message)
    {
        CompleteHandler?.Invoke(Result.Fail(message));
    }
}

public abstract class DialogViewModel : DialogViewModel<bool>
{
    protected void Complete() => Complete(true);
}