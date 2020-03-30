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
            _headerView = new ListView(_tableData.GetHeaderDataSource());
            _headerView.X = 0;
            _headerView.Y = 0;
            _headerView.Width = Dim.Fill();
            _headerView.Height = 1;

            var splitter = new Splitter();
            splitter.X = 0;
            splitter.Y = 1;
            splitter.Width = Dim.Fill();
            splitter.Height = 1;

            _tableListView = new ListView(_tableData.GetBodyDataSource());
            _tableListView.X = 0;
            _tableListView.Y = 2;
            _tableListView.Width = Dim.Fill();
            _tableListView.Height = Dim.Fill();

            Add(_headerView);
            Add(splitter);
            Add(_tableListView);
        }

    }
}