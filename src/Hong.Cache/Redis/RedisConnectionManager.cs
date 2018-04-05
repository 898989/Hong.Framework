using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using static Hong.Common.Extendsion.Guard;
using StackRedis = StackExchange.Redis;

namespace Hong.Cache.Redis
{
    internal class RedisConnectionManager
    {
        static IDictionary<string, StackRedis.ConnectionMultiplexer> connections = new Dictionary<string, StackRedis.ConnectionMultiplexer>();
        static object connectLock = new object();
        
        //private readonly ILogger logger;
        public readonly string connectionString;
        public readonly int DatabaseInsideRedis;
        //private readonly RedisConfiguration configuration;

        public RedisConnectionManager(RedisCacheConfiguration configuration)//, ILoggerFactory loggerFactory)
        {
            NotNull(configuration, nameof(configuration));
            //NotNull(loggerFactory, nameof(loggerFactory));

            connectionString = GetConnectionString(configuration);
            DatabaseInsideRedis = configuration.DataBase;

            //this.configuration = configuration;
            //this.logger = loggerFactory.CreateLogger(this);
        }

        public StackRedis.RedisFeatures Features
        {
            get
            {
                var server = this.Servers.FirstOrDefault(p => p.IsConnected);

                if (server == null)
                {
                    throw new InvalidOperationException("No servers are connected or configured.");
                }

                return server.Features;
                ////return new StackRedis.RedisFeatures(new Version(2, 4));
            }
        }

        public IEnumerable<StackRedis.IServer> Servers
        {
            get
            {
                var endpoints = this.Connect().GetEndPoints();
                foreach (var endpoint in endpoints)
                {
                    var server = this.Connect().GetServer(endpoint);
                    yield return server;
                }
            }
        }

        public StackRedis.IDatabase Database => this.Connect().GetDatabase(this.DatabaseInsideRedis);

        public StackRedis.ISubscriber Subscriber => this.Connect().GetSubscriber();

        public void RemoveConnection()
        {
            lock (connectLock)
            {
                if (connections.TryGetValue(this.connectionString, out StackRedis.ConnectionMultiplexer connection))
                {
                    //this.logger.LogInfo("Removing stale redis connection.");
                    connections.Remove(this.connectionString);
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "nope")]
        public StackRedis.ConnectionMultiplexer Connect()
        {
            if (!connections.TryGetValue(this.connectionString, out StackRedis.ConnectionMultiplexer connection))
            {
                lock (connectLock)
                {
                    if (!connections.TryGetValue(this.connectionString, out connection))
                    {
                        //this.logger.LogInfo("Trying to connect with the following configuration: '{0}'", this.connectionString);
                        //connection = StackRedis.ConnectionMultiplexer.Connect(this.connectionString, new LogWriter(this.logger));
                        connection = StackRedis.ConnectionMultiplexer.Connect(this.connectionString);

                        if (!connection.IsConnected)
                        {
                            connection.Dispose();
                            throw new InvalidOperationException("Connection failed.");
                        }

                        connection.ConnectionRestored += (sender, args) =>
                        {
                            //this.logger.LogInfo(args.Exception, "Connection restored, type: '{0}', failure: '{1}'", args.ConnectionType, args.FailureType);
                        };
                        connection.ConnectionFailed += (object sender, StackRedis.ConnectionFailedEventArgs e) =>
                        {

                        };
                        connection.InternalError += (object sender, StackRedis.InternalErrorEventArgs e) =>
                        {
                        };

                        var endpoints = connection.GetEndPoints();
                        if (!endpoints.Select(p => connection.GetServer(p))
                            .Any(p => !p.IsSlave || p.AllowSlaveWrites))
                        {
                            throw new InvalidOperationException("No writeable endpoint found.");
                        }

                        connection.PreserveAsyncOrder = false;
                        connections.Add(this.connectionString, connection);
                    }
                }
            }

            if (connection == null)
            {
                throw new InvalidOperationException(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "Couldn't establish a connection for '{0}'.",
                        this.connectionString));
            }

            return connection;
        }

        private string GetConnectionString(RedisCacheConfiguration configuration)
        {
            string conString = configuration.ConnectionString;

            if (string.IsNullOrWhiteSpace(configuration.ConnectionString))
            {
                var options = CreateConfigurationOptions(configuration);
                conString = options.ToString();
            }

            return conString;
        }

        private StackRedis.ConfigurationOptions CreateConfigurationOptions(RedisCacheConfiguration configuration)
        {
            var configurationOptions = new StackRedis.ConfigurationOptions()
            {
                AllowAdmin = configuration.AllowAdmin,
                ConnectTimeout = configuration.ConnectionTimeout,
                Password = configuration.Password,
                Ssl = configuration.IsSsl,
                SslHost = configuration.SslHost,
                ConnectRetry = configuration.ConnectRetry,
                AbortOnConnectFail = false
            };

            //foreach (var endpoint in configuration.Endpoints)
            //{
            //    configurationOptions.EndPoints.Add(endpoint.Host, endpoint.Port);
            //}
            configurationOptions.EndPoints.Add(configuration.Host, configuration.Port);

            return configurationOptions;
        }
    }
}