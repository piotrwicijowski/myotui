using System;
using System.Collections.Generic;
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
        protected readonly IList<string> _previousSearches = new List<string>();
        protected int _previousSearchesIndex = -1;
        public string SearchPhrase {get; protected set;}

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
                _searchField.Text = "";
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

        public bool SearchAbort()
        {
            _previousSearchesIndex = -1;
            SearchAborted?.Invoke(this, new EventArgs());
            return true;
        }

        public bool SearchAccept()
        {
            if(!string.IsNullOrEmpty(_searchField.Text.ToString()))
            {
                SearchPhrase = _searchField.Text.ToString();
                _previousSearches.Insert(0,SearchPhrase);
                _previousSearchesIndex = -1;
            }
            SearchAccepted?.Invoke(this, new EventArgs());
            return true;
        }

        public bool SearchHistoryPrev()
        {
            _previousSearchesIndex = Helpers.Clamp(_previousSearchesIndex + 1, 0, _previousSearches.Count - 1);
            _searchField.Text = _previousSearches[_previousSearchesIndex];
            _searchField.CursorPosition = _searchField.Text.Length;
            return true;
        }

        public bool SearchHistoryNext()
        {
            _previousSearchesIndex = Helpers.Clamp(_previousSearchesIndex - 1, -1, _previousSearches.Count - 1);
            if(_previousSearchesIndex >= 0)
            {
                _searchField.Text = _previousSearches[_previousSearchesIndex];
            }
            else
            {
                _searchField.Text = string.Empty;
            }
            _searchField.CursorPosition = _searchField.Text.Length;
            return true;
        }
    }
}