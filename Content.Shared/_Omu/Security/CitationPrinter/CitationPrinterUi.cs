using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._Omu.CitationPrinter;

public static class CitationPrinterConstants
{
    public const int MaximumNameLength = 64;
    public const int MaximumOffenseLength = 96;
    public const int MaximumNotesLength = 64;

    public static bool IsValidName(string? text) =>
        IsValidField(text, MaximumNameLength);

    public static bool IsValidOffense(string? text) =>
        IsValidField(text, MaximumOffenseLength);

    public static bool IsValidNotes(string? text) =>
        IsValidField(text, MaximumNotesLength);

    private static bool IsValidField(string? text, int maximumLength)
    {
        if (text == null || text.Length > maximumLength)
            return false;

        foreach (var character in text)
        {
            if (char.IsControl(character))
                return false;
        }

        return true;
    }
}

[Serializable, NetSerializable]
public enum CitationPrinterUiKey : byte
{
    Key
}

[Serializable, NetSerializable]
public sealed class CitationPrinterPrintMessage : BoundUserInterfaceMessage
{
    public readonly string RecipientName;
    public readonly string Offense;
    public readonly string Notes;

    public CitationPrinterPrintMessage(
        string recipientName,
        string offense,
        string notes)
    {
        RecipientName = recipientName;
        Offense = offense;
        Notes = notes;
    }
}

[Serializable, NetSerializable]
public sealed class CitationPrinterUiState : BoundUserInterfaceState
{
    public readonly TimeSpan RemainingCooldown;

    public CitationPrinterUiState(TimeSpan remainingCooldown)
    {
        RemainingCooldown = remainingCooldown;
    }
}