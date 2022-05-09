using System.Collections.Generic;

namespace Admin.Application.Custom.API.PublicArea.Combox.Dto
{
    /// <summary>
    /// 国家下拉输出参数
    /// </summary>
    public class PublicComboBoxDto
    {
        public List<ComboxStringDto> Comboxs { get; set; }

        public PublicComboBoxDto()
        {
            Comboxs = new List<ComboxStringDto>();
        }
    }
}