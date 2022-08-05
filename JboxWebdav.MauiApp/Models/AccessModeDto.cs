using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JboxWebdav.MauiApp.Models
{
    public class AccessModeDto
    {
        public AccessModeDto(string display, AccessModeEnum type, int accessCode)
        {
            Display = display;
            Type = type;
            AccessCode = accessCode;
        }

        public string Display { get; set; }
        public AccessModeEnum Type { get; set; }
        public int AccessCode { get; set; }
    }

    public enum AccessModeEnum
    {
        Full, ReadOnly, ReadWrite, NoDelete
    }
}
