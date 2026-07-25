using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fincore.Application.DTO.MasterTable
{
    public class UpdateDocumentTypeDto
    {
        public string? DocumentCategory { get; set; }

        public bool IsActive { get; set; }

        public int ModifiedBy { get; set; }
    }
}
