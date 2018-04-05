using ProtoBuf;

namespace Hong.Service.Internal
{
    public abstract class BaseService<Model>
    {
        public abstract void ValidationAdd();

        public abstract void ValidationUpdate();

        public abstract void ValidationDelete();
    }
}
