﻿//  <copyright file="DefaultRedisCacheSerializer.cs" project="CodeZero.RedisCache" solution="CodeZero">
//      Copyright (c) 2018 CodeZero Framework.  All rights reserved.
//  </copyright>
//  <author>Nasr Aldin M.</author>
//  <email>nasr2ldin@gmail.com</email>
//  <website>https://nasraldin.com/codezero</website>
//  <github>https://nasraldin.github.io/CodeZero</github>
//  <date>01/01/2018 01:00 AM</date>
using System;
using CodeZero.Dependency;
using CodeZero.Json;
using StackExchange.Redis;

namespace CodeZero.Runtime.Caching.Redis
{
    /// <summary>
    ///     Default implementation uses JSON as the underlying persistence mechanism.
    /// </summary>
    public class DefaultRedisCacheSerializer : IRedisCacheSerializer, ITransientDependency
    {
        /// <summary>
        ///     Creates an instance of the object from its serialized string representation.
        /// </summary>
        /// <param name="objbyte">String representation of the object from the Redis server.</param>
        /// <returns>Returns a newly constructed object.</returns>
        /// <seealso cref="IRedisCacheSerializer.Serialize" />
        public virtual object Deserialize(RedisValue objbyte)
        {
            return JsonSerializationHelper.DeserializeWithType(objbyte);
        }

        /// <summary>
        ///     Produce a string representation of the supplied object.
        /// </summary>
        /// <param name="value">Instance to serialize.</param>
        /// <param name="type">Type of the object.</param>
        /// <returns>Returns a string representing the object instance that can be placed into the Redis cache.</returns>
        /// <seealso cref="IRedisCacheSerializer.Deserialize" />
        public virtual string Serialize(object value, Type type)
        {
            return JsonSerializationHelper.SerializeWithType(value, type);
        }
    }
}