using System.Collections.Generic;
using System.Linq;
using Terminal.Gui;

namespace myotui.Models
{
    public class TableData : ITableData
    {
        protected TableDataSource _headerDataSource;
        protected TableDataSource _bodyDataSource;
        public TableData(IList<IDictionary<string, object>> contentList, IList<string> columnMapOrder, IList<double> columnPorportions)
        {
            var headerContent = columnMapOrder.ToDictionary(x => x, x => (object)x);
            var headerContentList = new List<IDictionary<string,object>>(){headerContent};
            _headerDataSource = new TableDataSource(headerContentList, columnMapOrder, columnPorportions);
            _bodyDataSource = new TableDataSource(contentList, columnMapOrder, columnPorportions);
        }

        public IListDataSource GetHeaderDataSource() => _headerDataSource;
        public IListDataSource GetBodyDataSource() => _bodyDataSource;
    }
}