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
    /// Program class
    /// </summary>
    class Program
    {
        /// <summary>
        /// Main method
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            try
            {
                Console.Write("Please enter the values of A" + Environment.NewLine);
                var personA = Console.ReadLine()?.Trim();
                Console.Write("Please enter the values of B" + Environment.NewLine);
                var personB = Console.ReadLine()?.Trim();


                if (File.Exists(@"Model.txt"))
                {
                    var modelfile = System.IO.File.ReadAllText(@"Model.txt");
                    var graph = StringToObject(modelfile) as ConcurrentBag<GraphNode<string>>;
                    Console.Write("Total number of people in the social network : {0}", graph?.Count);
                    FindShortestPath(graph, personA, personB);

                    Console.ReadLine();
                }
                #region No Model
                else
                {
                    var networks = VertexHelper.GetNetworks();
                    var graphnodes = VertexHelper.GetGraphNodes(VertexHelper.GetVertexs(networks));
                    Console.Write("Total number of people in the social network : {0}", graphnodes.Count);
                    FormNodes(graphnodes, networks, personA, personB);
                }
                #endregion
                Console.ReadLine();
            }
            catch (Exception ex)
            {

                Console.Write(ex.Message + "Please try again...");
                Console.ReadLine();
            }
        }

        /// <summary>
        /// FOrm Nodes
        /// </summary>
        /// <param name="graphnodes"></param>
        /// <param name="networks"></param>
        /// <param name="personA"></param>
        /// <param name="personB"></param>
        private static void FormNodes(ConcurrentBag<GraphNode<string>> graphnodes, ConcurrentBag<Network> networks, string personA, string personB)
        {
            var watch = Stopwatch.StartNew();
            int loopcount = 0;
            var tokenSource = new CancellationTokenSource();
            CancellationToken ct = tokenSource.Token;
            var exceptions = new ConcurrentQueue<Exception>();
            var spinner = new Spinner(0, 5);

            spinner.Start();
            Task task = Task.Factory.StartNew(delegate
            {
                //Cancelled 
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
                        //Console.WriteLine(loopcount);

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

            task.ContinueWith(antecendent => Completion(watch, graphnodes, personA, personB, spinner), ct);
        }

        /// <summary>
        /// Completion event
        /// </summary>
        /// <param name="watch"></param>
        /// <param name="graph"></param>
        /// <param name="personA"></param>
        /// <param name="personB"></param>
        /// <param name="spinner"></param>
        private static void Completion(Stopwatch watch, ConcurrentBag<GraphNode<string>> graph, string personA, string personB, Spinner spinner)
        {
            try
            {
                spinner.Stop();
                var stringvalue = ObjectToString(graph);
                System.IO.File.WriteAllText(@"Model.txt", stringvalue);

                watch.Stop();
                var elapsedms = watch.ElapsedMilliseconds;
                Console.WriteLine(TimeSpan.FromMilliseconds(elapsedms).TotalMinutes);

                FindShortestPath(graph, personA, personB);

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
        private static void FindShortestPath(ConcurrentBag<GraphNode<string>> graph, string from, string to)
        {

            try
            {
                var spinner = new Spinner(0, 5);

                spinner.Start();
                var watch = System.Diagnostics.Stopwatch.StartNew();
                var dijkstra = new Dijkstra<string>(graph);
                var path = dijkstra.FindShortestPathBetween(graph.FirstOrDefault(x => x.Value == from), graph.FirstOrDefault(x => x.Value == to));
                spinner.Stop();
                watch.Stop();
                var elapsedms = watch.ElapsedMilliseconds;
                Console.WriteLine("Total Time required to search : {0}", TimeSpan.FromMilliseconds(elapsedms).TotalMinutes);
                Console.WriteLine(Environment.NewLine + "Distance between {0} and {1} is {2}", from, to, path.Count - 1);
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

        /// <summary>
        /// Object to string conversion
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private static string ObjectToString(object obj)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                new BinaryFormatter().Serialize(ms, obj);
                return Convert.ToBase64String(ms.ToArray());
            }
        }

        /// <summary>
        /// String to object conversion
        /// </summary>
        /// <param name="base64String"></param>
        /// <returns></returns>
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
