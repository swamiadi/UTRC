using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialNetwork
{
    public class Vertex<T> 
    {
        public string VertexValue { get; set; }

        List<Vertex<T>> Neighbors { get; set; }

        public void AddEdge(Vertex<T> vertex)
        {
            Neighbors.Add(vertex);
        }
    }
}
