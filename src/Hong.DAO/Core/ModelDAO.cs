using Hong.Common.Extendsion;
using Hong.DAO.QueryCache;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using System.Threading.Tasks;
using static Hong.Common.Extendsion.Guard;
using static Hong.Common.Extendsion.Object;

namespace Hong.DAO.Core
{
    public class ModelDAO<Model> : DataBase, IModelDAO<Model>
    {
        /// <summary>实体命令仓库
        /// </summary>
        readonly CmdModelRepository cmdModelRepository = new CmdModelRepository();

        /// <summary>SQL命令仓库
        /// </summary>
        readonly CmdExcuteRepository cmdExcuteRepository = new CmdExcuteRepository();

        /// <summary> 实体信息
        /// </summary>
        readonly DataModelInfo<Model> modelInfo = new DBFieldHelper<Model>().GetModelInfo();

        /// <summary>查询缓存
        /// </summary>
        readonly QueryCacheManager<Model> queryCacheManager = new QueryCacheManager<Model>();

        /// <summary> 实体信息
        /// </summary>
        public DataModelInfo<Model> ModelInfo => modelInfo;

        internal ModelDAO(ILoggerFactory loggerFactory = null) : base(loggerFactory)
        {
        }

        #region 更新

        /// <summary>更新实体到数据库
        /// </summary>
        /// <param name="model">实体</param>
        public async Task Update(Model model)
        {
            NotNull(model, nameof(model));

            var id = modelInfo.GetID(model);
            if (id < 1)
            {
                throw new InvalidException("ID");
            }

            var cmd = cmdModelRepository.Pop(SQLAction.UPDATE, Connection);
            var hasVersion = modelInfo.GetVersion != null;
            cmd.CommandText = hasVersion
                ? modelInfo.UpdateSQL + id + " and version<=" + modelInfo.GetVersion(model)
                : modelInfo.UpdateSQL + id;

            var index = 0;
            var ps = cmd.Parameters;

            if (ps.Count == 0)
            {
                DBFieldAttribute dbField = null;

                foreach (var func in modelInfo.GetFields)
                {
                    dbField = modelInfo.DBFeilds[index];

                    var p = cmd.CreateParameter();
                    p.ParameterName = Configuration.ParamerNameStr + index++;
                    p.Value = func(model);
                    p.Size = dbField.Length;
                    p.DbType = dbField.DbType;
                    p.Direction = System.Data.ParameterDirection.Input;
                    ps.Add(p);
                }
            }
            else
            {
                foreach (var func in modelInfo.GetFields)
                {
                    ps[index++].Value = func(model);
                }
            }

            try
            {
                if (await ExecuteNonQuery(cmd) < 1)
                {
                    if (hasVersion)
                    {
                        Log?.LogWarning("#Method =>Update", "可能版本已过期");
                        throw new VersionException();
                    }
                    else
                    {
                        Log?.LogWarning("#Method =>Update", "可能数据不存在");
                        throw new DbException("行号" + id + "不存在");
                    }
                }
            }
            finally
            {
                cmdModelRepository.Push(SQLAction.UPDATE, cmd);
            }

            if (hasVersion)
            {
                modelInfo.VersionIncrement(model);
            }

            queryCacheManager.TryRemoveCache(modelInfo.TableName);

            if (AddedEvent != null)
            {
                if (Connection.CurrentTransaction.IsOpenedTransaction)
                {
                    Connection.CurrentTransaction.Events.Add(new EventItem() { Event = UpdatedEvent, ID = id });
                }
                else
                {
                    UpdatedEvent(this, id);
                }
            }
        }

        #endregion

        #region 添加

