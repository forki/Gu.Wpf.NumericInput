﻿namespace Gu.Wpf.NumericInput.UITests.DoubleBox
{
    using System.Collections.Generic;
    using Gu.Wpf.NumericInput.Demo;
    using NUnit.Framework;
    using TestStack.White.UIItems;
    using TestStack.White.WindowsAPI;

    public class SpinnersTests : WindowTests
    {
        private static readonly IReadOnlyList<string> BoxContainerIds = new[]
        {
            AutomationIds.VanillaGroupBox,
            AutomationIds.DataTemplateGroupBox,
            AutomationIds.ControlTemplate
        };

        protected override string WindowName { get; } = "SpinnerWindow";

        private GroupBox DoubleBoxGroupBox => this.Window.Get<GroupBox>(AutomationIds.DoubleBoxGroupBox);

        private CheckBox AllowSpinnersBox => this.DoubleBoxGroupBox.Get<CheckBox>(AutomationIds.AllowSpinnersBox);

        private TextBox DigitsBox => this.DoubleBoxGroupBox.Get<TextBox>(AutomationIds.DigitsBox);

        private TextBox VmValueBox => this.DoubleBoxGroupBox.Get<TextBox>(AutomationIds.VmValueBox);

        private TextBox IncrementBox => this.DoubleBoxGroupBox.Get<TextBox>(AutomationIds.IncrementBox);

        private TextBox MinBox => this.DoubleBoxGroupBox.Get<TextBox>(AutomationIds.MinBox);

        private TextBox MaxBox => this.DoubleBoxGroupBox.Get<TextBox>(AutomationIds.MaxBox);

        [SetUp]
        public void SetUp()
        {
            this.DigitsBox.Enter("");
            this.MinBox.Enter("");
            this.MaxBox.Enter("");
            this.IncrementBox.Enter("1");
            this.VmValueBox.Enter("0");
            this.AllowSpinnersBox.Checked = false;
        }

        [TestCaseSource(nameof(BoxContainerIds))]
        public void UpdatesViewModel(string containerId)
        {
            this.AllowSpinnersBox.Checked = true;
            this.DigitsBox.Enter("1");
            var container = this.DoubleBoxGroupBox.Get<UIItemContainer>(containerId);
            var inputBox = container.Get<TextBox>(AutomationIds.InputBox);
            var increaseButton = container.Get<Button>(SpinnerDecorator.IncreaseButtonName);
            var decreaseButton = container.Get<Button>(SpinnerDecorator.DecreaseButtonName);
            var vmValueBox = this.VmValueBox;
            inputBox.Enter("1.23");
            vmValueBox.Click();
            Assert.AreEqual("1.23", inputBox.EditText());
            Assert.AreEqual("1.2", inputBox.FormattedText());
            Assert.AreEqual("1.23", vmValueBox.Text);
            Assert.AreEqual("1.23", inputBox.Value());
            Assert.AreEqual(TextSource.UserInput, inputBox.TextSource());
            Assert.AreEqual(Status.Idle, inputBox.Status());

            Assert.AreEqual(true, increaseButton.Enabled);
            Assert.AreEqual(true, decreaseButton.Enabled);
            increaseButton.Click();
            Assert.AreEqual("2.23", inputBox.EditText());
            Assert.AreEqual("2.2", inputBox.FormattedText());
            Assert.AreEqual("1.23", vmValueBox.Text);
            Assert.AreEqual(false, inputBox.HasValidationError());
            vmValueBox.Click();

            Assert.AreEqual("2.23", inputBox.EditText());
            Assert.AreEqual("2.2", inputBox.FormattedText());
            Assert.AreEqual("2.23", vmValueBox.Text);
            Assert.AreEqual("2.23", inputBox.Value());
            Assert.AreEqual(TextSource.UserInput, inputBox.TextSource());
            Assert.AreEqual(Status.Idle, inputBox.Status());

            this.IncrementBox.Enter("5");
            vmValueBox.Click();
            increaseButton.Click();
            vmValueBox.Click();
            Assert.AreEqual("7.23", inputBox.EditText());
            Assert.AreEqual("7.2", inputBox.FormattedText());
            Assert.AreEqual(false, inputBox.HasValidationError());
            Assert.AreEqual("7.23", vmValueBox.Text);
            Assert.AreEqual("7.23", inputBox.Value());
            Assert.AreEqual(TextSource.UserInput, inputBox.TextSource());
            Assert.AreEqual(Status.Idle, inputBox.Status());

            decreaseButton.Click();
            vmValueBox.Click();
            Assert.AreEqual("2.23", inputBox.EditText());
            Assert.AreEqual("2.2", inputBox.FormattedText());
            Assert.AreEqual(false, inputBox.HasValidationError());
            Assert.AreEqual("2.23", vmValueBox.Text);
            Assert.AreEqual("2.23", inputBox.Value());
            Assert.AreEqual(TextSource.UserInput, inputBox.TextSource());
            Assert.AreEqual(Status.Idle, inputBox.Status());
        }

        [TestCaseSource(nameof(BoxContainerIds))]
        public void TruncatesToMax(string containerId)
        {
            var groupBox = this.Window.Get<GroupBox>(AutomationIds.DoubleBoxGroupBox);
            this.AllowSpinnersBox.Checked = true;
            this.IncrementBox.Enter("5");
            this.MaxBox.Enter("3");
            var container = groupBox.Get<UIItemContainer>(containerId);
            var inputBox = container.Get<TextBox>(AutomationIds.InputBox);
            var increaseButton = container.Get<Button>(SpinnerDecorator.IncreaseButtonName);
            var decreaseButton = container.Get<Button>(SpinnerDecorator.DecreaseButtonName);
            var vmValueBox = this.VmValueBox;
            vmValueBox.Click();
            Assert.AreEqual("0", inputBox.EditText());
            Assert.AreEqual("0", inputBox.FormattedText());
            Assert.AreEqual("0", vmValueBox.Text);
            Assert.AreEqual("0", inputBox.Value());
            Assert.AreEqual(TextSource.ValueBinding, inputBox.TextSource());
            Assert.AreEqual(Status.Idle, inputBox.Status());

            Assert.AreEqual(true, increaseButton.Enabled);
            Assert.AreEqual(true, decreaseButton.Enabled);
            increaseButton.Click();
            vmValueBox.Click();
            Assert.AreEqual("3", inputBox.EditText());
            Assert.AreEqual("3", inputBox.FormattedText());
            Assert.AreEqual(false, inputBox.HasValidationError());
            Assert.AreEqual("3", vmValueBox.Text);
            Assert.AreEqual("3", inputBox.Value());
            Assert.AreEqual(TextSource.UserInput, inputBox.TextSource());
            Assert.AreEqual(Status.Idle, inputBox.Status());

            Assert.AreEqual(false, increaseButton.Enabled);
            Assert.AreEqual(true, decreaseButton.Enabled);
        }

        [TestCaseSource(nameof(BoxContainerIds))]
        public void TruncatesToMin(string containerId)
        {
            var groupBox = this.Window.Get<GroupBox>(AutomationIds.DoubleBoxGroupBox);
            this.AllowSpinnersBox.Checked = true;
            this.IncrementBox.Enter("5");
            this.MinBox.Enter("-3");
            var container = groupBox.Get<UIItemContainer>(containerId);
            var inputBox = container.Get<TextBox>(AutomationIds.InputBox);
            var increaseButton = container.Get<Button>(SpinnerDecorator.IncreaseButtonName);
            var decreaseButton = container.Get<Button>(SpinnerDecorator.DecreaseButtonName);
            var vmValueBox = this.VmValueBox;
            vmValueBox.Click();
            Assert.AreEqual("0", inputBox.EditText());
            Assert.AreEqual("0", inputBox.FormattedText());
            Assert.AreEqual("0", vmValueBox.Text);
            Assert.AreEqual("0", inputBox.Value());
            Assert.AreEqual(TextSource.ValueBinding, inputBox.TextSource());
            Assert.AreEqual(Status.Idle, inputBox.Status());

            Assert.AreEqual(true, increaseButton.Enabled);
            Assert.AreEqual(true, decreaseButton.Enabled);
            decreaseButton.Click();
            vmValueBox.Click();
            Assert.AreEqual("-3", inputBox.EditText());
            Assert.AreEqual("-3", inputBox.FormattedText());
            Assert.AreEqual(false, inputBox.HasValidationError());
            Assert.AreEqual("-3", vmValueBox.Text);
            Assert.AreEqual("-3", inputBox.Value());
            Assert.AreEqual(TextSource.UserInput, inputBox.TextSource());
            Assert.AreEqual(Status.Idle, inputBox.Status());

            Assert.AreEqual(true, increaseButton.Enabled);
            Assert.AreEqual(false, decreaseButton.Enabled);
        }

        [TestCaseSource(nameof(BoxContainerIds))]
        public void DecreasesWhenGreaterThanMax(string containerId)
        {
            var groupBox = this.Window.Get<GroupBox>(AutomationIds.DoubleBoxGroupBox);
            this.AllowSpinnersBox.Checked = true;
            this.MaxBox.Enter("3");
            var container = groupBox.Get<UIItemContainer>(containerId);
            var inputBox = container.Get<TextBox>(AutomationIds.InputBox);
            var increaseButton = container.Get<Button>(SpinnerDecorator.IncreaseButtonName);
            var decreaseButton = container.Get<Button>(SpinnerDecorator.DecreaseButtonName);
            var vmValueBox = this.VmValueBox;
            vmValueBox.Click();
            inputBox.Enter("5");
            vmValueBox.Click();
            Assert.AreEqual("5", inputBox.EditText());
            Assert.AreEqual("5", inputBox.FormattedText());
            Assert.AreEqual(true, inputBox.HasValidationError());
            Assert.AreEqual("0", vmValueBox.Text);
            Assert.AreEqual("0", inputBox.Value());
            Assert.AreEqual(TextSource.UserInput, inputBox.TextSource());
            Assert.AreEqual(Status.Idle, inputBox.Status());

            Assert.AreEqual(false, increaseButton.Enabled);
            Assert.AreEqual(true, decreaseButton.Enabled);
            decreaseButton.Click();
            Assert.AreEqual("4", inputBox.EditText());
            Assert.AreEqual("4", inputBox.FormattedText());
            Assert.AreEqual(true, inputBox.HasValidationError());
            Assert.AreEqual("0", vmValueBox.Text);
            Assert.AreEqual("0", inputBox.Value());
            Assert.AreEqual(TextSource.UserInput, inputBox.TextSource());
            Assert.AreEqual(Status.Idle, inputBox.Status());

            Assert.AreEqual(false, increaseButton.Enabled);
            Assert.AreEqual(true, decreaseButton.Enabled);

            decreaseButton.Click();
            Assert.AreEqual("3", inputBox.EditText());
            Assert.AreEqual("3", inputBox.FormattedText());
            Assert.AreEqual(false, inputBox.HasValidationError());
            Assert.AreEqual("0", vmValueBox.Text);
            Assert.AreEqual("3", inputBox.Value());
            Assert.AreEqual(TextSource.UserInput, inputBox.TextSource());
            Assert.AreEqual(Status.Idle, inputBox.Status());
        }

        [TestCaseSource(nameof(BoxContainerIds))]
        public void IncreasesWhenLessThanMin(string containerId)
        {
            var groupBox = this.Window.Get<GroupBox>(AutomationIds.DoubleBoxGroupBox);
            this.AllowSpinnersBox.Checked = true;
            this.MinBox.Enter("-3");
            var container = groupBox.Get<UIItemContainer>(containerId);
            var inputBox = container.Get<TextBox>(AutomationIds.InputBox);
            var increaseButton = container.Get<Button>(SpinnerDecorator.IncreaseButtonName);
            var decreaseButton = container.Get<Button>(SpinnerDecorator.DecreaseButtonName);
            var vmValueBox = this.VmValueBox;
            vmValueBox.Click();
            inputBox.Enter("-5");
            vmValueBox.Click();
            Assert.AreEqual("-5", inputBox.EditText());
            Assert.AreEqual("-5", inputBox.FormattedText());
            Assert.AreEqual(true, inputBox.HasValidationError());
            Assert.AreEqual("0", vmValueBox.Text);
            Assert.AreEqual("0", inputBox.Value());
            Assert.AreEqual(TextSource.UserInput, inputBox.TextSource());
            Assert.AreEqual(Status.Idle, inputBox.Status());
            Assert.AreEqual(true, increaseButton.Enabled);
            Assert.AreEqual(false, decreaseButton.Enabled);

            increaseButton.Click();
            Assert.AreEqual("-4", inputBox.EditText());
            Assert.AreEqual("-4", inputBox.FormattedText());
            Assert.AreEqual(true, inputBox.HasValidationError());
            Assert.AreEqual("0", vmValueBox.Text);
            Assert.AreEqual("0", inputBox.Value());
            Assert.AreEqual(TextSource.UserInput, inputBox.TextSource());
            Assert.AreEqual(Status.Idle, inputBox.Status());
            Assert.AreEqual(true, increaseButton.Enabled);
            Assert.AreEqual(false, decreaseButton.Enabled);

            increaseButton.Click();
            Assert.AreEqual("-3", inputBox.EditText());
            Assert.AreEqual("-3", inputBox.FormattedText());
            Assert.AreEqual(false, inputBox.HasValidationError());
            Assert.AreEqual("0", vmValueBox.Text);
            Assert.AreEqual("-3", inputBox.Value());
            Assert.AreEqual(TextSource.UserInput, inputBox.TextSource());
            Assert.AreEqual(Status.Idle, inputBox.Status());
        }

        [TestCaseSource(nameof(BoxContainerIds))]
        public void Undo(string containerId)
        {
            var groupBox = this.Window.Get<GroupBox>(AutomationIds.DoubleBoxGroupBox);
            this.AllowSpinnersBox.Checked = true;
            var container = groupBox.Get<UIItemContainer>(containerId);
            var inputBox = container.Get<TextBox>(AutomationIds.InputBox);
            var increaseButton = container.Get<Button>(SpinnerDecorator.IncreaseButtonName);
            ////var decreaseButton = container.Get<Button>(SpinnerDecorator.DecreaseButtonName);
            var vmValueBox = this.VmValueBox;
            Assert.AreEqual("0", inputBox.EditText());
            Assert.AreEqual("0", inputBox.FormattedText());
            increaseButton.Click();
            Assert.AreEqual("1", inputBox.EditText());
            Assert.AreEqual("1", inputBox.FormattedText());
            Assert.AreEqual(false, inputBox.HasValidationError());
            Assert.AreEqual("0", vmValueBox.Text);
            Assert.AreEqual("1", inputBox.Value());
            Assert.AreEqual(TextSource.UserInput, inputBox.TextSource());
            Assert.AreEqual(Status.Idle, inputBox.Status());

            inputBox.Click();
            var keyboard = this.Window.Keyboard;
            keyboard.HoldKey(KeyboardInput.SpecialKeys.CONTROL);
            keyboard.Enter("z");
            keyboard.LeaveKey(KeyboardInput.SpecialKeys.CONTROL);
            vmValueBox.Click();
            Assert.AreEqual("0", inputBox.EditText());
            Assert.AreEqual("0", inputBox.FormattedText());
            Assert.AreEqual(false, inputBox.HasValidationError());
            Assert.AreEqual("0", vmValueBox.Text);
            Assert.AreEqual("0", inputBox.Value());
            Assert.AreEqual(TextSource.UserInput, inputBox.TextSource());
            Assert.AreEqual(Status.Idle, inputBox.Status());
        }
    }
}