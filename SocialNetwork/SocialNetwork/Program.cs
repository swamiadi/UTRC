using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace SocialNetwork
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var vertexes = new List<Vertex<string>>();
            var networks = VertexHelper.GetNetworks();
            foreach (var vertexitem in VertexHelper.GetVertexs(networks))
            {
                var vertex = new Vertex<string> {VertexValue = vertexitem};
                vertexes.Add(vertex);
            }
            

          
        }


       
    }
}
