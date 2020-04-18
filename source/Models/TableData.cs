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

        public TableData(IList<IDictionary<string, object>> bodyContent, IList<IDictionary<string, object>> headerContent, IList<string> columnMapOrder, IList<SizeHint> columnWidths)
        {
            // var headerContent = columnMapOrder.ToDictionary(x => x, x => (object)x);
            // var headerContentList = new List<IDictionary<string,object>>(){headerContent};
            if(headerContent.Count != 0 && columnMapOrder.Count != 0)
            { 
                _headerDataSource = new TableDataSource(headerContent, columnMapOrder, columnWidths);
            }
            _bodyDataSource = new TableDataSource(bodyContent, columnMapOrder, columnWidths);
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
    }
}