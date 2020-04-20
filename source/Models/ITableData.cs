using System.Collections.Generic;
using Terminal.Gui;

namespace myotui.Models
{
    public interface ITableData
    {
        public IListDataSource GetHeaderDataSource();
        public IListDataSource GetBodyDataSource();
        public IDictionary<string, object> this [int key] {get;}
        public IList<int> GetRowIndexesForSearchPhrase(string phrase);
        public void SetHighlight(string highlight);
    }
}