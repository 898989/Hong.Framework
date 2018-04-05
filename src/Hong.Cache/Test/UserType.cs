using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hong.Test.Model
{
    [ProtoContract]
    public class UserType
    {
        [ProtoMember(1)]
        public string Name { get; set; }

        [ProtoMember(2)]
        public int ParentID { get; set; }
    }
}
