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
    private readonly Label _issuerValue;
    private readonly Label _status;
    private readonly Button _printButton;

    public event Action<string, string>? PrintRequested;
    public event Action? RefreshRequested;

    public CitationPrinterWindow()
    {
        Title = "Citation printer";
        MinSize = new Vector2(480, 300);

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
            PlaceHolder = "Name of the person receiving the citation",
            IsValid = CitationPrinterConstants.IsValidField
        };
        root.AddChild(_nameInput);

        root.AddChild(new Label
        {
            Text = "Offense"
        });

        _offenseInput = new LineEdit
        {
            HorizontalExpand = true,
            PlaceHolder = "Description of the offense",
            IsValid = CitationPrinterConstants.IsValidField
        };
        root.AddChild(_offenseInput);

        root.AddChild(new Label
        {
            Text = "Maximum 128 characters per editable field."
        });

        root.AddChild(new Label
        {
            Text = "Issued by"
        });

        // read only
        _issuerValue = new Label
        {
            Text = string.Empty,
            MinSize = new Vector2(0, 24)
        };
        root.AddChild(_issuerValue);

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

        var refreshButton = new Button
        {
            Text = "Refresh ID"
        };

        var clearButton = new Button
        {
            Text = "Clear"
        };

        _printButton = new Button
        {
            Text = "Print",
            Disabled = true
        };

        buttons.AddChild(refreshButton);
        buttons.AddChild(clearButton);
        buttons.AddChild(_printButton);

        refreshButton.OnPressed += _ => RefreshRequested?.Invoke();

        clearButton.OnPressed += _ =>
        {
            _nameInput.Text = string.Empty;
            _offenseInput.Text = string.Empty;
            _nameInput.GrabKeyboardFocus();
        };

        _printButton.OnPressed += _ =>
        {
            if (!CitationPrinterConstants.IsValidField(_nameInput.Text)
                || !CitationPrinterConstants.IsValidField(_offenseInput.Text))
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

    public void SetIssuer(string issuer)
    {
        _issuerValue.Text = issuer;
    }

    public void SetPrintReady(bool ready)
    {
        _printButton.Disabled = !ready;
        _status.Text = ready ? "Ready." : "Printer cooling down...";
    }
}