using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YzProject.Domain.Other
{
    public class SimpleGuidGenerator : IGuidGenerator
    {

        public virtual Guid Create()
        {
            return Guid.NewGuid();
        }
    }
}
