using Hong.Cache;
using Hong.Cache.Core;
using ProtoBuf;
using System.Collections.Generic;

namespace Hong.Test.Model
{
    [CacheSet(ExpirationMode.None, 0, 0)]
    [ProtoContract]
    public class User : Service, IDataVersion
    {
        [ProtoMember(11)]
        public Dictionary<string, string> Dic = new Dictionary<string, string>();

        [ProtoMember(1)]
        public string Name;

        [ProtoMember(2)]
        public string Email = string.Empty;

        [ProtoMember(3)]
        public string Mobile;

        [ProtoMember(4)]
        public string Telephone { get; set; }

        string _Company = string.Empty;
        [ProtoMember(5)]
        public string Company
        {
            get
            {
                return _Company;
            }
            set
            {
                _Company = value;
            }
        }

        [ProtoMember(13)]
        public long Version { get; set; }

        [ProtoMember(6)]
        public int Age;

        [ProtoMember(7)]
        public string School;

        [ProtoMember(8)]
        public double Money;

        [ProtoMember(9)]
        public bool IsMarray = true;

        [ProtoMember(10)]
        public User Son;

        [ProtoMember(12)]
        public UserType Type { get; set; } = new UserType();
    }
}
