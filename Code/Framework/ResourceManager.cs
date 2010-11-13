using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Synergy
{
    public abstract class Resource
    {
        public float LoadingProgress = 0;
        public enum ResourceStatus
        {
            Pending, Loaded, Loading, Failed
        };
        string resourcename;
        public string ResourceName { get { return resourcename; } }
        protected ResourceStatus status = ResourceStatus.Pending;
        public ResourceStatus Status { get { return status; } }
        public Resource(string _ResourceName)
        {
            resourcename = _ResourceName;
            ResourceManager.AddResource(this);
        }
        /// <summary>
        /// this method allows the resource to load a bit of itself if needed, for example read a packet from a network stream
        /// </summary>
        public abstract void Update();
        public abstract object Get();
    }

    public static class ResourceManager
    {
        public static Resource Get(string _ResourceName)
        {
            if (Resources.ContainsKey(_ResourceName))
            {
                Resource res = Resources[_ResourceName];
                if (res != null)
                    return res;
            }
            return null;
        }

        public static void Update()
        {
            foreach (Resource r in Resources.Values)
            {
                r.Update();
            }
        }
        public static void AddResource(Resource _Resource)
        {
            if (Resources.ContainsKey(_Resource.ResourceName)) return;
            Resources.Add(_Resource.ResourceName, _Resource);
        }
        static Dictionary<string, Resource> Resources = new Dictionary<string, Resource>();
    }
}
