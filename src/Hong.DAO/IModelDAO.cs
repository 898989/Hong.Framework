using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hong.DAO.Core
{
    public interface IModelDAO<Model>
    {
        /// <summary> 实体信息
        /// </summary>
        DataModelInfo<Model> ModelInfo { get; }

        /// <summary>从数据库删除实体对应的行
        /// </summary>
        /// <param name="model">实体</param>
        Task Delete(Model model);

        /// <summary>
        /// 从数据库批量删除实体对应的行
        /// </summary>
        /// <param name="model">实体</param>
        /// <param name="activeNotifyEvent">是否激活通知事件</param>
        /// <returns></returns>
        Task Delete(List<Model> model, bool activeNotifyEvent = true);

        /// <summary>获取字段值,用于延时加载字段值,通常用于text类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model">实体</param>
        /// <param name="dbField">数据表字段名称</param>
        /// <returns></returns>
        Task<FieldType> GetFeild<FieldType>(Model model, string dbField);

        /// <summary>新增实体到数据库
        /// </summary>
        /// <param name="model">实体</param>
        Task Insert(Model model);

        /// <summary>加载到数据
        /// </summary>
        /// <param name="model">被初始化的实体</param>
        Task Load(Model model);

        /// <summary>批量加载数据
        /// </summary>
        /// <param name="ids">行编号列表</param>
        /// <returns></returns>
        Task<List<Model>> Load(List<int> ids);

        /// <summary>加载到数据
        /// </summary>
        /// <param name="id">数据行ID</param>
        Task<Model> Load(int id);

        /// <summary>查询返回ID,具有查询缓存, 查询频率比较高或业务缓存变动频次无法预知的情况使用
        /// </summary>
        /// <param name="cmdText">条件</param>
        /// <param name="arguments">参数</param>
        /// <returns>返回条件结果行号,返回结果不会为 null</returns>
        Task<List<int>> QueryID(string cmdText, params object[] arguments);

        /// <summary>查询反回实体,无查询缓存, 对于数据变动频率非常小比如操作,通常在业务层缓存固定的情况使用
        /// </summary>
        /// <param name="cmdText">条件</param>
        /// <param name="arguments">参数</param>
        /// <returns>返回条件结果行号</returns>
        Task<List<Model>> QueryModel(string cmdText, params object[] arguments);

        /// <summary>更新实体到数据库
        /// </summary>
        /// <param name="model">实体</param>
        Task Update(Model model);

        /// <summary>更新完成事件
        /// </summary>
        EventHandler<int> UpdatedEvent { get; set; }

        /// <summary>删除完成事件
        /// </summary>
        EventHandler<int> DeletedEvent { get; set; }

        /// <summary>新增完成事件
        /// </summary>
        EventHandler<int> AddedEvent { get; set; }
    }
}