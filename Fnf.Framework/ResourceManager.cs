using OpenTK.Input;
using System;
using System.Collections.Generic;

namespace Fnf.Framework
{
    /// <summary>
    /// Holds onto <seealso cref="ResourcePool"/>s and provides helpfull methods to improve resouce managment
    /// </summary>
    public static class ResourceManager
    {
        public static Dictionary<string, IResourcePool> Pools = new Dictionary<string, IResourcePool>();

        /// <summary>
        /// Makes a new pool if it doesn't exists
        /// </summary>
        public static void MakeNewPool<Tkey,Tval>(string name)
        {
            if (Pools.ContainsKey(name)) return;
            Pools.Add(name, new ResourcePool<Tkey,Tval>(name));
        }

        /// <summary>
        /// Returns the resource pool with the correct type
        /// </summary>
        public static ResourcePool<Tkey, Tval> GetResourcePool<Tkey, Tval>(string name)
        {
            return Pools[name] as ResourcePool<Tkey, Tval>;
        }
    }

    public class ResourcePool<Tkey,Tval> : IResourcePool
    {
        public Dictionary<Tkey, Tval> entries;
        public string name;

        public ResourcePool(string name)
        {
            entries = new Dictionary<Tkey, Tval>();
            this.name = name;
        }
    }

    public interface IResourcePool
    {
    }
}