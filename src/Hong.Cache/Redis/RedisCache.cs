using System;
using System.Collections.Generic;
using System.Linq;
using StackRedis = StackExchange.Redis;
using Hong.Common.Extendsion;
using Hong.Cache.Core;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace Hong.Cache.Redis
{
    public class RedisCache<TValue> : Cache<TValue>
    {
        const string HashFieldVersion = "version";
        const string HashFieldExpirationMode = "expiration";
        const string HashFieldExpirationTimeout = "timeout";
        const string HashFieldType = "type";
        const string HashFieldValue = "value";

        //键已存在不存在不作任何操作     ARGV 1:HashFieldValue, 2:HashFieldType, 3:HashFieldExpirationMode, 4:HashFieldExpirationTimeout, 5:HashFieldVersion
        static readonly string ScriptAdd = $@"
if redis.call('HSETNX', KEYS[1], '{HashFieldValue}', ARGV[1]) == 1 then
    local result = redis.call('HMSET', KEYS[1], '{HashFieldType}', ARGV[2], '{HashFieldExpirationMode}', ARGV[3], '{HashFieldExpirationTimeout}', ARGV[4], '{HashFieldVersion}', ARGV[5])
    if ARGV[3] ~= '0' and ARGV[4] ~= '0' then
        redis.call('PEXPIRE', KEYS[1], ARGV[4])
    end
    if KEYS[2] then 
        redis.call('HSET', KEYS[2], KEYS[1], '1')
    end

    return result
end
return nil";

        //tonumber(result) 版本号必须更新时才能更新,存在更新不存在添加 ARGV 1:HashFieldValue, 2:HashFieldType, 3:HashFieldExpirationMode, 4:HashFieldExpirationTimeout, 5:HashFieldVersion
        static readonly string ScriptPut = $@"
local version = redis.call('HGET', KEYS[1], '{HashFieldVersion}')
if version and ARGV[5] <= version then
    return 'TIMEOUT'
end
local result = redis.call('HMSET', KEYS[1], '{HashFieldValue}', ARGV[1], '{HashFieldType}', ARGV[2], '{HashFieldExpirationMode}', ARGV[3], '{HashFieldExpirationTimeout}', ARGV[4], '{HashFieldVersion}', ARGV[5])
if ARGV[3] ~= '0' and ARGV[4] ~= '0' then
    redis.call('PEXPIRE', KEYS[1], ARGV[4])
end
if KEYS[2] then 
    redis.call('HSET', KEYS[2], KEYS[1], '1')
end
return result
";
        /*
        private static readonly string ScriptPut = $@"
local version = redis.call('HGET', KEYS[1], '{HashFieldVersion}')
if version and ARGV[5] < version then
    return 'TIMEOUT'
end
ARGV[5] = ARGV[5] + 1
local result = redis.call('HMSET', KEYS[1], '{HashFieldValue}', ARGV[1], '{HashFieldType}', ARGV[2], '{HashFieldExpirationMode}', ARGV[3], '{HashFieldExpirationTimeout}', ARGV[4], '{HashFieldVersion}', ARGV[5])
if ARGV[3] ~= '0' and ARGV[4] ~= '0' then
    redis.call('PEXPIRE', KEYS[1], ARGV[4])
end
if KEYS[2] then 
    redis.call('HSET', KEYS[2], KEYS[1], 'key')
end
return result
";*/

        //tonumber()版本号必须更新时才能更新 ARGV 1:HashFieldValue,2:HashFieldVersion
        static readonly string ScriptUpdate = $@"
local version = redis.call('HGET', KEYS[1], '{HashFieldVersion}')
if not version then
    return 'NO'
elseif ARGV[2] <= version then
    return 'TIMEOUT'
end
return redis.call('HMSET', KEYS[1], '{HashFieldValue}', ARGV[1],'{HashFieldVersion}', ARGV[2])
";

        /*private static readonly string ScriptUpdate = $@"
local version = redis.call('HGET', KEYS[1], '{HashFieldVersion}')
if not version then
    return 'NO'
elseif ARGV[2] < version then
    return 'TIMEOUT'
end
ARGV[2] = ARGV[2] + 1
return redis.call('HMSET', KEYS[1], '{HashFieldValue}', ARGV[1],'{HashFieldVersion}', ARGV[2])
";*/

        //支持滑动过期
        static readonly string ScriptGet = $@"
local result = redis.call('HMGET', KEYS[1], '{HashFieldValue}', '{HashFieldExpirationMode}', '{HashFieldExpirationTimeout}', '{HashFieldType}', '{HashFieldVersion}')
if (result[2] == '1' and result[3] ~= '' and result[3] ~= '0') then
    redis.call('PEXPIRE', KEYS[1], result[3])
end
return result";

        static readonly string ScriptDel = $@"
redis.call('HDEL', KEYS[1], '{HashFieldValue}', '{HashFieldExpirationMode}', '{HashFieldExpirationTimeout}', '{HashFieldType}')
local result = redis.call('HDEL', KEYS[2], KEYS[1])
return result";

        RedisConnectionManager Connection = null;

        static Dictionary<ScriptType, StackRedis.LoadedLuaScript> LuaScripts = new Dictionary<ScriptType, StackRedis.LoadedLuaScript>();

        /// <summary>
        /// 脚本初始化全局状态
        /// </summary>
        static Dictionary<string, bool> _scriptInitStatus = new Dictionary<string, bool>();

        static readonly RedisValueConverter ValueConverter = new RedisValueConverter();

        readonly ILogger Log = null;

        /// <summary>是否允许执行 lua 脚本
        /// </summary>
        public bool AlloweLua { get; set; }

        internal RedisCache(RedisCacheConfiguration configuration, ILoggerFactory loggerFactory = null)
        {
            Connection = new RedisConnectionManager(configuration);
            AlloweLua = configuration.AllowAdmin;

            Log = loggerFactory?.CreateLogger("RedisCache");
            TryCount = configuration.TryCount;
        }

        IEnumerable<StackRedis.IServer> Servers => Connection.Servers;

        StackRedis.IDatabase Database => Connection.Database;

        bool _ScriptsLoaded = false;
        /// <summary>
        /// 脚本是否初始化
        /// </summary>
        bool ScriptsLoaded
        {
            get
            {
                if (_ScriptsLoaded)
                {
                    return true;
                }

                _scriptInitStatus.TryGetValue(Connection?.connectionString, out _ScriptsLoaded);
                return _ScriptsLoaded;
            }
            set
            {
                _ScriptsLoaded = value;
            }
        }

        /// <summary>
        /// 区域里面的所有key
        /// </summary>
        /// <param name="region">区域</param>
        /// <returns></returns>
        public override IEnumerable<string> RegionHasKeys(string region)
        {
            var hashKeys = Database.HashKeys(region);

            if (hashKeys.Length > 0)
            {
                // lets remove all keys which where in the region
                // 01/32/16 changed to remove one by one because on clusters the keys could belong to multiple slots
                foreach (var key in hashKeys.Where(p => p.HasValue))
                {
                    yield return key.ToString();
                }
            }
        }

        public override bool TryAdd(string key, string region, TValue value, long version, ExpirationMode expirationMode = ExpirationMode.None, int cacheTimeSpan = 0)
        {
            if (!string.IsNullOrWhiteSpace(region))
            {
                key = BuildKey(key, region);
            }

            if (!this.AlloweLua)
            {
                try
                {
                    return SetNoScript(key, region, value, version, StackRedis.When.NotExists, expirationMode, cacheTimeSpan) == 0;
                }
                catch (Exception ex)
                {
                    Log?.LogError("#Method =>TryAdd", ex);
                    return false;
                }
            }

            return SetByScript(key, region, value, version, StackRedis.When.NotExists, expirationMode, cacheTimeSpan) == 0;
        }

        public override async Task<bool> TryClear() =>
            await Task.Factory.StartNew(() =>
            {
                foreach (var server in Servers.Where(p => !p.IsSlave))
                {
                    if (!server.IsConnected) continue;

                    try
                    {
                        server.FlushDatabase();
                    }
                    catch (Exception ex)
                    {
                        Log?.LogError("#Method =>TryClear", ex);
                        return false;
                    }
                }

                return true;
            });

        public override async Task<bool> TryClear(string region) =>
            await Task.Factory.StartNew(() =>
            {
                var db = Database;

                foreach (var key in RegionHasKeys(region))
                {
                    db.KeyDelete(key, StackRedis.CommandFlags.FireAndForget);
                }

                // now delete the region
                return db.KeyDelete(region);
            });

        public override bool TryExpire(string key, string region, ExpirationMode expirationMode, int cacheTimeSpan)
        {
            if (!string.IsNullOrWhiteSpace(region))
            {
                key = BuildKey(key, region);
            }

            throw new NotImplementedException();
        }

        public override TValue TryGet(string key, string region)
        {
            if (!string.IsNullOrWhiteSpace(region))
            {
                key = BuildKey(key, region);
            }

            if (!this.AlloweLua)
            {
                return this.GetNoScript(key);
            }

            return GetByScript(key);
        }

        public override bool TryRemove(string key, string region)
        {
            if (!string.IsNullOrWhiteSpace(region))
            {
                key = BuildKey(key, region);
            }

            if (!AlloweLua)
            {
                return RemoveNotScript(key, region);
            }

            return RemoveByScript(key, region);
        }

        public override short TrySet(string key, string region, TValue value, long version, ExpirationMode expirationMode = ExpirationMode.None, int cacheTimeSpan = 0)
        {
            if (!string.IsNullOrWhiteSpace(region))
            {
                key = BuildKey(key, region);
            }

            if (!AlloweLua)
            {
                try
                {
                    return SetNoScript(key, region, value, version, StackRedis.When.Always, expirationMode, cacheTimeSpan);
                }
                catch (Exception ex)
                {
                    Log?.LogError("#Method =>TrySet", ex);
                    return 0;
                }
            }

            return SetByScript(key, region, value, version, StackRedis.When.Always, expirationMode, cacheTimeSpan);
        }

        public override short TryUpdate(string key, string region, TValue value, long version)
        {
            if (!string.IsNullOrWhiteSpace(region))
            {
                key = BuildKey(key, region);
            }

            if (!this.AlloweLua)
            {
                return UpdateNotScript(key, region, value, version);
            }

            return UpdateByScript(key, region, value, version);
        }


        private TValue GetByScript(string fullKey)
        {
            StackRedis.RedisResult result = null;

            try
            {
                result = this.Eval(ScriptType.Get, fullKey);
            }
            catch (Exception ex)
            {
                Log?.LogError("#Method =>GetByScript", ex);
                return default(TValue);
            }

            if (result == null || result.IsNull)
            {
                return default(TValue);
            }

            var values = (StackRedis.RedisValue[])result;

            var value = values[0];
            var expirationMode = values[1];
            var timeout = values[2];
            var valueType = values[3];
            var version = values[4];

            return FromRedisValue(value, valueType);
        }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        private TValue GetNoScript(string fullKey)
        {
            var getFields = new StackRedis.RedisValue[]
            {
                HashFieldValue,
                HashFieldExpirationTimeout,
                HashFieldVersion
            };

            StackRedis.RedisValue[] values = null;

            try
            {
                values = Database.HashGet(fullKey, getFields, StackRedis.CommandFlags.None);

                if (values == null || values.Length != 3)
                {
                    return default(TValue);
                }

                Database.KeyExpire(fullKey, new TimeSpan(values[2].TryToType<int>(0)), StackRedis.CommandFlags.FireAndForget);
            }
            catch (Exception ex)
            {
                Log?.LogError("#Method =>GetNoScript", ex);
                return default(TValue);
            }

            return FromRedisValue(values[0], typeof(TValue).FullName);
        }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member


        public short SetByScript(string fullKey, string region, TValue value, long version, StackRedis.When when, ExpirationMode expirationMode = ExpirationMode.None, int cacheTimeSpan = 0)
        {
            var val = ToRedisValue(value);
            //var flags = sync ? StackRedis.CommandFlags.None : StackRedis.CommandFlags.FireAndForget;
            var flags = StackRedis.CommandFlags.None;

            //ARGV [1]: value, [2]: type, [3]: expirationMode, [4]: expirationTimeout(millis), [5]: created(ticks)
            var parameters = new StackRedis.RedisValue[]
            {
                val,
                value.GetType().AssemblyQualifiedName,
                (int)expirationMode,
                cacheTimeSpan,
                version
            };

            StackRedis.RedisResult result = null;

            try
            {
                result = Eval(when == StackRedis.When.NotExists ? ScriptType.Add : ScriptType.Put, fullKey, region, parameters, flags);
            }
            catch (Exception ex)
            {
                Log?.LogError("#Method =>SetByScript", ex);
                return 0;
            }

            if (result == null)
            {
                if (flags.HasFlag(StackRedis.CommandFlags.FireAndForget))
                {
                    //还未得到结果
                    return 1;
                }

                return 0;
            }

            var resultValue = (StackRedis.RedisValue)result;

            if (resultValue.HasValue && resultValue.ToString().Equals("OK", StringComparison.OrdinalIgnoreCase))
            {
                return 1;
            }
            else if (resultValue.HasValue && resultValue.ToString().Equals("TIMEOUT"))
            {
                return -1;
            }

            //this.Logger.LogWarn("DB {0} | Failed to set item {1}: {2}.", this.connection.Database.Database, fullKey, resultValue.ToString());
            return 0;
        }
        private short SetNoScript(string fullKey, string region, TValue value, long version, StackRedis.When when, ExpirationMode expirationMode = ExpirationMode.None, int cacheTimeSpan = 0, bool sync = false)
        {
            var redisValue = this.ToRedisValue(value);

            var keyValue = new StackRedis.HashEntry[]
            {
                new StackRedis.HashEntry(HashFieldValue,redisValue),
                new StackRedis.HashEntry(HashFieldType,value.GetType().AssemblyQualifiedName),
                new StackRedis.HashEntry(HashFieldExpirationMode,(int)expirationMode),
                new StackRedis.HashEntry(HashFieldExpirationTimeout,cacheTimeSpan),
                new StackRedis.HashEntry(HashFieldVersion,version)
            };

            //var flags = sync ? StackRedis.CommandFlags.None : StackRedis.CommandFlags.FireAndForget;
            var flags = StackRedis.CommandFlags.None;

            var remoteVer = Database.HashGet(fullKey, HashFieldVersion, StackRedis.CommandFlags.None);
            if (!remoteVer.IsNull)
            {
                if (when == StackRedis.When.NotExists)
                {
                    return 0;
                }

                if (remoteVer.TryToType<long>(0) >= version)
                {
                    return -1;
                }
            }

            Database.HashSet(fullKey, keyValue, flags);

            if (!string.IsNullOrWhiteSpace(region))
            {
                if (!Database.HashSet(region, fullKey, "key", StackRedis.When.Always, StackRedis.CommandFlags.FireAndForget))
                {
                    return -1;
                }
            }

            if (expirationMode != ExpirationMode.None)
            {
                if (!Database.KeyExpire(fullKey, TimeSpan.FromSeconds(cacheTimeSpan), StackRedis.CommandFlags.FireAndForget))
                {
                    return -1;
                }
            }
            else
            {
                // bugfix #9
                if (!Database.KeyExpire(fullKey, default(TimeSpan?), StackRedis.CommandFlags.FireAndForget))
                {
                    return -1;
                }
            }

            return 1;
        }


        private short UpdateByScript(string fullKey, string region, TValue value, long version)
        {
            var val = this.ToRedisValue(value);

            //ARGV [1]: value, [2]: version
            var parameters = new StackRedis.RedisValue[]
            {
                val,
                version
            };

            StackRedis.RedisResult result = null;
            //只能同步必须得到结果

            try
            {
                result = Eval(ScriptType.Update, fullKey, region, parameters, StackRedis.CommandFlags.None);
            }
            catch (Exception ex)
            {
                Log?.LogError("#Method =>UpdateByScript", ex);
                return 0;
            }

            if (result == null || result.IsNull)
            {
                return 0;
            }

            var resultValue = (StackRedis.RedisValue)result;

            if (resultValue.HasValue)
            {
                switch (resultValue.ToString())
                {
                    case "OK":
                        return 1;
                    case "TIMEOUT":
                        return -1;
                    case "NO":
                        return 0;
                }
            }

            return 0;
        }
        private short UpdateNotScript(string fullKey, string region, TValue value, long version)
        {
            var redisValue = this.ToRedisValue(value);

            var keyValue = new StackRedis.HashEntry[]
            {
               new StackRedis.HashEntry(HashFieldValue,redisValue),
               new StackRedis.HashEntry(HashFieldVersion,version)
            };

            try
            {
                var tran = Database.CreateTransaction();
                tran.AddCondition(StackRedis.Condition.HashEqual(fullKey, HashFieldVersion, version - 1));
                tran.HashSetAsync(fullKey, keyValue);

                if (tran.Execute())
                {
                    return 1;
                }
            }
            catch (Exception ex)
            {
                Log?.LogError("#Method =>UpdateNotScript", ex);
                return 0;
            }

            return 0;
        }


        private bool RemoveByScript(string fullKey, string region, bool sync = false)
        {
            var flags = sync ? StackRedis.CommandFlags.None : StackRedis.CommandFlags.FireAndForget;
            StackRedis.RedisResult result = null;

            try
            {
                result = this.Eval(ScriptType.Del, fullKey, region, null, flags);
            }
            catch (Exception ex)
            {
                Log?.LogError("#Method =>RemoveByScript", ex);
                return false;
            }

            if (result == null)
            {
                if (flags.HasFlag(StackRedis.CommandFlags.FireAndForget))
                {
                    //还未得到结果
                    return true;
                }

                return false;
            }

            var resultValue = (StackRedis.RedisValue)result;

            if (resultValue.HasValue && resultValue.IsInteger)
            {
                return true;
            }

            return false;
        }
        private bool RemoveNotScript(string fullKey, string region)
        {
            try
            {
                Database.HashDelete(region, fullKey, StackRedis.CommandFlags.FireAndForget);
                return Database.KeyDelete(fullKey);
            }
            catch (Exception ex)
            {
                Log?.LogError("#Method =>RemoveNotScript", ex);
                return false;
            }
        }


        private StackRedis.RedisResult Eval(ScriptType scriptType, StackRedis.RedisKey fullKey, string region = null, StackRedis.RedisValue[] values = null, StackRedis.CommandFlags flags = StackRedis.CommandFlags.None)
        {
            LoadScripts();

            if (!LuaScripts.TryGetValue(scriptType, out StackRedis.LoadedLuaScript script))
            {
                //this.Logger.LogCritical("Something is wrong with the Lua scripts. Seem to be not loaded.");
                ScriptsLoaded = false;
                Log?.LogError("#Method =>Eval", "lua加载失败");
                throw new InvalidOperationException("lua加载失败");
            }

            try
            {
                if (region == null)
                {
                    return Database.ScriptEvaluate(script.Hash, new[] { fullKey }, values, flags);
                }

                return Database.ScriptEvaluate(script.Hash, new[] { fullKey, (StackExchange.Redis.RedisKey)region }, values, flags);
            }
            catch (StackRedis.RedisServerException ex) when (ex.Message.StartsWith("NOSCRIPT", StringComparison.OrdinalIgnoreCase))
            {
                Log?.LogWarning("#Method =>Eval", "服务端脚本不存在, 将尝试创建脚本");
                //this.Logger.LogInfo("Received NOSCRIPT from server. Reloading scripts...");
                LoadScripts();

                // retry
                throw;
            }
        }


        private TValue FromRedisValue(StackRedis.RedisValue value, string valueType)
        {
            if (value.IsNull)
            {
                return default(TValue);
            }

            var typedConverter = ValueConverter as IRedisValueConverter<TValue>;
            if (typedConverter != null)
            {
                return typedConverter.FromRedisValue(value, valueType);
            }

            return ValueConverter.FromRedisValue<TValue>(value, valueType);
        }
        private StackRedis.RedisValue ToRedisValue(TValue value)
        {
            var typedConverter = ValueConverter as IRedisValueConverter<TValue>;
            if (typedConverter != null)
            {
                return typedConverter.ToRedisValue(value);
            }

            return ValueConverter.ToRedisValue(value);
        }

        static object _loadScriptsLock = new object();
        void LoadScripts()
        {
            if (ScriptsLoaded)
            {
                return;
            }

            lock (_loadScriptsLock)
            {
                if (ScriptsLoaded)
                {
                    return;
                }

                //this.Logger.LogInfo("Loading scripts.");

                var putLua = StackRedis.LuaScript.Prepare(ScriptPut);
                var addLua = StackRedis.LuaScript.Prepare(ScriptAdd);
                var updateLua = StackRedis.LuaScript.Prepare(ScriptUpdate);
                var getLua = StackRedis.LuaScript.Prepare(ScriptGet);
                var delLua = StackRedis.LuaScript.Prepare(ScriptDel);

                foreach (var server in Servers)
                {
                    if (server.IsConnected)
                    {
                        LuaScripts[ScriptType.Put] = putLua.Load(server);
                        LuaScripts[ScriptType.Add] = addLua.Load(server);
                        LuaScripts[ScriptType.Update] = updateLua.Load(server);
                        LuaScripts[ScriptType.Get] = getLua.Load(server);
                        LuaScripts[ScriptType.Del] = delLua.Load(server);
                    }
                }

                _scriptInitStatus.Add(Connection?.connectionString, true);
                ScriptsLoaded = true;
            }
        }
    }

    internal enum ScriptType
    {
        Put,
        Add,
        Update,
        Get,
        Del
    }
}
