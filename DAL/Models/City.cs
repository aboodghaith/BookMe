using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class City : BaseModel
    {


        public string Name { get; set; }


        public ICollection<Service>? Services { get; set; }
    }
}
