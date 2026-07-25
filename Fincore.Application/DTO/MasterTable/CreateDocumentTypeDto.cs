using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fincore.Application.DTO.MasterTable
{
    public class CreateDocumentTypeDto
    {
        public string? DocumentCategory { get; set; }

        public bool IsActive { get; set; }

        public int CreatedBy { get; set; }
    }
}
