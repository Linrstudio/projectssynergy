using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace SynergySequence
{
    public abstract class SequenceManager
    {
        List<Prototype> prototypes = new List<Prototype>();
        public Prototype[] Prototypes { get { return prototypes.ToArray(); } }
        public void AddPrototype(Prototype _Prototype) { prototypes.Add(_Prototype); }

        List<DataType> datatypes = new List<DataType>();
        public DataType[] DataTypes { get { return datatypes.ToArray(); } }
        public void AddDataType(DataType _DataType) { datatypes.Add(_DataType); }

        public SequenceManager()
        {
            if (Application.CurrentCulture != System.Globalization.CultureInfo.GetCultureInfo("en-US"))
            {
                MessageBox.Show("Your system is not running in the \"en-US\" culture!\nloading files from a different culture might not work!");
            }
        }

        public DataType GetDataType(string _DataTypeName)
        {
            foreach (DataType t in datatypes) if (t.Name == _DataTypeName) return t;
            return null;
        }

        public Prototype GetProtoType(string _TypeName)
        {
            foreach (Prototype p in prototypes)
            {
                if (p.BlockType.FullName == _TypeName) return p;
            }
            return null;
        }

        public CodeBlock CreateCodeBlock(string _TypeName) { return CreateCodeBlock(GetProtoType(_TypeName)); }
        public abstract CodeBlock CreateCodeBlock(Prototype _Prototype);

        public class DataType
        {
            public DataType(string _Name, Color _Color)
            {
                Name = _Name;
                Color = _Color;
            }
            public string Name;
            public Color Color;
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
