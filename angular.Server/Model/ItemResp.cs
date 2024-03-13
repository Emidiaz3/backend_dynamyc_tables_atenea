using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiRestCuestionario.Model
{
    public class ItemResp
    {
        public ItemResp()
        {

        }
        public int status { get; set; }
        public string message { get; set; }
        public object data { get; set; }
    }
    public class ItemResplistobject
    {
        public ItemResplistobject()
        {

        }
        public int status { get; set; }
        public string message { get; set; }
        public List<object> data { get; set; }
    }
}
