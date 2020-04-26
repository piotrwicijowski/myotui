using System;
using System.Collections.Generic;
using System.Linq;
using myotui.Models;
using Terminal.Gui;

namespace myotui.Views
{
    public class TableView : View
    {
        private ListView _tableListView;
        private ListView _headerView;
        private SearchTextField _searchField;
        private readonly ITableData _tableData;
        public string SearchPhrase {get; set;} = "";
        protected IList<int> _searchResultIndexes {get; set;} = new List<int>();
        public Action<IDictionary<string, object>> FocusedItemChanged;
        public TableView(ITableData tableData, bool hasHeader, bool hasSearch)
        {
            _tableData = tableData;

            var headerDataSource = _tableData.GetHeaderDataSource();
            var splitter = new Splitter();

            _tableListView = new ListView(_tableData.GetBodyDataSource());
            _tableListView.X = 0;
            _tableListView.Y = headerDataSource != null ? 2 : 0;
            _tableListView.Width = Dim.Fill();
            _tableListView.Height = Dim.Fill();

            var headerIsEmpty = headerDataSource == null || headerDataSource.Count == 0;
            if(!headerIsEmpty && hasHeader)
            {
                _headerView = new ListView(headerDataSource);
                _headerView.CanFocus = false;
                _headerView.X = 0;
                _headerView.Y = 0;
                _headerView.Width = Dim.Fill();
                _headerView.Height = 1;

                splitter.CanFocus = false;
                splitter.X = 0;
                splitter.Y = 1;
                splitter.Width = Dim.Fill();
                splitter.Height = 1;

                Add(_headerView);
                Add(splitter);
            }
            _tableListView.SelectedChanged += () =>
            {
                TriggerFocusedLineEvent();
            };
            Add(_tableListView);
            if(hasSearch)
            {
                _searchField = new SearchTextField();
                _searchField.X = 0;
                _searchField.Y = Pos.Bottom(_tableListView);
                _searchField.Width = Dim.Fill();
                _searchField.Height = 1;
                _searchField.CanFocus = true;
                _searchField.SearchAccepted += (sender, args) =>
                {
                    SetFocus(_tableListView);
                    ApplySearchToTableView();
                };
                _searchField.SearchAborted += (sender, args) =>
                {
                    SetFocus(_tableListView);
                };
                _searchField.OnLeave += (sender, args) =>
                {
                    HideSearch();
                };
            }
        }

        public bool FocusNextLine()
        {
            return _tableListView.MoveDown();
        }

        public void TriggerFocusedLineEvent()
        {
            if (FocusedItemChanged != null)
            {
                var selectedItemIndex = _tableListView.SelectedItem;
                FocusedItemChanged(_tableData[selectedItemIndex]);
            }
        }

        public bool FocusPrevLine()
        {
            return _tableListView.MoveUp();
        }

        public bool FocusFirstLine()
        {
            var previousSelected = _tableListView.SelectedItem;
            _tableListView.SelectedItem = 0;
            if(previousSelected != _tableListView.SelectedItem)
            {
                TriggerFocusedLineEvent();
            }
            _tableListView.SetNeedsDisplay();
            return true;
        }
        public bool FocusLastLine()
        {
            var previousSelected = _tableListView.SelectedItem;
            _tableListView.SelectedItem = _tableListView.Source.Count - 1;
            if(previousSelected != _tableListView.SelectedItem)
            {
                TriggerFocusedLineEvent();
            }
            _tableListView.SetNeedsDisplay();
            return true;
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
        }

        public bool FocusSearch()
        {
            if(_searchField != null)
            {
                if(!Subviews.Contains(_searchField))
                {
                    Add(_searchField);
                    _tableListView.Height = Dim.Fill() - 1;
                }
                SetFocus(_searchField);
                return true;
            }
            return false;
        }

        public bool HideSearch()
        {
            if(_searchField != null)
            {
                Remove(_searchField);
                _tableListView.Height = Dim.Fill();
                SetNeedsDisplay();
            }
            return true;
        }

        public bool SearchAbort() => _searchField.SearchAbort();
        public bool SearchAccept() => _searchField.SearchAccept();
        public bool SearchHistoryNext() => _searchField.SearchHistoryNext();
        public bool SearchHistoryPrev() => _searchField.SearchHistoryPrev();

        private void ApplySearchToTableView()
        {
            SearchPhrase = _searchField.SearchPhrase;
            _tableData.SetHighlight(SearchPhrase);
            FocusNextSearch();
        }

        public bool FocusNextSearch()
        {
            var currentIndex = _tableListView.SelectedItem;
            _searchResultIndexes = _tableData.GetRowIndexesForSearchPhrase(SearchPhrase);
            var nextSearchIndexes = _searchResultIndexes.Where(index => index > currentIndex);
            if(nextSearchIndexes.Count() > 0)
            {
                _tableListView.SelectedItem = nextSearchIndexes.FirstOrDefault();
                if(currentIndex != _tableListView.SelectedItem)
                {
                    TriggerFocusedLineEvent();
                }
                _tableListView.SetNeedsDisplay();
                return true;
            }
            return false;
        }

        public bool FocusPrevSearch()
        {
            var currentIndex = _tableListView.SelectedItem;
            _searchResultIndexes = _tableData.GetRowIndexesForSearchPhrase(SearchPhrase);
            var nextSearchIndexes = _searchResultIndexes.Where(index => index < currentIndex);
            if(nextSearchIndexes.Count() > 0)
            {
                _tableListView.SelectedItem = nextSearchIndexes.LastOrDefault();
                if(currentIndex != _tableListView.SelectedItem)
                {
                    TriggerFocusedLineEvent();
                }
                _tableListView.SetNeedsDisplay();
                return true;
            }
            return false;
        }

    }
}