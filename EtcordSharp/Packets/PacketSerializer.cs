using EtcordSharp.Packets.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace EtcordSharp.Packets
{
    public static class PacketSerializer
    {
        private static Dictionary<PacketType, MethodInfo> methods;

        public static void Initialize()
        {
            Console.WriteLine("Initialize");
        }
        static PacketSerializer()
        {
            methods = new Dictionary<PacketType, MethodInfo>();

            AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => t.Namespace != null && t.Namespace.Contains("EtcordSharp"))
                .SelectMany(t => t.GetMethods())
                .ToList()
                .ForEach(m => CheckMethod(m));
        }
        private static void CheckMethod(MethodInfo method)
        {
            PacketReceiver attr = (PacketReceiver)method.GetCustomAttribute(typeof(PacketReceiver));
            if (attr != null)
            {
                if (!methods.ContainsKey(attr.MessageType))
                    methods.Add(attr.MessageType, method);
                else
                    Console.WriteLine("Error: Multiple receiver methods for packet \"" + attr.MessageType.ToString() + "\" (" + methods[attr.MessageType].Name + " and " + method.Name + ")");
            }
        }

        public static byte[] ReceivePacket(object recipient, byte[] data, int offset, out bool isResponseReliable)
        {
            isResponseReliable = true;
            int position = offset;

            // Get the packet type
            object packetType;
            if (!Deserialize(typeof(PacketType), data, ref position, out packetType))
            {
                Console.WriteLine("Error: PacketType is fucked up");
                return null;
            }

            // Find the method for this packet type 
            MethodInfo method;
            if (methods.TryGetValue((PacketType)packetType, out method))
            {
                // Method return object
                object ret;

                // Check if the method requires a parameter
                ParameterInfo[] parameters = method.GetParameters();
                if (parameters.Length > 0)
                {
                    Type structure = parameters[0].ParameterType;
                    FieldInfo[] fields = structure.GetFields();
                    object obj = Activator.CreateInstance(structure);
                
                    // Deserialize structure fields
                    for (int i = 0; i < fields.Length; i++)
                    {
                        object value;
                        if (!Deserialize(fields[i].FieldType, data, ref position, out value))
                        {
                            Console.WriteLine("Error: Unsupported type \"" + fields[i].FieldType.FullName + "\"");
                            return null;
                        }

                        fields[i].SetValue(obj, value);
                    }
                
                    // Call the method
                    ret = method.Invoke(recipient, new object[1] { obj });
                }
                else
                {
                    // Call the method without the parameter
                    ret = method.Invoke(recipient, null);
                }

                // If the method returned a packet struct send it as a response
                if (ret != null && ret.GetType().GetInterfaces().Contains(typeof(IPacketStruct)))
                {
                    isResponseReliable = ((Packet)ret.GetType().GetCustomAttribute(typeof(Packet), false)).Reliable;
                    return (byte[])SerializePacketInfo.MakeGenericMethod(new[] { ret.GetType() }).Invoke(null, new[] { (PacketType)packetType, ret });
                }
                return null;
            }
            else
            {
                Console.WriteLine("Error: No method for packet type \"" + ((PacketType)packetType).ToString() + "\"");
                return null;
            }
        }

        private static readonly MethodInfo SerializePacketInfo = typeof(PacketSerializer).GetMethod("SerializePacket");
        public static byte[] SerializePacket<T>(T packet) where T : IPacketStruct
        {
            PacketType type = ((Packet)typeof(T).GetCustomAttribute(typeof(Packet))).Type;
            
            FieldInfo[] fields = packet.GetType().GetFields();

            int packetTypeSize = GetObjectSize(type);
            int packetSize = GetPacketSize(packet, fields);

            byte[] bytes = new byte[packetTypeSize + packetSize];
            int position = 0;

            // Write packet type
            if (!Serialize(type, bytes, ref position)) Console.WriteLine("Error: Packet serialization error");
            
            // Serialize structure fields
            for (int i = 0; i < fields.Length; i++)
            {
                if (!Serialize(fields[i].FieldType, fields[i].GetValue(packet), bytes, ref position))
                {
                    Console.WriteLine("Error: Unsupported type \"" + fields[i].FieldType.FullName + "\"");
                }
            }

            return bytes;
        }
        public static byte[] SerializeEvent(PacketType type)
        {
            byte[] bytes = new byte[GetObjectSize(type)];
            int position = 0;

            // Write packet type
            if (!Serialize(type, bytes, ref position)) Console.WriteLine("Error: Packet serialization error");

            return bytes;
        }


        public static bool Serialize<T>(T value, byte[] data, ref int position)
        {
            return Serialize(typeof(T), value, data, ref position);
        }
        public static bool Serialize(Type type, object value, byte[] data, ref int position)
        {
            if (type == typeof(byte))
            {
                data[position] = (byte)value;
                position++;
            }
            else if (type.GetInterfaces().Contains(typeof(IPacketSerializable)))
            {
                return ((IPacketSerializable)value).Serialize(data, ref position);
            }
            else if (type.IsEnum)
            {
                EnumPackageType packageType = (EnumPackageType)type.GetCustomAttribute(typeof(EnumPackageType), false);
                if (packageType == null)
                {
                    Console.WriteLine("Error: Enum \"" + type.FullName + "\" doesn't have EnumPackageType attribute");
                    return false;
                }
                
                return Serialize(packageType.type, DynamicCast(DynamicCast(value, type.GetEnumUnderlyingType()), packageType.type), data, ref position);
            }
            else if (type == typeof(bool))
            {
                byte[] bytes = BitConverter.GetBytes((bool)value);
                Array.Copy(bytes, 0, data, position, 1);
                position += 1;
            }
            else if (type == typeof(short))
            {
                byte[] bytes = BitConverter.GetBytes((short)value);
                Array.Copy(bytes, 0, data, position, 2);
                position += 2;
            }
            else if (type == typeof(ushort))
            {
                byte[] bytes = BitConverter.GetBytes((ushort)value);
                Array.Copy(bytes, 0, data, position, 2);
                position += 2;
            }
            else if (type == typeof(int))
            {
                byte[] bytes = BitConverter.GetBytes((int)value);
                Array.Copy(bytes, 0, data, position, 4);
                position += 4;
            }
            else if (type == typeof(uint))
            {
                byte[] bytes = BitConverter.GetBytes((uint)value);
                Array.Copy(bytes, 0, data, position, 4);
                position += 4;
            }
            else if (type == typeof(long))
            {
                byte[] bytes = BitConverter.GetBytes((long)value);
                Array.Copy(bytes, 0, data, position, 8);
                position += 8;
            }
            else if (type == typeof(ulong))
            {
                byte[] bytes = BitConverter.GetBytes((ulong)value);
                Array.Copy(bytes, 0, data, position, 8);
                position += 8;
            }
            else if (type == typeof(float))
            {
                byte[] bytes = BitConverter.GetBytes((float)value);
                Array.Copy(bytes, 0, data, position, 4);
                position += 4;
            }
            else if (type == typeof(double))
            {
                byte[] bytes = BitConverter.GetBytes((double)value);
                Array.Copy(bytes, 0, data, position, 8);
                position += 8;
            }
            else
            {
                return false;
            }

            return true;
        }

        public static bool Deserialize<T>(byte[] data, ref int position, out T value)
        {
            object obj;
            bool ret = Deserialize(typeof(T), data, ref position, out obj);

            value = (T)obj;
            return ret;
        }
        public static bool Deserialize(Type type, byte[] data, ref int position, out object value)
        {
            if (type == typeof(byte))
            {
                value = data[position];
                position++;
            }
            else if (type.GetInterfaces().Contains(typeof(IPacketSerializable)))
            {
                IPacketSerializable serializable = (IPacketSerializable)Activator.CreateInstance(type);
                serializable.Deserialize(data, ref position);
                value = serializable;
            }
            else if (type.IsEnum)
            {
                EnumPackageType packageType = (EnumPackageType)type.GetCustomAttribute(typeof(EnumPackageType), false);
                if (packageType == null)
                {
                    Console.WriteLine("Error: Enum \"" + type.FullName + "\" doesn't have EnumPackageType attribute");
                    value = null;
                    return false;
                }

                object obj;
                if (!Deserialize(packageType.type, data, ref position, out obj))
                {
                    value = null;
                    return false;
                }

                value = DynamicCast(DynamicCast(obj, type.GetEnumUnderlyingType()), type);
            }
            else if (type == typeof(bool))
            {
                value = BitConverter.ToBoolean(data, position);
                position++;
            }
            else if (type == typeof(short))
            {
                value = BitConverter.ToInt16(data, position);
                position += 2;
            }
            else if (type == typeof(ushort))
            {
                value = BitConverter.ToUInt16(data, position);
                position += 2;
            }
            else if (type == typeof(int))
            {
                value = BitConverter.ToInt32(data, position);
                position += 4;
            }
            else if (type == typeof(uint))
            {
                value = BitConverter.ToUInt32(data, position);
                position += 4;
            }
            else if (type == typeof(long))
            {
                value = BitConverter.ToInt64(data, position);
                position += 8;
            }
            else if (type == typeof(ulong))
            {
                value = BitConverter.ToUInt64(data, position);
                position += 8;
            }
            else if (type == typeof(float))
            {
                value = BitConverter.ToSingle(data, position);
                position += 4;
            }
            else if (type == typeof(double))
            {
                value = BitConverter.ToDouble(data, position);
                position += 8;
            }
            else
            {
                value = null;
                return false;
            }

            return true;
        }


        public static object CastTo<T>(dynamic value)
        {
            return (T)value;
        }
        private static readonly MethodInfo CastToInfo = typeof(PacketSerializer).GetMethod("CastTo");
        private static object DynamicCast(object value, Type type)
        {
            return CastToInfo.MakeGenericMethod(new[] { type }).Invoke(null, new[] { value });
        }

        private static readonly Dictionary<Type, int> TypeSizes = new Dictionary<Type, int>(){
            { typeof(byte), 1 },
            { typeof(bool), 1 },
            { typeof(short), 2 },
            { typeof(ushort), 2 },
            { typeof(int), 4 },
            { typeof(uint), 4 },
            { typeof(long), 8 },
            { typeof(ulong), 8 },
            { typeof(float), 4 },
            { typeof(double), 8 },
        };
        private static int GetPacketSize(object packet, FieldInfo[] fields)
        {
            int size = 0;
            for (int i = 0; i < fields.Length; i++)
            {
                size += GetObjectSize(fields[i].GetValue(packet), fields[i].FieldType);
            }
            return size;
        }
        public static int GetObjectSize<T>(T obj)
        {
            return GetObjectSize(obj, typeof(T));
        }
        public static int GetObjectSize(object obj, Type type)
        {
            int size;
            if (TypeSizes.TryGetValue(type, out size))
            {
                return size;
            }
            else if (type.GetInterfaces().Contains(typeof(IPacketSerializable)))
            {
                return ((IPacketSerializable)obj).GetSize();
            }
            else if (type.IsEnum)
            {
                EnumPackageType packageType = (EnumPackageType)type.GetCustomAttribute(typeof(EnumPackageType), false);
                if (packageType == null)
                {
                    Console.WriteLine("Error: Enum \"" + type.FullName + "\" doesn't have EnumPackageType attribute");
                    return 0;
                }

                return GetObjectSize(DynamicCast(DynamicCast(obj, type.GetEnumUnderlyingType()), packageType.type), packageType.type);
            }
            else
            {
                Console.WriteLine("Error: Unsupported type \"" + type.FullName + "\"");
                return 0;
            }
        }
    }
}
