using System;
using System.Numerics;
using Content.Shared._Omu.CitationPrinter;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using static Robust.Client.UserInterface.Controls.BoxContainer;

namespace Content.Client._Omu.CitationPrinter;

public sealed class CitationPrinterWindow : DefaultWindow
{
    private readonly LineEdit _nameInput;
    private readonly LineEdit _offenseInput;
    private readonly Label _status;
    private readonly Button _printButton;

    public event Action<string, string>? PrintRequested;

    public CitationPrinterWindow()
    {
        Title = "Citation printer";
        MinSize = new Vector2(480, 220);

        var root = new BoxContainer
        {
            Orientation = LayoutOrientation.Vertical,
            SeparationOverride = 8,
            Margin = new Thickness(8)
        };

        ContentsContainer.AddChild(root);

        root.AddChild(new Label
        {
            Text = "Name"
        });

        _nameInput = new LineEdit
        {
            HorizontalExpand = true,
            PlaceHolder = "Clowny McHonkface",
            IsValid = CitationPrinterConstants.IsValidName
        };
        root.AddChild(_nameInput);

        root.AddChild(new Label
        {
            Text = "Offense"
        });

        _offenseInput = new LineEdit
        {
            HorizontalExpand = true,
            PlaceHolder = "Littering",
            IsValid = CitationPrinterConstants.IsValidOffense
        };
        root.AddChild(_offenseInput);

        _status = new Label
        {
            Text = "Reading printer state..."
        };
        root.AddChild(_status);

        var buttons = new BoxContainer
        {
            Orientation = LayoutOrientation.Horizontal,
            SeparationOverride = 8
        };
        root.AddChild(buttons);

        var clearButton = new Button
        {
            Text = "Clear"
        };

        _printButton = new Button
        {
            Text = "Print",
            Disabled = true
        };

        buttons.AddChild(clearButton);
        buttons.AddChild(_printButton);

        clearButton.OnPressed += _ =>
        {
            _nameInput.Text = string.Empty;
            _offenseInput.Text = string.Empty;
            _nameInput.GrabKeyboardFocus();
        };

        _printButton.OnPressed += _ =>
        {
            if (!CitationPrinterConstants.IsValidName(_nameInput.Text)
                || !CitationPrinterConstants.IsValidOffense(_offenseInput.Text))
            {
                return;
            }

            _printButton.Disabled = true;
            _status.Text = "Printing...";

            PrintRequested?.Invoke(
                _nameInput.Text,
                _offenseInput.Text);
        };
    }

    public void SetPrintReady(bool ready)
    {
        _printButton.Disabled = !ready;
        _status.Text = ready ? "Ready" : "Cooling down...";
    }
}