using Terminal.Gui;

namespace myotui.Models
{
    public interface ITableData
    {
        public IListDataSource GetHeaderDataSource();
        public IListDataSource GetBodyDataSource();
    }
}