        /// <summary>新增实体到数据库
        /// </summary>
        /// <param name="model">实体</param>
        public async Task Insert(Model model)
        {
            NotNull(model, nameof(model));

            var cmd = cmdModelRepository.Pop(SQLAction.INSERT, Connection);
            var index = 0;
            var ps = cmd.Parameters;
            var id = 0;

            if (ps.Count == 0)
            {
                cmd.CommandText = modelInfo.InsertSQL;
                DBFieldAttribute dbField = null;

                foreach (var func in modelInfo.GetFields)
                {
                    dbField = modelInfo.DBFeilds[index];

                    var p = cmd.CreateParameter();
                    p.ParameterName = Configuration.ParamerNameStr + index++;
                    p.Value = func(model);
                    p.Size = dbField.Length;
                    p.DbType = dbField.DbType;
                    p.Direction = System.Data.ParameterDirection.Input;
                    ps.Add(p);
                }
            }
            else
            {
                foreach (var func in modelInfo.GetFields)
                {
                    ps[index++].Value = func(model);
                }
            }

            try
            {
                id = Convert.ToInt32(await ExecuteScalar(cmd));
            }
            finally
            {
                cmdModelRepository.Push(SQLAction.INSERT, cmd);
            }

            if (id < 1)
            {
                Log?.LogError("#Method =>Insert", "插入数据失败, 未能获取新的ID");
                throw new DbException("插入数据失败, 未能获取新的ID");
            }

            modelInfo.SetID(model, id);

            if (modelInfo.GetVersion != null)
            {
                modelInfo.VersionIncrement(model);
            }

            queryCacheManager.TryRemoveCache(modelInfo.TableName);

            if (AddedEvent != null)
            {
                if (Connection.CurrentTransaction.IsOpenedTransaction)
                {
                    Connection.CurrentTransaction.Events.Add(new EventItem() { Event = AddedEvent, ID = id });
                }
                else
                {
                    AddedEvent(this, id);
                }
            }
        }

        /// <summary>
        /// 批量新增,暂未实现
        /// </summary>
        /// <param name="models"></param>
        /// <returns></returns>
        //public async Task Insert(List<Model> models)
        //{
        //    NotNull(models, nameof(models));
        //    throw new NotSupportedException("批量语句暂未实现");
        //}
        #endregion

        #region 删除

        /// <summary>从数据库删除实体对应的行
        /// </summary>
        /// <param name="model">实体</param>
        public async Task Delete(Model model)
        {
            NotNull(model, nameof(model));

            var id = modelInfo.GetID(model);
            if (id < 1)
            {
                throw new InvalidException("ID");
            }

            var cmd = cmdModelRepository.Pop(SQLAction.DELETE, Connection);
            cmd.CommandText = modelInfo.DeleteSQL + id;

            try
            {
                await ExecuteNonQuery(cmd);
            }
            finally
            {
                cmdModelRepository.Push(SQLAction.DELETE, cmd);
            }

            queryCacheManager.TryRemoveCache(modelInfo.TableName);

            if (AddedEvent != null)
            {
                if (Connection.CurrentTransaction.IsOpenedTransaction)
                {
                    Connection.CurrentTransaction.Events.Add(new EventItem() { Event = DeletedEvent, ID = id });
                }
                else
                {
                    DeletedEvent(this, id);
                }
            }
        }

        /// <summary>
        /// 批量从数据库删除实体对应的行
        /// </summary>
        /// <param name="models">实体集合</param>
        /// <param name="activeNotifyEvent">是否激活删除通知事件</param>
        /// <returns></returns>
        public async Task Delete(List<Model> models, bool activeNotifyEvent = true)
        {
            NotNull(models, nameof(models));

            StringBuilder ids = new StringBuilder();
            foreach (var model in models)
            {
                NotNull(model, nameof(model));
                ids.Append(modelInfo.GetID(model)).Append(",");
            }
            ids.Remove(ids.Length - 1, 1);

            var cmd = cmdModelRepository.Pop(SQLAction.DELETE, Connection);
            cmd.CommandText = modelInfo.DeleteSQL.TrimEnd('=') + "(" + ids.ToString() + ")";
            ids = null;

            try
            {
                await ExecuteNonQuery(cmd);
            }
            finally
            {
                cmdModelRepository.Push(SQLAction.DELETE, cmd);
            }

            queryCacheManager.TryRemoveCache(modelInfo.TableName);

            if (activeNotifyEvent && AddedEvent != null)
            {
                if (Connection.CurrentTransaction.IsOpenedTransaction)
                {
                    foreach (var model in models)
                    {
                        Connection.CurrentTransaction.Events.Add(new EventItem() { Event = DeletedEvent, ID = modelInfo.GetID(model) });
                    }
                }
                else
                {
                    foreach (var model in models)
                    {
                        DeletedEvent(this, modelInfo.GetID(model));
                    }
                }
            }
        }
        #endregion

