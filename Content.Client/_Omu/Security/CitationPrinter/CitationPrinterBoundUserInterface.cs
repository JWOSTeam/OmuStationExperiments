using System;
using Content.Shared._Omu.CitationPrinter;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;
using Robust.Shared.Timing;

namespace Content.Client._Omu.CitationPrinter;

public sealed class CitationPrinterBoundUserInterface : BoundUserInterface
{
    private CitationPrinterWindow? _window;
    private int _stateRevision;

    public CitationPrinterBoundUserInterface(EntityUid owner, Enum uiKey)
        : base(owner, uiKey)
    {
    }

    protected override void Open()
    {
        base.Open();

        _window = this.CreateWindow<CitationPrinterWindow>();

        _window.PrintRequested += (name, offense, notes) =>
            SendMessage(new CitationPrinterPrintMessage(name, offense, notes));
    }

    protected override void UpdateState(BoundUserInterfaceState state)
    {
        base.UpdateState(state);

        if (state is not CitationPrinterUiState printerState
            || _window is not { } window)
        {
            return;
        }

        var revision = ++_stateRevision;

        if (printerState.RemainingCooldown <= TimeSpan.Zero)
        {
            window.SetPrintReady(true);
            return;
        }

        window.SetPrintReady(false);

        Timer.Spawn(printerState.RemainingCooldown, () =>
        {
            if (_stateRevision != revision || _window != window)
                return;

            window.SetPrintReady(true);
        });
    }

    protected override void Dispose(bool disposing)
    {
        ++_stateRevision;
        _window = null;
        base.Dispose(disposing);
    }
}