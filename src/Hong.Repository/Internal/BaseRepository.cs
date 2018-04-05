using System;
using System.Collections.Generic;
using Hong.Cache;
using Hong.Common.Extendsion;
using Hong.DAO;

namespace Hong.Repository.Internal
{
    public abstract class BaseRepository<Service, DataEntity>
    {
        /// <summary>获取实体数据ID
        /// </summary>
        protected Func<DataEntity, long> GetModelID = Reflection.GetField<DataEntity, long>("ID");

        /// <summary>实例化服务对象（通过数据实体ID）
        /// </summary>
        protected Func<long, Service> InstanceByID = Reflection.CreateInstance<Service, long>();

        /// <summary>实例化服务对象（通过数据实体）
        /// </summary>
        protected Func<DataEntity, Service> InstanceByEntity = Reflection.CreateInstance<Service, DataEntity>();

        /// <summary>仓库缓存管理对象
        /// </summary>
        protected ICacheManager<Service> CacheManager = CacheFactory.CreateCacheManager<Service>();

        /// <summary>数据库操作工厂
        /// </summary>
        protected DBFactory<DataEntity> DBFactory = new DBFactory<DataEntity>();

        /// <summary>从仓库获取服务对象（通过ID），ID小于1时返回一个新的服务对象
        /// </summary>
        /// <param name="id">数据行ID</param>
        /// <returns></returns>
        public Service Get(long id)
        {
            if (id < 1)
            {
                return InstanceByID(id);
            }

            var key = GetCacheKey(id);

            Service obj = CacheManager.TryGet(key);
            if (obj != null)
            {
                return obj;
            }

            obj = InstanceByID(id);
            CacheManager.TrySet(key, obj);

            return obj;
        }

        /// <summary>从仓库获取服务对象
        /// </summary>
        /// <param name="dataEntity">数据实体</param>
        /// <returns></returns>
        public Service Get(DataEntity dataEntity)
        {
            long id = GetModelID(dataEntity);
            if (id < 1)
            {
                throw new ArgumentException("参数" + nameof(dataEntity) + "无数据");
            }

            var key = GetCacheKey(id);

            Service obj = CacheManager.TryGet(key);
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
        public List<Service> Get(List<long> ids)
        {
            var list = new List<Service>();
            var queryIds = new List<long>();

            foreach (var item in ids)
            {
                Service obj = CacheManager.TryGet(GetCacheKey(item));
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

            foreach (var model in DBFactory.Model.Load(queryIds).Result)
            {
                CacheManager.TrySet(GetCacheKey(model), InstanceByEntity(model));
            }

            int count = ids.Count;
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
        protected virtual string GetCacheKey(long id)
        {
            return CacheIdentity + "." + id;
        }

        /// <summary>获取缓存KEY
        /// </summary>
        /// <param name="dataEntity">数据实体</param>
        /// <returns></returns>
        protected virtual string GetCacheKey(DataEntity dataEntity)
        {
            return CacheIdentity + "." + GetModelID(dataEntity);
        }

        public abstract void Add(Service service);
        public abstract void Add(List<Service> service);

        public abstract void Update(Service service);
        public abstract void Update(List<Service> service);

        public abstract void Remove(Service service);
        public abstract void Remove(List<Service> service);
    }
}
