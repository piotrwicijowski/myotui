using System;
using System.Collections.Generic;
using System.Linq;
using myotui.Models.Config;
using Terminal.Gui;

namespace myotui.Models
{
    public class TableData : ITableData
    {
        protected TableDataSource _headerDataSource;
        protected TableDataSource _bodyDataSource;
        protected IList<string> _columnMapOrder;

        public TableData(IList<IDictionary<string, object>> bodyContent, IList<IDictionary<string, object>> headerContent, IList<string> columnMapOrder, IList<SizeHint> columnWidthHints)
        {
            // var headerContent = columnMapOrder.ToDictionary(x => x, x => (object)x);

            // var headerContentList = new List<IDictionary<string,object>>(){headerContent};
            _columnMapOrder = columnMapOrder;
            if(headerContent != null && headerContent.Count != 0 && columnMapOrder.Count != 0)
            { 
                _headerDataSource = new TableDataSource(headerContent, columnMapOrder, columnWidthHints);
            }
            _bodyDataSource = new TableDataSource(bodyContent, columnMapOrder, columnWidthHints);
            var maxColumnWidths = GetMaxColumnContentWidths();
            _headerDataSource?.SetMaxColumnWidths(maxColumnWidths);
            _bodyDataSource?.SetMaxColumnWidths(maxColumnWidths);
        }

        public IDictionary<string, int> GetMaxColumnContentWidths()
        {
            var result = _bodyDataSource.GetMaxColumnContentWidths();
            var headerWidths = _headerDataSource?.GetMaxColumnContentWidths();
            if(headerWidths != null)
            {
                result = result.ToDictionary(kp => kp.Key, kp => 
                {
                    var headerWidth = 0;
                    var headerHasKey = headerWidths.TryGetValue(kp.Key, out headerWidth);
                    return Math.Max(kp.Value,headerWidth);
                });
            }
            return result;
        }

        public IListDataSource GetHeaderDataSource() => _headerDataSource;
        public IListDataSource GetBodyDataSource() => _bodyDataSource;
        public IDictionary<string, object> this [int key]
        {
            get => _bodyDataSource[key];
        }
        public IList<int> GetRowIndexesForSearchPhrase(string phrase)
        {
            var result = new List<int>();
            for(int i = 0; i < _bodyDataSource.Count; i++)
            {
                var rowMatches = _bodyDataSource[i].Where(kv => _columnMapOrder.Contains(kv.Key)).Select(kv => kv.Value).Any(value => value != null && value.ToString().Contains(phrase,StringComparison.CurrentCultureIgnoreCase));
                if(rowMatches)
                {
                    result.Add(i);
                }
            }
            return result;
        }
        public void SetFilter(string filter)
        {
            var tableBodyDataSource = _bodyDataSource as TableDataSource;
            if(tableBodyDataSource != null)
            {
                tableBodyDataSource.Filter = filter;
            }
        }

        public void SetHighlight(string highlight)
        {
            var tableBodyDataSource = _bodyDataSource as TableDataSource;
            if(tableBodyDataSource != null)
            {
                tableBodyDataSource.Highlight = highlight;
            }
        }
    }
}