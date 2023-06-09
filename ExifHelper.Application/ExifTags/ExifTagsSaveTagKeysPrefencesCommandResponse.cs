using ExifHelper.Application.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExifHelper.Application.ExifTags
{
    public class ExifTagsSaveTagKeysPrefencesCommandResponse : IExifCommandResponse
    {
        public string FilePath { get; set; }
    }
}
