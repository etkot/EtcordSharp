using System;

namespace EtcordSharp.Packets.Attributes
{
    [AttributeUsage(AttributeTargets.Enum, AllowMultiple = false, Inherited = false)]
    public class EnumPackageType : Attribute
    {
        public Type type { get; private set; }

        public EnumPackageType(Type type)
        {
            this.type = type;
        }
    }
}
