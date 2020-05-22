using System;

namespace EtcordSharp.Packets.Types
{
    public struct String : IPacketSerializable
    {
        public string Value { get; private set; }


        public int GetSize()
        {
            VarInt size = Value != null ? Value.Length : 0;
            return size.GetSize() + size;
        }
        public bool Deserialize(byte[] bytes, ref int position)
        {
            VarInt size = new VarInt();
            size.Deserialize(bytes, ref position);

            Value = "";
            for (int i = 0; i < size; i++)
                Value += (char)bytes[position++];

            return true;
        }
        public bool Serialize(byte[] bytes, ref int position)
        {
            VarInt size = Value != null ? Value.Length : 0;
            if (!size.Serialize(bytes, ref position)) return false;

            for (int i = 0; i < size; i++)
                bytes[position++] = (byte)Value[i];

            return true;
        }

        public static implicit operator string(String s) => s.Value;
        public static implicit operator String(string s) => new String { Value = s };
    }
}
