namespace myotui.Services
{
    public class ModeService : IModeService
    {
        public string DefaultMode {get; set;}
        public string CurrentMode {get; set;}
        public bool BindingMatchesCurrentMode(string bindingMode)
        {
            return CurrentMode == bindingMode || bindingMode == "*";
        }
    }
}