        #region 查询加载

        /// <summary>加载到数据
        /// </summary>
        /// <param name="model">被初始化的实体</param>
        public async Task Load(Model model)
        {
            NotNull(model, nameof(model));

            var id = modelInfo.GetID(model);
            if (id < 1)
            {
                throw new InvalidException("ID");
            }

            var cmd = cmdModelRepository.Pop(SQLAction.SELECT, Connection);
            cmd.CommandText = modelInfo.SelectSQL + id;
            var index = 0;

            try
            {
                await Connection.Open();
            }
            catch (Exception ex)
            {
                cmdModelRepository.Push(SQLAction.SELECT, cmd);
                Log?.LogError("#Method =>Load<Model> =>Connection.Open()", ex);

                throw;
            }

            try
            {
                using (DbDataReader reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false))
                {
                    if (reader.Read())
                    {
                        foreach (var fInfo in modelInfo.SetFields)
                        {
                            modelInfo.SetFields[index](model, reader.GetValue(index++));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log?.LogError("#Method =>Load<Model>", ex);
                throw;
            }
            finally
            {
                Connection.Close();
                cmdModelRepository.Push(SQLAction.SELECT, cmd);
            }
        }

        /// <summary>加载到数据
        /// </summary>
        /// <param name="id">数据行ID</param>
        public async Task<Model> Load(int id)
        {
            var model = modelInfo.CreateInstance();
            modelInfo.SetID(model, id);
            await Load(model);

            return model;
        }

        /// <summary>批量加载数据
        /// </summary>
        /// <param name="ids">行编号列表</param>
        /// <returns></returns>
        public async Task<List<Model>> Load(List<int> ids)
        {
            if (ids == null || ids.Count == 0)
            {
                return new List<Model>();
            }

            var sb = new StringBuilder();
            sb.Append("select ").Append(modelInfo.SelectFields)
                .Append(" from ")
                .Append(modelInfo.TableNameInSQL)
                .Append(" where id in (");
            foreach (int id in ids)
            {
                sb.Append(id).Append(",");
            }
            sb.Remove(sb.Length - 1, 1).Append(")");

            var index = 0;
            var models = new List<Model>();
            var cmd = cmdModelRepository.Pop(SQLAction.SELECT, Connection);

            cmd.CommandText = sb.ToString();
            sb = null;

            try
            {
                await Connection.Open();
            }
            catch (Exception ex)
            {
                cmdModelRepository.Push(SQLAction.SELECT, cmd);
                Log?.LogError("#Method =>Load<List> =>Connection.Open()", ex);

                throw;
            }

            try
            {
                using (var reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false))
                {
                    while (reader.Read())
                    {
                        index = 0;
                        var model = modelInfo.CreateInstance();
                        foreach (var fInfo in modelInfo.SetFields)
                        {
                            modelInfo.SetFields[index](model, reader.GetValue(index++));
                        }

                        models.Add(model);
                    }
                }
            }
            catch (Exception ex)
            {
                Log?.LogError("#Method =>Load<List>", ex);
                throw;
            }
            finally
            {
                Connection.Close();
                cmdModelRepository.Push(SQLAction.SELECT, cmd);
            }

            return models;
        }

        /// <summary>查询返回ID,具有查询缓存, 查询频率比较高或业务缓存变动频次无法预知的情况使用
        /// 纯ID查询请使用Load方法
        /// </summary>
        /// <param name="cmdText">条件</param>
        /// <param name="arguments">参数</param>
        /// <returns>返回条件结果行号,返回结果不会为 null</returns>
        public Task<List<int>> QueryID(string cmdText, params object[] arguments)
        {
            NotNullOrEmpty(cmdText, nameof(cmdText));
            var ids = new List<int>();

            return queryCacheManager.GetCacheData(async () =>
            {
                var repository = cmdExcuteRepository.GetRepository(cmdText);
                var cmd = repository.Pop(Connection);
                cmd.CommandText = cmdText;

                if (arguments != null)
                {
                    var count = cmd.Parameters.Count;
                    if (count == 0)
                    {
                        AddGeneralParams(cmd, arguments);
                    }
                    else
                    {
                        var p = cmd.Parameters;
                        for (var i = 0; i < count; i++)
                        {
                            p[i].Value = arguments[i];
                        }
                    }
                }

                try
                {
                    await Connection.Open();
                }
                catch (Exception ex)
                {
                    repository.Push(cmd);
                    Log?.LogError("#Method =>QueryID =>Connection.Open()", ex);

                    throw;
                }

                try
                {
                    using (DbDataReader reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false))
                    {
                        while (reader.Read())
                        {
                            ids.Add(reader.GetInt32(0));
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log?.LogError("#Method =>QueryID", ex);
                    throw;
                }
                finally
                {
                    Connection.Close();
                    repository.Push(cmd);
                }

                return ids;
            }, cmdText, arguments);
        }

        /// <summary>查询反回实体,无查询缓存, 对于数据变动频率非常小比如操作,通常在业务层缓存固定的情况使用
        /// 纯ID查询请使用Load方法
        /// </summary>
        /// <param name="cmdText">条件</param>
        /// <param name="arguments">参数</param>
        /// <returns>返回条件结果行号</returns>
        public async Task<List<Model>> QueryModel(string cmdText, params object[] arguments)
        {
            NotNullOrEmpty(cmdText, nameof(cmdText));

            var models = new List<Model>();
            var index = 0;
            var repository = cmdExcuteRepository.GetRepository(cmdText);
            var cmd = repository.Pop(Connection);
            cmd.CommandText = cmdText;

            if (arguments != null)
            {
                var count = cmd.Parameters.Count;
                if (count == 0)
                {
                    AddGeneralParams(cmd, arguments);
                }
                else
                {
                    var p = cmd.Parameters;
                    for (var i = 0; i < count; i++)
                    {
                        p[i].Value = arguments[i];
                    }
                }
            }

            try
            {
                await Connection.Open();
            }
            catch (Exception ex)
            {
                repository.Push(cmd);
                Log?.LogError("#Method =>QueryModel =>Connection.Open()", ex);

                throw;
            }

            try
            {
                using (DbDataReader reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false))
                {
                    while (reader.Read())
                    {
                        index = 0;
                        var model = modelInfo.CreateInstance();
                        foreach (var fInfo in modelInfo.SetFields)
                        {
                            modelInfo.SetFields[index](model, reader.GetValue(index++));
                        }
                        models.Add(model);
                    }
                }
            }
            catch (Exception ex)
            {
                Log?.LogError("#Method =>QueryModel", ex);
                throw;
            }
            finally
            {
                Connection.Close();
                repository.Push(cmd);
            }

            return models;
        }

        /// <summary>获取字段值,用于延时加载字段值,通常用于text类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model">实体</param>
        /// <param name="dbField">数据表字段名称</param>
        /// <returns></returns>
        public async Task<FieldType> GetFeild<FieldType>(Model model, string dbField)
        {
            NotNull(model, nameof(model));
            NotNullOrEmpty(dbField, nameof(dbField));

            foreach (var c in dbField)
            {
                if (c == '\'' || c == ' ' || c == ';')
                {
                    throw new InvalidException(dbField);
                }
            }

            var cmd = cmdModelRepository.Pop(SQLAction.SELECT, Connection);
            cmd.CommandText = string.Format("select {0} from {1} where id={2}", dbField, modelInfo.TableNameInSQL, modelInfo.GetID(model));
            object value = null;

            try
            {
                value = await ExecuteScalar(cmd);
            }
            finally
            {
                cmdModelRepository.Push(SQLAction.SELECT, cmd);
            }

            if (value == null)
            {
                return default(FieldType);
            }

            return (FieldType)value;
        }

        #endregion
    }
}
