using System;
using System.Collections.Generic;
using Hong.Cache;
using Hong.Common.Extendsion;
using Hong.DAO;
using System.Threading.Tasks;

namespace Hong.Service.Internal
{
    public abstract class BaseRepository<Service, DataEntity>
    {
        /// <summary>实例化服务对象（通过数据实体）
        /// </summary>
        protected readonly Func<DataEntity, Service> InstanceByEntity = Reflection.CreateInstance<Service, DataEntity>();

        /// <summary>仓库缓存管理对象
        /// </summary>
        protected virtual ICacheManager<Service> CacheManager { get; set; } = CacheFactory.CreateCacheManager<Service>();

        /// <summary>数据库操作工厂
        /// </summary>
        protected readonly DBFactory<DataEntity> DAO = DBFactory<DataEntity>.CreateInstance();

        /// <summary>从仓库获取服务对象（通过ID），ID小于1时返回一个新的服务对象
        /// </summary>
        /// <param name="id">数据行ID</param>
        /// <returns></returns>
        public async Task<Service> Get(int id)
        {
            if (id < 1)
            {
                return InstanceByEntity(DAO.Model.ModelInfo.CreateInstance());
            }

            var key = GetCacheKey(id);

            Service obj = CacheManager.TryGet(key);
            if (obj != null)
            {
                return obj;
            }

            obj = InstanceByEntity(await DAO.Model.Load(id));

            CacheManager.TrySet(key, obj);

            return obj;
        }

        /// <summary>从仓库获取服务对象
        /// </summary>
        /// <param name="dataEntity">数据实体</param>
        /// <returns></returns>
        public Service Get(DataEntity dataEntity)
        {
            var id = DAO.Model.ModelInfo.GetID(dataEntity);
            if (id < 1)
            {
                throw new ArgumentException("参数" + nameof(dataEntity) + "无数据");
            }

            var key = GetCacheKey(id);

            var obj = CacheManager.TryGet(key);
            if (obj != null)
            {
                return obj;
            }

            obj = InstanceByEntity(dataEntity);
            CacheManager.TrySet(key, obj);

            return obj;
        }

        /// <summary>从仓库获取服务对象
        /// </summary>
        /// <param name="ids">数据行ID集合</param>
        /// <returns></returns>
        public async Task<List<Service>> Get(List<int> ids)
        {
            var list = new List<Service>();
            var queryIds = new List<int>();

            foreach (var item in ids)
            {
                var obj = CacheManager.TryGet(GetCacheKey(item));
                list.Add(obj);
                if (obj == null)
                {
                    queryIds.Add(item);
                }
            }

            if (queryIds.Count == 0)
            {
                return list;
            }

            foreach (var model in await DAO.Model.Load(queryIds))
            {
                CacheManager.TrySet(GetCacheKey(model), InstanceByEntity(model));
            }

            var count = ids.Count;
            for (var index = 0; index < count; index++)
            {
                if (list[index] != null)
                {
                    continue;
                }

                list[index] = CacheManager.TryGet(GetCacheKey(ids[index]));
            }

            return list;
        }

        /// <summary>从仓库获取服务对象
        /// </summary>
        /// <param name="dataEntitys">数据实体集合</param>
        /// <returns></returns>
        public List<Service> Get(List<DataEntity> dataEntitys)
        {
            var services = new List<Service>();

            foreach (var m in dataEntitys)
            {
                services.Add(Get(m));
            }

            return services;
        }

        private string _cacheIdentity = null;
        /// <summary>缓存标识
        /// </summary>
        string CacheIdentity
        {
            get
            {
                if (_cacheIdentity != null)
                {
                    return _cacheIdentity;
                }

                var cacheSet = CacheSet.GetSet(typeof(Service));
                if (cacheSet == null)
                {
                    _cacheIdentity = typeof(Service).Name;
                }
                else
                {
                    _cacheIdentity = cacheSet.CacheIdentity;
                }

                return _cacheIdentity;
            }
        }

        /// <summary>获取缓存KEY
        /// </summary>
        /// <param name="id">数据行ID</param>
        /// <returns></returns>
        protected virtual string GetCacheKey(int id)
        {
            return CacheIdentity + "." + id;
        }

        /// <summary>获取缓存KEY
        /// </summary>
        /// <param name="dataEntity">数据实体</param>
        /// <returns></returns>
        protected virtual string GetCacheKey(DataEntity dataEntity)
            => GetCacheKey(DAO.Model.ModelInfo.GetID(dataEntity));

        public BaseRepository()
        {
            DAO.Model.AddedEvent += DataAddedEvent;
            DAO.Model.UpdatedEvent += DataUpdatedEvent;
            DAO.Model.DeletedEvent += DataDeletedEvent;
        }

        public abstract Task Add(Service service);
        public abstract Task Add(List<Service> service);

        public abstract Task Update(Service service);
        public abstract Task Update(List<Service> service);

        public abstract Task Remove(Service service);
        public abstract Task Remove(List<Service> service, bool activeNotifyEvent = true);

        public virtual async Task<List<Service>> Query(string sql, params object[] arguments)
        {
            return await Get(await DAO.Model.QueryID(sql, arguments));
        }

        protected virtual void DataAddedEvent(object sender, int id)
        {
            CacheManager.FlushAdd(GetCacheKey(id));
        }

        protected virtual void DataUpdatedEvent(object sender, int id)
        {
            var result = CacheManager.FlushUpdate(GetCacheKey(id));
            if (result == -1)
            {
                throw new Exception("刷新缓存时失败, 如果并发更新频繁请换方案");
            }
            else if (result == 0)
            {
                throw new Exception("更新缓存时失败");
            }
        }

        protected virtual void DataDeletedEvent(object sender, int id)
        {
            CacheManager.TryRemove(GetCacheKey(id));
        }
    }
}
