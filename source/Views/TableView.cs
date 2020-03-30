using myotui.Models;
using Terminal.Gui;

namespace myotui.Views
{
    public class TableView : View
    {
        private ListView _tableListView;
        private ListView _headerView;

        private readonly ITableData _tableData;
        
        public TableView(ITableData tableData)
        {
            _tableData = tableData;
            _tableListView = new ListView(_tableData.GetBodyDataSource());
            _headerView = new ListView(_tableData.GetHeaderDataSource());
            Add(_headerView);
            Add(_headerView);
        }

    }
}