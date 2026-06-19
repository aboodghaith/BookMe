using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class Category : BaseModel
    {
      

        public string Name { get; set; }

        public string? Description { get; set; }

        public string? ImagePath { get; set; }

        public ICollection<Service>? Services { get; set; }
    }
}
