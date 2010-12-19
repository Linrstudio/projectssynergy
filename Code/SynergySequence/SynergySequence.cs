using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SynergySequence
{
    public class SynergySequence
    {
        static List<Prototype> prototypes = new List<Prototype>();
        public static void AddPrototype(Prototype _Prototype) { prototypes.Add(_Prototype); }
        public static Prototype[] Prototypes { get { return prototypes.ToArray(); } }

        public SynergySequence()
        {

        }

        public class Prototype
        {
            public Prototype(string _Name, string _GroupName, string _Description, Type _BlockType)
            {
                Name = _Name; GroupName = _GroupName; Description = _Description; BlockType = _BlockType; UserCanAdd = true;
            }
            public Prototype(string _Name, string _GroupName, string _Description, Type _BlockType, bool _UserCanAdd)
            {
                Name = _Name; GroupName = _GroupName; Description = _Description; BlockType = _BlockType; UserCanAdd = _UserCanAdd;
            }
            public string Name;
            public string GroupName;
            public string Description;
            public Type BlockType;
            public bool UserCanAdd;
            public object Create() { return Activator.CreateInstance(null, BlockType); }
        }
    }
}
