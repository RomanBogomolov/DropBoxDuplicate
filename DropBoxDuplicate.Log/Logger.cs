using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;

namespace DropBoxDuplicate.Log
{
    public static class Logger
    {
        public static NLog.Logger ServiceLog = LogManager.GetCurrentClassLogger();
    }
}
