using ApplicationTracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationTracker.Utilities
{
    public class ViewModelUtils
    {
        //
        public static IEnumerable<ProcessWrapper> ConvertProcDataToProcWrapper(IEnumerable<ProcessData> unconverted)
        {
            var converted = new List<ProcessWrapper>();

            foreach (var proc in unconverted)
            {
                converted.Add(new ProcessWrapper { ProcessName = proc.ProcessName, ProcessTime = proc.ProcessTime, ProcessDate = proc.ProcessDate });
            }

            return converted;
        }
    }
}
