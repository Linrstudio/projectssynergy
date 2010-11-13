#if false
using System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.CSharp;
using Microsoft.VisualBasic;
using Synergy;

namespace Synergy
{
    public class Domain
    {
        string name;
        public string Name { get { return name; } }
        AppDomain domain;
        public List<Assembly> Assemblies = new List<Assembly>();
        public Domain(string _Name)
        {
            name = _Name;
            AppDomainSetup setup = new AppDomainSetup();
            setup.ApplicationBase = Environment.CurrentDirectory;
            setup.DisallowBindingRedirects = false;

            setup.ConfigurationFile = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;


            domain = AppDomain.CreateDomain(Name, null, setup);
        }

        public void AddAssembly(System.Reflection.Assembly _Assembly)
        {
            //domain.Load();
            Assemblies.Add(new Assembly(this, _Assembly));
        }

        public object CreateInstance(string _AssemblyName, string _InstanceName, params object[] _Parameters)
        {
            return domain.CreateInstanceFrom(_AssemblyName, _InstanceName, _Parameters);
        }

        ~Domain()
        {
            if (domain != null)
            {
                AppDomain.Unload(domain);
                Log.Write("Domain Manager", "Domain {0} Unloaded", Name);
            }
        }

        public class Assembly
        {
            public Assembly(Domain _Domain, System.Reflection.Assembly _Assembly)
            {
                domain = _Domain;
                assembly = _Assembly;
            }

            Domain domain;
            System.Reflection.Assembly assembly;
            public object CreateInstance(string _InstanceName, params object[] _Parameters)
            {
                return null;// Domain.CreateInstance(assembly.FullName, _InstanceName, _Parameters);
            }
        }
    }
}
#endif