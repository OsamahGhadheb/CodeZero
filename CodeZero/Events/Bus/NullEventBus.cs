﻿//  <copyright file="NullEventBus.cs" project="CodeZero" solution="CodeZero">
//      Copyright (c) 2018 CodeZero Framework.  All rights reserved.
//  </copyright>
//  <author>Nasr Aldin M.</author>
//  <email>nasr2ldin@gmail.com</email>
//  <website>https://nasraldin.com/codezero</website>
//  <github>https://nasraldin.github.io/CodeZero</github>
//  <date>01/01/2018 01:00 AM</date>
using System;
using System.Threading.Tasks;
using CodeZero.Events.Bus.Factories;
using CodeZero.Events.Bus.Handlers;
using CodeZero.Utils.Etc;

namespace CodeZero.Events.Bus
{
    /// <summary>
    /// An event bus that implements Null object pattern.
    /// </summary>
    public sealed class NullEventBus : IEventBus
    {
        /// <summary>
        /// Gets single instance of <see cref="NullEventBus"/> class.
        /// </summary>
        public static NullEventBus Instance { get { return SingletonInstance; } }
        private static readonly NullEventBus SingletonInstance = new NullEventBus();

        private NullEventBus()
        {
        }

        /// <inheritdoc/>
        public IDisposable Register<TEventData>(Action<TEventData> action) where TEventData : IEventData
        {
            return NullDisposable.Instance;
        }

        /// <inheritdoc/>
        public IDisposable AsyncRegister<TEventData>(Func<TEventData, Task> action) where TEventData : IEventData
        {
            return NullDisposable.Instance;
        }

        /// <inheritdoc/>
        public IDisposable Register<TEventData>(IEventHandler<TEventData> handler) where TEventData : IEventData
        {
            return NullDisposable.Instance;
        }

        /// <inheritdoc/>
        public IDisposable AsyncRegister<TEventData>(IAsyncEventHandler<TEventData> handler) where TEventData : IEventData
        {
            return NullDisposable.Instance;
        }

        /// <inheritdoc/>
        public IDisposable Register<TEventData, THandler>()
            where TEventData : IEventData
            where THandler : IEventHandler, new()
        {
            return NullDisposable.Instance;
        }

        /// <inheritdoc/>
        public IDisposable Register(Type eventType, IEventHandler handler)
        {
            return NullDisposable.Instance;
        }

        /// <inheritdoc/>
        public IDisposable Register<TEventData>(IEventHandlerFactory handlerFactory) where TEventData : IEventData
        {
            return NullDisposable.Instance;
        }

        /// <inheritdoc/>
        public IDisposable Register(Type eventType, IEventHandlerFactory handlerFactory)
        {
            return NullDisposable.Instance;
        }

        /// <inheritdoc/>
        public void Unregister<TEventData>(Action<TEventData> action) where TEventData : IEventData
        {
        }

        /// <inheritdoc/>
        public void AsyncUnregister<TEventData>(Func<TEventData, Task> action) where TEventData : IEventData
        {
        }

        /// <inheritdoc/>
        public void Unregister<TEventData>(IEventHandler<TEventData> handler) where TEventData : IEventData
        {
        }

        /// <inheritdoc/>
        public void AsyncUnregister<TEventData>(IAsyncEventHandler<TEventData> handler) where TEventData : IEventData
        {
        }

        /// <inheritdoc/>
        public void Unregister(Type eventType, IEventHandler handler)
        {
        }

        /// <inheritdoc/>
        public void Unregister<TEventData>(IEventHandlerFactory factory) where TEventData : IEventData
        {
        }

        /// <inheritdoc/>
        public void Unregister(Type eventType, IEventHandlerFactory factory)
        {
        }

        /// <inheritdoc/>
        public void UnregisterAll<TEventData>() where TEventData : IEventData
        {
        }

        /// <inheritdoc/>
        public void UnregisterAll(Type eventType)
        {
        }

        /// <inheritdoc/>
        public void Trigger<TEventData>(TEventData eventData) where TEventData : IEventData
        {
        }

        /// <inheritdoc/>
        public void Trigger<TEventData>(object eventSource, TEventData eventData) where TEventData : IEventData
        {
        }

        /// <inheritdoc/>
        public void Trigger(Type eventType, IEventData eventData)
        {
        }

        /// <inheritdoc/>
        public void Trigger(Type eventType, object eventSource, IEventData eventData)
        {
        }

        /// <inheritdoc/>
        public Task TriggerAsync<TEventData>(TEventData eventData) where TEventData : IEventData
        {
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public Task TriggerAsync<TEventData>(object eventSource, TEventData eventData) where TEventData : IEventData
        {
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public Task TriggerAsync(Type eventType, IEventData eventData)
        {
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public Task TriggerAsync(Type eventType, object eventSource, IEventData eventData)
        {
            return Task.CompletedTask;
        }
    }
}
