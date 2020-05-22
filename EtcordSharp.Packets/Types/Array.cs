namespace EtcordSharp.Packets.Types
{
    public struct Array<T> : IPacketSerializable
    {
        public T[] Elements { get; private set; }
        public int Length { get { return Elements.Length; } }


        public Array(int length)
        {
            Elements = new T[length];
        }
        public Array(T[] elements)
        {
            Elements = elements;
        }

        public int GetSize()
        {
            VarInt elementsLength = Elements.Length;

            int size = elementsLength.GetSize();
            for (int i = 0; i < Elements.Length; i++)
            {
                size += PacketSerializer.GetObjectSize(Elements[i], typeof(T));
            }

            return size;
        }
        public bool Deserialize(byte[] bytes, ref int position)
        {
            VarInt size = new VarInt();
            size.Deserialize(bytes, ref position);

            Elements = new T[size];

            for (int i = 0; i < size; i++)
                if (!PacketSerializer.Deserialize(bytes, ref position, out Elements[i]))
                    return false;

            return true;
        }
        public bool Serialize(byte[] bytes, ref int position)
        {
            VarInt size = Elements.Length;
            if (!size.Serialize(bytes, ref position)) return false;

            for (int i = 0; i < size; i++)
                if (!PacketSerializer.Serialize(typeof(T), Elements[i], bytes, ref position))
                    return false;

            return true;
        }

        public T this[int index]
        {
            get { return Elements[index]; }
            set { Elements[index] = value; }
        }
    }
}
