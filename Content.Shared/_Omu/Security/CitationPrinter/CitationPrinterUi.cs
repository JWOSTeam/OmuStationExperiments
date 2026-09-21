using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._Omu.CitationPrinter;

public static class CitationPrinterConstants
{
    public const int MaximumFieldLength = 128;

    public static bool IsValidField(string? text)
    {
        if (text == null || text.Length > MaximumFieldLength)
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

    public CitationPrinterPrintMessage(string recipientName, string offense)
    {
        RecipientName = recipientName;
        Offense = offense;
    }
}

[Serializable, NetSerializable]
public sealed class CitationPrinterRefreshMessage : BoundUserInterfaceMessage
{
}

[Serializable, NetSerializable]
public sealed class CitationPrinterUiState : BoundUserInterfaceState
{
    public readonly string IssuerName;
    public readonly TimeSpan RemainingCooldown;

    public CitationPrinterUiState(
        string issuerName,
        TimeSpan remainingCooldown)
    {
        IssuerName = issuerName;
        RemainingCooldown = remainingCooldown;
    }
}