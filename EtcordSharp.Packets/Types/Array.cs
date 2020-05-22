namespace EtcordSharp.Packets.Types
{
    public struct Array<T> : IPacketSerializable
    {
        public T[] Elements { get; private set; }
        public int Length { get => Elements.Length; }

        
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
            VarInt length = Elements != null ? Elements.Length : 0;

            int size = length.GetSize();
            for (int i = 0; i < length; i++)
                size += PacketSerializer.GetObjectSize(Elements[i], typeof(T));

            return size;
        }
        public bool Deserialize(byte[] bytes, ref int position)
        {
            VarInt length = new VarInt();
            length.Deserialize(bytes, ref position);

            Elements = new T[length];

            for (int i = 0; i < length; i++)
                if (!PacketSerializer.Deserialize(bytes, ref position, out Elements[i]))
                    return false;

            return true;
        }
        public bool Serialize(byte[] bytes, ref int position)
        {
            VarInt length = Elements != null ? Elements.Length : 0;
            if (!length.Serialize(bytes, ref position)) return false;

            for (int i = 0; i < length; i++)
                if (!PacketSerializer.Serialize(typeof(T), Elements[i], bytes, ref position))
                    return false;

            return true;
        }

        public T this[int index]
        {
            get => Elements[index];
            set => Elements[index] = value;
        }

        public static implicit operator T[](Array<T> a) => a.Elements;
        public static implicit operator Array<T>(T[] a) => new Array<T>(a);
    }
}
