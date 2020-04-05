using System.Collections.Generic;
using System.Linq;
using Terminal.Gui;

namespace myotui.Models
{
    public class TableData : ITableData
    {
        protected TableDataSource _headerDataSource;
        protected TableDataSource _bodyDataSource;
        public TableData(IList<IDictionary<string, object>> bodyContent, IList<IDictionary<string, object>> headerContent, IList<string> columnMapOrder, IList<double> columnPorportions)
        {
            // var headerContent = columnMapOrder.ToDictionary(x => x, x => (object)x);
            // var headerContentList = new List<IDictionary<string,object>>(){headerContent};
            if(headerContent.Count != 0 && columnMapOrder.Count != 0)
            { 
                _headerDataSource = new TableDataSource(headerContent, columnMapOrder, columnPorportions);
            }
            _bodyDataSource = new TableDataSource(bodyContent, columnMapOrder, columnPorportions);
        }

        public IListDataSource GetHeaderDataSource() => _headerDataSource;
        public IListDataSource GetBodyDataSource() => _bodyDataSource;
        public IDictionary<string, object> this [int key]
        {
            get => _bodyDataSource[key];
        }
    }
}