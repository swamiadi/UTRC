using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SocialNetwork.Helpers;
using SocialNetwork.Models;


namespace SocialNetwork
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            try
            {
                var vertexes = new ConcurrentBag<Vertex<string>>();
                var networks = VertexHelper.GetNetworks();
                foreach (var vertexitem in VertexHelper.GetVertexs(networks))
                {
                    var vertex = new Vertex<string> { VertexValue = vertexitem };
                    vertexes.Add(vertex);
                }
             



                int loopcount = 0;
                var exceptions = new ConcurrentQueue<Exception>();
                var watch = System.Diagnostics.Stopwatch.StartNew();

                var tokenSource = new CancellationTokenSource();
                CancellationToken ct = tokenSource.Token;

                Task task = Task.Factory.StartNew(delegate
                {
                    // Were we already canceled?
                    ct.ThrowIfCancellationRequested();
                    var loopResult = Parallel.ForEach(networks, new ParallelOptions
                    {
                        MaxDegreeOfParallelism =
                            Environment.ProcessorCount,
                        CancellationToken = new CancellationToken()
                    }, (networkitem) =>
                    {
                        try
                        {
                            Interlocked.Increment(ref loopcount);
                            Console.WriteLine(loopcount);
                            var vertex = vertexes.FirstOrDefault(x => x.VertexValue == networkitem.PersonA);
                            vertex?.AddEdge(vertexes.FirstOrDefault(x => x.VertexValue == networkitem.PersonB));
                        }
                        catch (Exception e)
                        {

                            exceptions.Enqueue(e);
                        }

                    });

                }, tokenSource.Token); // Pass same token to StartNew.

                task.ContinueWith(antecendent => SignalCompletion(watch), ct);


                if (exceptions.Count > 0) throw new AggregateException(exceptions);


                Console.ReadLine();
                //foreach (var networkitem in networks)
                //{

                //}
            }
            catch (Exception ex)
            {

                throw ex;
            }


          
        }


        private static void SignalCompletion(Stopwatch watch)
        {
            double elapseds = 0;

            watch.Stop();
            elapseds = watch.ElapsedMilliseconds * 0.001;
            Console.WriteLine(elapseds);
            Console.ReadLine();
        }
    }
}
