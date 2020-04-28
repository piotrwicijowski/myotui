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
        private SearchFilterTextField _searchField;
        private SearchFilterTextField _filterField;
        private ITableData _tableData;
        public string SearchPhrase {get; set;} = "";
        public string FilterPhrase {get; set;} = "";
        protected IList<int> _searchResultIndexes {get; set;} = new List<int>();
        public Action<IDictionary<string, object>> FocusedItemChanged;
        protected bool _hasSearch;
        protected bool _hasHeader;
        public TableView(ITableData tableData, bool hasHeader, bool hasSearch)
        {
            _tableData = tableData;
            _hasHeader = hasHeader;
            _hasSearch = hasSearch;
            CreateLayout();
            SetData(tableData);
        }

        public void SetData(ITableData tableData)
        {
            _tableData = tableData;
            _tableListView.Source = _tableData.GetBodyDataSource();
            if(_headerView != null)
            {
                _headerView.Source = _tableData.GetHeaderDataSource();
            }
        }

        public void CreateLayout()
        {
            var headerDataSource = _tableData.GetHeaderDataSource();
            var splitter = new Splitter();

            // _tableListView = new ListView(_tableData.GetBodyDataSource());
            _tableListView = new ListView();
            _tableListView.CanFocus = true;
            _tableListView.X = 0;
            _tableListView.Y = headerDataSource != null ? 2 : 0;
            _tableListView.Width = Dim.Fill();
            _tableListView.Height = Dim.Fill();

            var headerIsEmpty = headerDataSource == null || headerDataSource.Count == 0;
            if(!headerIsEmpty && _hasHeader)
            {
                // _headerView = new ListView(headerDataSource);
                _headerView = new ListView();
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
            if(_hasSearch)
            {
                _searchField = new SearchFilterTextField("/ ");
                _searchField.X = 0;
                _searchField.Y = Pos.Bottom(_tableListView);
                _searchField.Width = Dim.Fill();
                _searchField.Height = 1;
                _searchField.CanFocus = true;
                _searchField.ClearOnEnter = true;
                _searchField.HideOnLeave = true;
                _searchField.EmptyPhraseRepeatsLast = true;
                _searchField.Accepted += (sender, args) =>
                {
                    SetFocus(_tableListView);
                    ApplySearchToTableView();
                };
                _searchField.Aborted += (sender, args) =>
                {
                    SetFocus(_tableListView);
                };
                _searchField.OnLeave += (sender, args) =>
                {
                    HideSearch();
                };
            }
            var hasFilter = _hasSearch;
            if(hasFilter)
            {
                _filterField = new SearchFilterTextField("Y ");
                _filterField.X = 0;
                _filterField.Y = Pos.Bottom(_tableListView);
                _filterField.Width = Dim.Fill();
                _filterField.Height = 1;
                _filterField.CanFocus = true;
                _filterField.ClearOnEnter = false;
                _filterField.HideOnLeave = false;
                _filterField.EmptyPhraseRepeatsLast = false;
                _filterField.Changed += (sender, args) =>
                {
                    ApplyFilterToTableView();
                };
                _filterField.Accepted += (sender, args) =>
                {
                    ApplyFilterToTableView();
                    if(_tableListView.SelectedItem >= _tableListView.Source.Count)
                    {
                        _tableListView.SelectedItem = 0;
                    }
                    SetFocus(_tableListView);

                };
                _filterField.Aborted += (sender, args) =>
                {
                    SetFocus(_tableListView);
                };
                _filterField.OnLeave += (sender, args) =>
                {
                    if(_filterField.HideOnLeave || string.IsNullOrEmpty(FilterPhrase))
                    {
                        HideFilter();
                    }
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
            if(_tableListView.Source.Count > 0)
            {
                _tableListView.SelectedItem = 0;
                if(previousSelected != _tableListView.SelectedItem)
                {
                    TriggerFocusedLineEvent();
                }
                _tableListView.SetNeedsDisplay();

            }
            return true;
        }
        public bool FocusLastLine()
        {
            var previousSelected = _tableListView.SelectedItem;
            if(_tableListView.Source.Count > 0)
            {
                _tableListView.SelectedItem = _tableListView.Source.Count - 1;
                if(previousSelected != _tableListView.SelectedItem)
                {
                    TriggerFocusedLineEvent();
                }
                _tableListView.SetNeedsDisplay();
            }
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
                }
                SetFocus(_searchField);
                Layout();
                return true;
            }
            return false;
        }

        protected void Layout()
        {
            var bottomPadding = 0;
            var hasFilter = false;
            if(_filterField != null && Subviews.Contains(_filterField))
            {
                bottomPadding += 1;
                _filterField.Y = Pos.Bottom(_tableListView);
                hasFilter = true;
            }
            if(_searchField != null && Subviews.Contains(_searchField))
            {
                bottomPadding += 1;
                _searchField.Y = Pos.Bottom(hasFilter ? (View)_filterField : (View)_tableListView);
            }
            _tableListView.Height = Dim.Fill() - bottomPadding;
        }

        public bool HideSearch()
        {
            if(_searchField != null)
            {
                Remove(_searchField);
                Layout();
                // SetNeedsDisplay();
            }
            return true;
        }

        public bool FocusFilter()
        {
            if(_searchField != null)
            {
                if(!Subviews.Contains(_filterField))
                {
                    Add(_filterField);
                }
                Layout();
                SetFocus(_filterField);
                return true;
            }
            return false;
        }

        public bool HideFilter()
        {
            if(_filterField != null)
            {
                Remove(_filterField);
                Layout();
                // SetNeedsDisplay();
            }
            return true;
        }

        public bool SearchAbort() => _searchField.Abort();
        public bool SearchAccept() => _searchField.Accept();
        public bool SearchHistoryNext() => _searchField.HistoryNext();
        public bool SearchHistoryPrev() => _searchField.HistoryPrev();

        public bool FilterAbort() => _filterField.Abort();
        public bool FilterAccept() => _filterField.Accept();
        public bool FilterHistoryNext() => _filterField.HistoryNext();
        public bool FilterHistoryPrev() => _filterField.HistoryPrev();

        private void ApplyFilterToTableView()
        {
            FilterPhrase = _filterField.Phrase;
            _tableData.SetFilter(FilterPhrase);
        }

        private void ApplySearchToTableView()
        {
            SearchPhrase = _searchField.Phrase;
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