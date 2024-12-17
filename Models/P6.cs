using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestItemTemplate.Models
{
    public partial class P6 : ObservableObject
    {
        public List<string> getInfoCommandDataBMS = new List<string>()
        {
            "02 01 00 00 00 00 00 00",
            "02 02 00 00 00 00 00 00",
            "02 03 00 00 00 00 00 00",
            "02 04 00 00 00 00 00 00",
        };


    }
}
