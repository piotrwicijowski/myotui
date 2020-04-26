using System;
using System.Collections.Generic;
using System.Linq;
using Terminal.Gui;

namespace myotui.Views
{
    public class HorizontalListView : View
    {
        public Action<IDictionary<string, object>> FocusedItemChanged;
        
        protected IList<IDictionary<string, object>> _data;
        private View _lastFocused;
        public HorizontalListView(IList<IDictionary<string, object>> data)
        {
            _data = data;
            CreateViewElements();
            OnEnter += (sender, args) =>
            {
                if(_lastFocused == null && Subviews != null && Subviews.Count > 0)
                {
                    _lastFocused = Subviews[0];
                }
                SetFocus(_lastFocused);
            };
        }

        private void CreateViewElements()
        {
            View previousElement = null;
            foreach(var element in _data)
            {
                var viewElement = new Button(element["DisplayName"].ToString());
                viewElement.OnEnter += (sender, args) =>
                {
                    TriggerFocusedColumnEvent();
                };
                viewElement.X = previousElement == null ? 0 : Pos.Right(previousElement);
                Add(viewElement);
                previousElement = viewElement;
            }
        }

        public void TriggerFocusedColumnEvent()
        {
            if (FocusedItemChanged != null)
            {
                var focusedElementIndex = Subviews.IndexOf(Focused);
                if(focusedElementIndex >= 0 && focusedElementIndex < _data.Count)
                {
                    FocusedItemChanged(_data[focusedElementIndex]);
                }
            }
        }

        public bool FocusNextColumn()
        {
            var currentIndex = Subviews.IndexOf(Focused);
            var subsequentIndex = currentIndex + 1;
            if(subsequentIndex >= 0 && subsequentIndex < _data.Count)
            {
                _lastFocused = Subviews[subsequentIndex];
                SetFocus(Subviews[subsequentIndex]);
                return true;
            }
            return false;

        }

        public bool FocusPrevColumn()
        {
            var currentIndex = Subviews.IndexOf(Focused);
            var subsequentIndex = currentIndex - 1;
            if(subsequentIndex >= 0 && subsequentIndex < _data.Count)
            {
                _lastFocused = Subviews[subsequentIndex];
                SetFocus(Subviews[subsequentIndex]);
                return true;
            }
            return false;
        }
        public bool FocusFirstColumn()
        {
            _lastFocused = Subviews.FirstOrDefault();
            SetFocus(_lastFocused);
            return true;
        }
        public bool FocusLastColumn()
        {
            _lastFocused = Subviews.LastOrDefault();
            SetFocus(_lastFocused);
            return true;
        }
    }
}