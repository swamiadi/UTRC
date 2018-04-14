using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GraphCollection;
using Newtonsoft.Json;
using SocialNetwork.Helpers;
using SocialNetwork.Models;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace SocialNetwork
{
    /// <summary>
    /// 
    /// </summary>
    class Program
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            Console.Write("Please enter the values of A" + Environment.NewLine);
            var personA = Console.ReadLine()?.Trim();
            Console.Write("Please enter the values of B" + Environment.NewLine);
            var personB = Console.ReadLine()?.Trim();

            if (File.Exists(@"Model.txt"))
            {
                var modelfile = System.IO.File.ReadAllText(@"Model.txt");
                var graph = StringToObject(modelfile) as List<GraphNode<string>>;
                Console.Write("Total number of people in the social network : {0}",graph?.Count);
                FindShortestPath(graph, personA, personB);

                Console.ReadLine();
            }
            #region No Model
            else
            {
               
                var networks = VertexHelper.GetNetworks();

                int loopcount = 0;
                var exceptions = new ConcurrentQueue<Exception>();
                var watch = Stopwatch.StartNew();

                var tokenSource = new CancellationTokenSource();
                CancellationToken ct = tokenSource.Token;
                var graphnodes = new List<GraphNode<string>>();

                foreach (var item in VertexHelper.GetVertexs(networks))
                {
                    GraphNode<string> node = new GraphNode<string>(item);
                    graphnodes.Add(node);
                }
                Console.Write("Total number of people in the social network : {0}", graphnodes.Count);
                Task task = Task.Factory.StartNew(delegate
                {
                    // Were we already canceled?
                    ct.ThrowIfCancellationRequested();
                    var loopResult = Parallel.ForEach(graphnodes, new ParallelOptions
                    {
                        MaxDegreeOfParallelism =
                            Environment.ProcessorCount,
                        CancellationToken = new CancellationToken()
                    }, (node) =>
                    {
                        try
                        {

                            Interlocked.Increment(ref loopcount);
                            Console.WriteLine(loopcount);

                            foreach (var item in networks.Where(x => x.PersonA == node.Value))
                            {
                                node.AddNeighbour(graphnodes.FirstOrDefault(x => x.Value == item.PersonB), 1);
                            }


                        }
                        catch (Exception e)
                        {

                            exceptions.Enqueue(e);
                        }

                    });

                }, tokenSource.Token); // Pass same token to StartNew.

                task.ContinueWith(antecendent => SignalCompletion(watch, graphnodes,personA,personB), ct);

            }
            #endregion
            Console.ReadLine();
        }

        private static void SignalCompletion(Stopwatch watch, List<GraphNode<string>> graph, string PersonA, string PersonB)
        {
            try
            {
                var stringvalue = ObjectToString(graph);
                System.IO.File.WriteAllText(@"Model.txt", stringvalue);

                watch.Stop();
                var elapsedms = watch.ElapsedMilliseconds;
                Console.WriteLine(TimeSpan.FromMilliseconds(elapsedms).TotalMinutes);

                FindShortestPath(graph, PersonA, PersonB);

                Console.ReadLine();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        private static void FindShortestPath(List<GraphNode<string>> graph, string from , string to )
        {

            try
            {
                var watch = System.Diagnostics.Stopwatch.StartNew();
                var dijkstra = new Dijkstra<string>(graph);
                var path = dijkstra.FindShortestPathBetween(graph.FirstOrDefault(x => x.Value == from), graph.FirstOrDefault(x => x.Value == to));
                watch.Stop();
                var elapsedms = watch.ElapsedMilliseconds;
                Console.WriteLine("Total Time required to search : {0}",TimeSpan.FromMilliseconds(elapsedms).TotalMinutes);
                Console.WriteLine(Environment.NewLine + "Distance between {0} and {1} is {2}",from,to,path.Count - 1);
                foreach (var item in path)
                {
                    Console.WriteLine(item.Value + Environment.NewLine);
                }

                Console.ReadLine();
            }
            catch (Exception ex)
            {

                throw;
            }
        }


        private static string ObjectToString(object obj)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                new BinaryFormatter().Serialize(ms, obj);
                return Convert.ToBase64String(ms.ToArray());
            }
        }

        private static object StringToObject(string base64String)
        {
            byte[] bytes = Convert.FromBase64String(base64String);
            using (MemoryStream ms = new MemoryStream(bytes, 0, bytes.Length))
            {
                ms.Write(bytes, 0, bytes.Length);
                ms.Position = 0;
                return new BinaryFormatter().Deserialize(ms);
            }
        }
    }
}
