using System;
using System.Collections.Generic;
using NStack;
using Terminal.Gui;

namespace myotui.Views
{
    public class SearchFilterTextField : View
    {
        private TextField _textField;
        private Label _placeholder;
        public event EventHandler Aborted;
        public event EventHandler Accepted;
		public event EventHandler<ustring> Changed;
        protected readonly IList<string> _previousPhrases = new List<string>();
        protected int _previousPhrasesIndex = -1;
        public string Phrase {get; protected set;} = "";

        public bool ClearOnEnter {get; set;}
        public bool HideOnLeave {get; set;}
        public bool EmptyPhraseRepeatsLast {get; set;}

        public SearchFilterTextField()
        {
            _placeholder = new Label("");
            _placeholder.X = 0;
            _placeholder.Y = 0;
            _placeholder.Width = Dim.Fill();
            _placeholder.Height = 1;

            _textField = new TextField("");
            _textField.X = 0;
            _textField.Y = 0;
            _textField.Width = Dim.Fill();
            _textField.Height = 1;
            Add(_placeholder);
            OnEnter += (sender, args) =>
            {
                RemoveAll();
                Add(_textField);
                if(ClearOnEnter)
                {
                    _textField.Text = "";
                }
                else
                {
                    _textField.Text = Phrase;
                }
                SetFocus(_textField);
            };
            OnLeave += (sender, args) =>
            {
                if(sender == this)
                {
                    if(HideOnLeave)
                    {
                        RemoveAll();
                        Add(_placeholder);
                    }
                    CanFocus = true;
                }
            };
            _textField.Changed += (sender, args) =>
            {
                Phrase = _textField.Text.ToString();
                Changed?.Invoke(this,args);
            };
        }

        public bool Abort()
        {
            _previousPhrasesIndex = -1;
            Aborted?.Invoke(this, new EventArgs());
            return true;
        }

        public bool Accept()
        {
            if(EmptyPhraseRepeatsLast && _previousPhrases.Count > 0 && string.IsNullOrEmpty(_textField.Text.ToString()))
            {
                Phrase = _previousPhrases[0];
            }
            else
            {
                Phrase = _textField.Text.ToString();
                _previousPhrases.Insert(0,Phrase);
            }
            _previousPhrasesIndex = -1;
            Accepted?.Invoke(this, new EventArgs());
            return true;
        }

        public bool HistoryPrev()
        {
            _previousPhrasesIndex = Helpers.Clamp(_previousPhrasesIndex + 1, 0, _previousPhrases.Count - 1);
            _textField.Text = _previousPhrases[_previousPhrasesIndex];
            _textField.CursorPosition = _textField.Text.Length;
            return true;
        }

        public bool HistoryNext()
        {
            _previousPhrasesIndex = Helpers.Clamp(_previousPhrasesIndex - 1, -1, _previousPhrases.Count - 1);
            if(_previousPhrasesIndex >= 0)
            {
                _textField.Text = _previousPhrases[_previousPhrasesIndex];
            }
            else
            {
                _textField.Text = string.Empty;
            }
            _textField.CursorPosition = _textField.Text.Length;
            return true;
        }
    }
}