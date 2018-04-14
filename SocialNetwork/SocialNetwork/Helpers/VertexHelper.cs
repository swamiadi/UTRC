using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using GraphCollection;
using SocialNetwork.Models;

namespace SocialNetwork.Helpers
{
    public static class VertexHelper
    {
        public static ConcurrentBag<string> GetVertexs(ConcurrentBag<Network> networks)
        {
            var distinctvertex = networks.Select(x => x.PersonA).Distinct().Concat(networks.Select(x=>x.PersonB).Distinct()).Distinct().ToList();
            //var distinctvertex = networks.Select(x => x.PersonA).Distinct().ToList();

            return new ConcurrentBag<string>(distinctvertex);
        }

        public static ConcurrentBag<Network> GetNetworks()
        {
            var text = System.IO.File.ReadAllText(@"../../SocialNetwork.txt");
            var lines = text.Split(
                                new[] { Environment.NewLine },
                                StringSplitOptions.None);

            var networks = lines.Select(line => line.Replace("\"", "").Split(','))
                .Select(network => new Network
                {
                    PersonA = network[0],
                    PersonB = network[1]
                });

            return new ConcurrentBag<Network>(networks);

        }


        public static ConcurrentBag<GraphNode<string>> GetGraphNodes(ConcurrentBag<string> vertex)
        {
            ConcurrentBag<GraphNode<string>> graphnodes = new ConcurrentBag<GraphNode<string>>();
            foreach (var item in vertex)
            {
                GraphNode<string> node = new GraphNode<string>(item);
                graphnodes.Add(node);
            }

            return graphnodes;
        }
    }
}
