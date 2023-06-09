using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExifHelper.Application.Configuration
{
    public interface IConfigurationManager
    {
        string ExifApplicationPath { get; set; }
        string CopyImagePath { get; set; }
    }
}
