using System;
using System.Collections;
using System.Linq;
using static Hong.Common.Extendsion.Guard;
using StackRedis = StackExchange.Redis;

namespace Hong.Cache.Redis
{
    internal interface IRedisValueConverter
    {
        StackRedis.RedisValue ToRedisValue<T>(T value);

        T FromRedisValue<T>(StackRedis.RedisValue value, string valueType);
    }

    internal interface IRedisValueConverter<T>
    {
        StackRedis.RedisValue ToRedisValue(T value);

        T FromRedisValue(StackRedis.RedisValue value, string valueType);
    }

    internal class RedisValueConverter :
        IRedisValueConverter,
        IRedisValueConverter<byte[]>,
        IRedisValueConverter<string>,
        IRedisValueConverter<int>,
        IRedisValueConverter<double>,
        IRedisValueConverter<bool>,
        IRedisValueConverter<long>,
        IRedisValueConverter<object>
    {
        private static readonly Type ByteArrayType = typeof(byte[]);
        private static readonly Type StringType = typeof(string);
        private static readonly Type IntType = typeof(int);
        private static readonly Type DoubleType = typeof(double);
        private static readonly Type BoolType = typeof(bool);
        private static readonly Type LongType = typeof(long);
        private readonly Hashtable types = new Hashtable();
        private readonly object typesLock = new object();

        public RedisValueConverter()
        {
        }

        StackRedis.RedisValue IRedisValueConverter<byte[]>.ToRedisValue(byte[] value) => value;
        StackRedis.RedisValue IRedisValueConverter<int>.ToRedisValue(int value) => value;
        StackRedis.RedisValue IRedisValueConverter<double>.ToRedisValue(double value) => value;
        StackRedis.RedisValue IRedisValueConverter<bool>.ToRedisValue(bool value) => value;
        StackRedis.RedisValue IRedisValueConverter<long>.ToRedisValue(long value) => value;
        StackRedis.RedisValue IRedisValueConverter<string>.ToRedisValue(string value) => value;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Scope = "member", Target = "CacheManager.Redis.RedisValueConverter.#CacheManager.Redis.IRedisValueConverter`1<System.Object>.ToRedisValue(System.Object)", Justification = "For performance reasons we don't do checks at this point. Also, its internally used only.")]
        StackRedis.RedisValue IRedisValueConverter<object>.ToRedisValue(object value)
        {
            var valueType = value.GetType();
            if (valueType == ByteArrayType)
            {
                var converter = (IRedisValueConverter<byte[]>)this;
                return converter.ToRedisValue((byte[])value);
            }
            else if (valueType == StringType)
            {
                var converter = (IRedisValueConverter<string>)this;
                return converter.ToRedisValue((string)value);
            }
            else if (valueType == IntType)
            {
                var converter = (IRedisValueConverter<int>)this;
                return converter.ToRedisValue((int)value);
            }
            else if (valueType == DoubleType)
            {
                var converter = (IRedisValueConverter<double>)this;
                return converter.ToRedisValue((double)value);
            }
            else if (valueType == BoolType)
            {
                var converter = (IRedisValueConverter<bool>)this;
                return converter.ToRedisValue((bool)value);
            }
            else if (valueType == LongType)
            {
                var converter = (IRedisValueConverter<long>)this;
                return converter.ToRedisValue((long)value);
            }

            return Common.Tools.ProtoBufSerializer.Serialize(value);
        }


        byte[] IRedisValueConverter<byte[]>.FromRedisValue(StackRedis.RedisValue value, string valueType) => value;
        string IRedisValueConverter<string>.FromRedisValue(StackRedis.RedisValue value, string valueType) => value;
        int IRedisValueConverter<int>.FromRedisValue(StackRedis.RedisValue value, string valueType) => (int)value;
        double IRedisValueConverter<double>.FromRedisValue(StackRedis.RedisValue value, string valueType) => (double)value;
        bool IRedisValueConverter<bool>.FromRedisValue(StackRedis.RedisValue value, string valueType) => (bool)value;
        long IRedisValueConverter<long>.FromRedisValue(StackRedis.RedisValue value, string valueType) => (long)value;
        object IRedisValueConverter<object>.FromRedisValue(StackRedis.RedisValue value, string type)
        {
            var valueType = this.GetType(type);

            if (valueType == ByteArrayType)
            {
                var converter = (IRedisValueConverter<byte[]>)this;
                return converter.FromRedisValue(value, type);
            }
            else if (valueType == StringType)
            {
                var converter = (IRedisValueConverter<string>)this;
                return converter.FromRedisValue(value, type);
            }
            else if (valueType == IntType)
            {
                var converter = (IRedisValueConverter<int>)this;
                return converter.FromRedisValue(value, type);
            }
            else if (valueType == DoubleType)
            {
                var converter = (IRedisValueConverter<double>)this;
                return converter.FromRedisValue(value, type);
            }
            else if (valueType == BoolType)
            {
                var converter = (IRedisValueConverter<bool>)this;
                return converter.FromRedisValue(value, type);
            }
            else if (valueType == LongType)
            {
                var converter = (IRedisValueConverter<long>)this;
                return converter.FromRedisValue(value, type);
            }

            return this.Deserialize(value, type);
        }


        public StackRedis.RedisValue ToRedisValue<T>(T value) => Common.Tools.ProtoBufSerializer.Serialize(value);

        public T FromRedisValue<T>(StackRedis.RedisValue value, string valueType) => (T)this.Deserialize(value, valueType);

        private object Deserialize(StackRedis.RedisValue value, string valueType)
        {
            var type = this.GetType(valueType);
            EnsureNotNull(type, "Type could not be loaded, {0}.", valueType);

            return Common.Tools.ProtoBufSerializer.Deserialize(value, type);
        }


        private Type GetType(string type)
        {
            if (!this.types.ContainsKey(type))
            {
                lock (this.typesLock)
                {
                    if (!this.types.ContainsKey(type))
                    {
                        var typeResult = Type.GetType(type, false);
                        if (typeResult == null)
                        {
                            // fixing an issue for corlib types if mixing net core clr and full clr calls 
                            // (e.g. typeof(string) is different for those two, either System.String, System.Private.CoreLib or System.String, mscorlib)
                            var typeName = type.Split(',').FirstOrDefault();
                            typeResult = Type.GetType(typeName, true);
                        }

                        this.types.Add(type, typeResult);
                    }
                }
            }

            return (Type)this.types[type];
        }
    }
}