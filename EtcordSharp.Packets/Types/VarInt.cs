using System;

namespace EtcordSharp.Packets.Types
{
    public struct VarInt : IPacketSerializable
    {
        public int Value { get; private set; }


        public int GetSize()
        {
            int size = 0;
            uint val = (uint)Value;
            do
            {
                byte temp = (byte)(val & 0b01111111);
                val >>= 7;
                if (val != 0)
                {
                    temp |= 0b10000000;
                }
                size++;
            } while (val != 0);

            return size;
        }
        public void Deserialize(byte[] bytes, ref int position)
        {
            int numRead = 0;
            Value = 0;
            byte read;
            do
            {
                read = bytes[position++];
                int val = (read & 0b01111111);
                Value |= (val << (7 * numRead));
                numRead++;
                if (numRead > 5)
                {
                    Console.WriteLine("Error: VarInt is too big");
                    return;
                }
            } while ((read & 0b10000000) != 0);
        }
        public void Serialize(byte[] bytes, ref int position)
        {
            uint val = (uint)Value;
            do
            {
                byte temp = (byte)(val & 0b01111111);
                val >>= 7;
                if (val != 0)
                {
                    temp |= 0b10000000;
                }
                bytes[position++] = temp;
            } while (val != 0);
        }

        public static implicit operator int(VarInt v) => v.Value;
        public static implicit operator VarInt(int i) => new VarInt { Value = i };
    }
}
