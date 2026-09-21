using System;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;

namespace Content.Shared._Omu.CitationPrinter;

[RegisterComponent, AutoGenerateComponentPause]
public sealed partial class CitationPrinterComponent : Component
{
    [DataField]
    public EntProtoId PrintedPrototype = "PaperCitation";

    [DataField]
    public TimeSpan PrintDelay = TimeSpan.FromSeconds(5);

    [DataField, AutoPausedField]
    public TimeSpan NextPrintAllowedAfter = TimeSpan.Zero;

    [DataField]
    public SoundSpecifier PrintSound =
        new SoundPathSpecifier("/Audio/Machines/printer.ogg");

    [DataField]
    public string Heading = "SECURITY CITATION";

    [DataField]
    public string Footer =
        "FORMAL NOTICE OF OFFENSE; FURTHER PENALTIES SUBJECT TO OFFICER DISCRETION";
}