namespace myotui.Models.Config
{
    public class ColumnDefinition
    {
        private string _displayName;

        public string Name { get; set; }
        public string DisplayName { get => _displayName ?? Name; set => _displayName = value; }
    }
}