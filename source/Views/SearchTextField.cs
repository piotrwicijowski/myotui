using System;
using NStack;
using Terminal.Gui;

namespace myotui.Views
{
    public class SearchTextField : View
    {
        private TextField _searchField;
        private Label _searchPlaceholder;
        public event EventHandler SearchAborted;
        public event EventHandler SearchAccepted;

        public SearchTextField()
        {
            _searchPlaceholder = new Label("");
            _searchPlaceholder.X = 0;
            _searchPlaceholder.Y = 0;
            _searchPlaceholder.Width = Dim.Fill();
            _searchPlaceholder.Height = 1;

            _searchField = new TextField("");
            _searchField.X = 0;
            _searchField.Y = 0;
            _searchField.Width = Dim.Fill();
            _searchField.Height = 1;
            Add(_searchPlaceholder);
            OnEnter += (sender, args) =>
            {
                RemoveAll();
                Add(_searchField);
                SetFocus(_searchField);
            };
            OnLeave += (sender, args) =>
            {
                if(sender == this)
                {
                    RemoveAll();
                    Add(_searchPlaceholder);
                    CanFocus = true;
                }
            };
        }

        public new ustring Text {
            get => _searchField.Text;
            set => _searchField.Text = value;
        }

        public override bool ProcessKey(KeyEvent keyEvent)
        {
            switch (keyEvent.Key)
            {
                case Key.Esc:
                    _searchField.Text = "";
                    SearchAborted?.Invoke(this, new EventArgs());
                    return true;
                case Key.ControlM:
                    SearchAccepted?.Invoke(this, new EventArgs());
                    return true;
                default:
                    return base.ProcessKey (keyEvent);
            }
        }

        
    }
}