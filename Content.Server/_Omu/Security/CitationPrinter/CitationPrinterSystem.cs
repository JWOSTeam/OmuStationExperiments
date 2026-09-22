using System;
using Content.Shared.Access.Systems;
using Content.Shared.ActionBlocker;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Paper;
using Content.Shared.UserInterface;
using Content.Shared._Omu.CitationPrinter;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Timing;
using Robust.Shared.Utility;

namespace Content.Server._Omu.CitationPrinter;

public sealed class CitationPrinterSystem : EntitySystem
{
    [Dependency] private readonly SharedUserInterfaceSystem _ui = default!;
    [Dependency] private readonly SharedIdCardSystem _idCards = default!;
    [Dependency] private readonly SharedHandsSystem _hands = default!;
    [Dependency] private readonly ActionBlockerSystem _blocker = default!;
    [Dependency] private readonly PaperSystem _paper = default!;
    [Dependency] private readonly SharedAudioSystem _audio = default!;
    [Dependency] private readonly IGameTiming _timing = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<CitationPrinterComponent,
            BeforeActivatableUIOpenEvent>(OnBeforeUiOpen);

        SubscribeLocalEvent<CitationPrinterComponent,
            CitationPrinterPrintMessage>(OnPrint);
    }

    private void OnBeforeUiOpen(
        EntityUid uid,
        CitationPrinterComponent component,
        BeforeActivatableUIOpenEvent args)
    {
        UpdateUi(uid, component);
    }

    private bool CanUse(EntityUid printer, EntityUid actor)
    {
        if (!_ui.IsUiOpen(printer, CitationPrinterUiKey.Key, actor))
            return false;

        if (_hands.GetActiveItem(actor) != printer)
            return false;

        if (!_blocker.CanInteract(actor, printer)
            || !_blocker.CanComplexInteract(actor))
        {
            return false;
        }

        return TryComp<ActivatableUIComponent>(printer, out var activatable)
            && activatable.CurrentSingleUser == actor;
    }

    private string GetIssuerName(EntityUid actor)
    {
        if (!_idCards.TryFindIdCard(actor, out var card))
            return string.Empty;

        var name = card.Comp.FullName;

        return string.IsNullOrWhiteSpace(name)
            ? string.Empty
            : name.Trim();
    }

    private void UpdateUi(
        EntityUid uid,
        CitationPrinterComponent component)
    {
        var remaining = component.NextPrintAllowedAfter - _timing.CurTime;

        if (remaining < TimeSpan.Zero)
            remaining = TimeSpan.Zero;

        _ui.SetUiState(
            uid,
            CitationPrinterUiKey.Key,
            new CitationPrinterUiState(remaining));
    }

    private void OnPrint(
        EntityUid uid,
        CitationPrinterComponent component,
        CitationPrinterPrintMessage args)
    {
        if (!CanUse(uid, args.Actor))
            return;

        if (!CitationPrinterConstants.IsValidName(args.RecipientName)
            || !CitationPrinterConstants.IsValidOffense(args.Offense))
        {
            UpdateUi(uid, component);
            return;
        }

        if (_timing.CurTime < component.NextPrintAllowedAfter)
        {
            UpdateUi(uid, component);
            return;
        }

        var issuer = GetIssuerName(args.Actor);
        var content = BuildContent(
            component,
            args.RecipientName.Trim(),
            args.Offense.Trim(),
            issuer);

        var printed = Spawn(
            component.PrintedPrototype,
            Transform(args.Actor).Coordinates);

        if (!TryComp<PaperComponent>(printed, out var paper))
        {
            Log.Error(
                $"Citation printer output {component.PrintedPrototype} "
                + "does not have a PaperComponent.");

            Del(printed);
            UpdateUi(uid, component);
            return;
        }

        _paper.SetContent((printed, paper), content);

        component.NextPrintAllowedAfter =
            _timing.CurTime + component.PrintDelay;

        _hands.PickupOrDrop(args.Actor, printed);
        _audio.PlayPvs(component.PrintSound, uid);

        UpdateUi(uid, component);
    }

    private static string BuildContent(
        CitationPrinterComponent component,
        string recipientName,
        string offense,
        string issuer)
    {
        var heading = FormattedMessage.EscapeText(component.Heading);
        var name = FormattedMessage.EscapeText(recipientName);
        var offenseText = FormattedMessage.EscapeText(offense);
        var issuerText = FormattedMessage.EscapeText(issuer);
        var footer = FormattedMessage.EscapeText(component.Footer);

        return
            $"[mono]{heading}[/mono]\n\n"
            + $"[mono]NAME: {name}[/mono]\n"
            + $"[mono]OFFENSE: {offenseText}[/mono]\n"
            + $"[mono]ISSUING OFFICER: {issuerText}[/mono]\n\n"
            + $"[mono]{footer}[/mono]";
    }
}