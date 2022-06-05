using GeometryUtils.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeometryUtils.Model
{
    public class Edge : IEdge
    {
        public IPoint P { get; set; }

        public IPoint Q { get; set; }

        public int Index { get; set; }
        
        public Edge(int index, IPoint p, IPoint q)
        {
            Index = index;
            P = p;
            Q = q;
        }
    }
}
