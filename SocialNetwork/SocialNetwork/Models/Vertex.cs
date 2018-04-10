using System.Collections.Generic;
using System.Threading.Tasks;

namespace SocialNetwork.Models
{
    public class Vertex<T> 
    {
       
        public Vertex()
        {
            if (Neighbors == null)
            {
                Neighbors = new List<Vertex<T>>();
            }
        }

        public string VertexValue { get; set; }

        private List<Vertex<T>> Neighbors { get; set; }

        public async Task AddEdge(Vertex<T> vertex)
        {
            var task = new Task(() =>
            {
                if (!Neighbors.Contains(vertex))
                {
                    lock (this)
                    {
                        Neighbors.Add(vertex); 
                    }
                }
            });
            try
            {
                task.Start();
                await task;
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }
    }
}
