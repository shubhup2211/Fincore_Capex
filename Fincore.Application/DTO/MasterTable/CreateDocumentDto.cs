using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fincore.Application.DTO.MasterTable
{
    public class CreateDocumentDto
    {
        public int DocumentTypeId { get; set; }
        public int UserId { get; set; }
        public int? EntityId { get; set; }
        public int? MasterTypeId { get; set; }

        public string FileName { get; set; }
        public string FileType { get; set; }
        public string FilePath { get; set; }
         public DateTime? CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
    }
}
