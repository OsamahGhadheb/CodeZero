﻿//  <copyright file="CodeZeroRedisCache.cs" project="CodeZero.RedisCache" solution="CodeZero">
//      Copyright (c) 2018 CodeZero Framework.  All rights reserved.
//  </copyright>
//  <author>Nasr Aldin M.</author>
//  <email>nasr2ldin@gmail.com</email>
//  <website>https://nasraldin.com/codezero</website>
//  <github>https://nasraldin.github.io/CodeZero</github>
//  <date>01/01/2018 01:00 AM</date>
using System;
using System.Reflection;
using CodeZero.Domain.Entities;
using CodeZero.Reflection.Extensions;
using StackExchange.Redis;

namespace CodeZero.Runtime.Caching.Redis
{
    /// <summary>
    /// Used to store cache in a Redis server.
    /// </summary>
    public class CodeZeroRedisCache : CacheBase
    {
        private readonly IDatabase _database;
        private readonly IRedisCacheSerializer _serializer;

        /// <summary>
        /// Constructor.
        /// </summary>
        public CodeZeroRedisCache(
            string name, 
            ICodeZeroRedisCacheDatabaseProvider redisCacheDatabaseProvider, 
            IRedisCacheSerializer redisCacheSerializer)
            : base(name)
        {
            _database = redisCacheDatabaseProvider.GetDatabase();
            _serializer = redisCacheSerializer;
        }

        public override object GetOrDefault(string key)
        {
            var objbyte = _database.StringGet(GetLocalizedKey(key));
            return objbyte.HasValue ? Deserialize(objbyte) : null;
        }

        public override void Set(string key, object value, TimeSpan? slidingExpireTime = null, TimeSpan? absoluteExpireTime = null)
        {
            if (value == null)
            {
                throw new CodeZeroException("Can not insert null values to the cache!");
            }

            //TODO: This is a workaround for serialization problems of entities.
            //TODO: Normally, entities should not be stored in the cache, but currently CodeZero.Zero packages does it. It will be fixed in the future.
            var type = value.GetType();
            if (EntityHelper.IsEntity(type) && type.GetAssembly().FullName.Contains("EntityFrameworkDynamicProxies"))
            {
                type = type.GetTypeInfo().BaseType;
            }

            _database.StringSet(
                GetLocalizedKey(key),
                Serialize(value, type),
                absoluteExpireTime ?? slidingExpireTime ?? DefaultAbsoluteExpireTime ?? DefaultSlidingExpireTime
                );
        }

        public override void Remove(string key)
        {
            _database.KeyDelete(GetLocalizedKey(key));
        }

        public override void Clear()
        {
            _database.KeyDeleteWithPrefix(GetLocalizedKey("*"));
        }

        protected virtual string Serialize(object value, Type type)
        {
            return _serializer.Serialize(value, type);
        }

        protected virtual object Deserialize(RedisValue objbyte)
        {
            return _serializer.Deserialize(objbyte);
        }

        protected virtual string GetLocalizedKey(string key)
        {
            return "n:" + Name + ",c:" + key;
        }
    }
}
