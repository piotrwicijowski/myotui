namespace myotui.Models.Config
{
    public class ColumnDefinition
    {
        private string _displayName;

        public string Name { get; set; }
        public string DisplayName { get; set; }
        public SizeHint Width {get; set;} = new SizeHint();
    }
}