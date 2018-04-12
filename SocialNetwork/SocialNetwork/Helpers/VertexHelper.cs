using System;
using System.Collections.Concurrent;
using System.Linq;
using SocialNetwork.Models;

namespace SocialNetwork.Helpers
{
    public static class VertexHelper
    {
        internal static ConcurrentBag<string> GetVertexs(ConcurrentBag<Network> networks)
        {
            var distinctvertex = networks.Select(x => x.PersonA).Distinct().Concat(networks.Select(x=>x.PersonB).Distinct()).Distinct().ToList();
            //var distinctvertex = networks.Select(x => x.PersonA).Distinct().ToList();

            return new ConcurrentBag<string>(distinctvertex);
        }

        internal static ConcurrentBag<Network> GetNetworks()
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
    }
}
