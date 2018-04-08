using SocialNetwork.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialNetwork
{
    public static class VertexHelper
    {
        internal static List<string> GetVertexs(List<Network> networks)
        {
            var distinctvertex = networks.Select(x => x.PersonA).Distinct().Concat(networks.Select(x=>x.PersonB).Distinct()).Distinct().ToList();
           
            return distinctvertex;
        }

        internal static List<Network> GetNetworks()
        {
            var text = System.IO.File.ReadAllText(@"../../SocialNetwork.txt");
            var lines = text.Split(
                                new[] { Environment.NewLine },
                                StringSplitOptions.None);

            return lines.Select(line => line.Replace("\"", "").Split(','))
                .Select(network => new Network
                {
                    PersonA = network[0],
                    PersonB = network[1]
                })
                .ToList();

        }
    }
}
