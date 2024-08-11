using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyExchange
{
    public class Root
    {
        public Rate rates { get; set; }
        public long timestamp;
        public string license;
    }
}
