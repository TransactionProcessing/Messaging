using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessagingService.Models
{
    [ExcludeFromCodeCoverage]
    public class EmailAttachment
    {
        #region Properties
        
        public String FileData { get; set; }

        public String Filename { get; set; }

        public FileType FileType { get; set; }

        #endregion
    }
    
    public enum FileType
    {
        None = 0,

        PDF
    }
}
