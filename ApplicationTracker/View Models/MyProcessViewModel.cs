using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ApplicationTracker.Models;

namespace ApplicationTracker.View_Models
{
    internal class MyProcessViewModel
    {

        public MyProcessViewModel(ObservableCollection<MyProcess> myProcessList)
        {
            MyProcess = myProcessList;
        }

        public ObservableCollection<MyProcess> MyProcess;
    }
}
