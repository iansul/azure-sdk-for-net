// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

namespace Microsoft.Azure.Management.Fluent.Network
{
    using Management.Network.Models;
    using System.Collections.Generic;
    using Resource.Core;
    using Management.Network;
    using System.Threading.Tasks;

    /// <summary>
    /// Implementation for Network
    /// </summary>
    public partial class NetworkImpl :
        GroupableParentResource<INetwork,
            VirtualNetworkInner,
            Rest.Azure.Resource,
            NetworkImpl,
            NetworkManager,
            Network.Definition.IWithGroup,
            Network.Definition.IWithCreate,
            Network.Definition.IWithCreate,
            Network.Update.IUpdate>,
        INetwork,
        Network.Definition.IDefinition,
        Network.Update.IUpdate
    {
        private IVirtualNetworksOperations innerCollection;
        private IDictionary<string, ISubnet> subnets;
        internal NetworkImpl(
            string name,
            VirtualNetworkInner innerModel,
            IVirtualNetworksOperations innerCollection,
            NetworkManager networkManager) : base(name, innerModel, networkManager)
        {
            this.innerCollection = innerCollection;
        }

        override protected void InitializeChildrenFromInner()
        {
            subnets = new SortedDictionary<string, ISubnet>();
            IList<SubnetInner> inners = Inner.Subnets;
            if (inners != null)
            {
                foreach (var inner in inners)
                {
                    SubnetImpl subnet = new SubnetImpl(inner, this);
                    subnets[inner.Name] = subnet;
                }
            }
        }

        public override INetwork Refresh()
        {
            var response = innerCollection.Get(ResourceGroupName, Name);
            SetInner(response);
            return this;
        }

        internal NetworkImpl WithSubnet(SubnetImpl subnet)
        {
            if (subnet != null)
            {
                subnets[subnet.Name] = subnet;
            }
            return this;
        }

        public NetworkImpl WithDnsServer(string ipAddress)
        {
            if (Inner.DhcpOptions == null)
                Inner.DhcpOptions = new DhcpOptions();

            if (Inner.DhcpOptions.DnsServers == null)
                Inner.DhcpOptions.DnsServers = new List<string>();

            Inner.DhcpOptions.DnsServers.Add(ipAddress);
            return this;
        }

        public NetworkImpl WithSubnet(string name, string cidr)
        {
            return DefineSubnet(name)
                .WithAddressPrefix(cidr)
                .Attach();
        }

        public NetworkImpl WithSubnets(IDictionary<string, string> nameCidrPairs)
        {
            subnets.Clear();
            foreach (var pair in nameCidrPairs)
            {
                WithSubnet(pair.Key, pair.Value);
            }
            return this;
        }

        public NetworkImpl WithoutSubnet(string name)
        {
            subnets.Remove(name);
            return this;
        }

        public NetworkImpl WithAddressSpace(string cidr)
        {
            if (Inner.AddressSpace == null)
                Inner.AddressSpace = new AddressSpace();

            if (Inner.AddressSpace.AddressPrefixes == null)
                Inner.AddressSpace.AddressPrefixes = new List<string>();

            Inner.AddressSpace.AddressPrefixes.Add(cidr);
            return this;
        }

        public SubnetImpl DefineSubnet(string name)
        {
            SubnetInner inner = new SubnetInner(name: name);
            return new SubnetImpl(inner, this);
        }

        public IList<string> AddressSpaces
        {
            get
            {
                if (Inner.AddressSpace == null)
                {
                    return new List<string>();
                }
                else if (Inner.AddressSpace.AddressPrefixes == null)
                {
                    return new List<string>();
                }
                else
                {
                    return Inner.AddressSpace.AddressPrefixes;
                }
            }
        }

        public IList<string> DnsServerIps
        {
            get
            {
                if (Inner.DhcpOptions == null)
                {
                    return new List<string>();
                }
                else if (Inner.DhcpOptions.DnsServers == null)
                {
                    return new List<string>();
                }
                else
                {
                    return Inner.DhcpOptions.DnsServers;
                }
            }
        }

        public IDictionary<string, ISubnet> Subnets()
        {
            return subnets;
        }

        override protected void BeforeCreating()
        {
            // Ensure address spaces
            if (AddressSpaces.Count == 0)
            {
                WithAddressSpace("10.0.0.0/16"); // Default address space
            }

            if (IsInCreateMode)
            {
                // Create a subnet as needed, covering the entire first address space
                if (subnets.Count == 0)
                {
                    WithSubnet("subnet1", AddressSpaces[0]);
                }
            }

            // Reset and update subnets
            Inner.Subnets = InnersFromWrappers<SubnetInner, ISubnet>(subnets.Values);
        }

        override protected void AfterCreating()
        {
            InitializeChildrenFromInner();
        }

        public SubnetImpl UpdateSubnet(string name)
        {
            ISubnet subnet;
            subnets.TryGetValue(name, out subnet);
            return (SubnetImpl)subnet;
        }

        override protected Task<VirtualNetworkInner> CreateInner()
        {
            return innerCollection.CreateOrUpdateAsync(ResourceGroupName, Name, Inner);
        }
    }
}