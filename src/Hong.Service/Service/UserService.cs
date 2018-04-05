using Hong.Service.Internal;
using Hong.Model;
using System;
using ProtoBuf;

namespace Hong.Service.Objects
{
    [Cache.CacheSet("UserService", "User")]
    [ProtoContract]
    public class UserService : BaseService<User>
    {
        [ProtoMember(1)]
        internal Model.User DataEntity;

        internal UserService()
        {
        }

        public UserService(User dataEntity)
        {
            DataEntity = dataEntity;
        }

        public int ID
        {
            get
            {
                return DataEntity.ID;
            }

            internal set
            {
                DataEntity.ID = value;
            }
        }

        public int Version
        {
            get { return DataEntity.ID; }
            set { DataEntity.Version = value; }
        }

        public string Account
        {
            get { return DataEntity.Account; }
            set { DataEntity.Account = value; }
        }

        public string Name
        {
            get { return DataEntity.Name; }
            set { DataEntity.Name = value; }
        }

        public DateTime AddTime
        {
            get { return DataEntity.AddTime; }
            set { DataEntity.AddTime = value; }
        }

        public string Mobile
        {
            get { return DataEntity.Mobile; }
            set { DataEntity.Mobile = value; }
        }

        public string Email
        {
            get { return DataEntity.Email; }
            set { DataEntity.Email = value; }
        }


        public override void ValidationAdd()
        {

        }

        public override void ValidationDelete()
        {
            throw new NotImplementedException();
        }

        public override void ValidationUpdate()
        {
            throw new NotImplementedException();
        }
    }
}
