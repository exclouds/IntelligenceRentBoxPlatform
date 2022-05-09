namespace Admin.Application.Custom.API.PublicArea.Combox.Dto
{
    public class ComboxStringDto
    {
        public ComboxStringDto(string value, string displayText)
        {
            Value = value;
            DisplayText = displayText;
        }
        //
        // 摘要:
        //     Value of the item.
        public string Value { get; set; }
        //
        // 摘要:
        //     Display text of the item.
        public string DisplayText { get; set; }
    }
}