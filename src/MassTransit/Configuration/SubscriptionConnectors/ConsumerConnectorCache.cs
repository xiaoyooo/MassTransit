﻿// Copyright 2007-2011 Chris Patterson, Dru Sellers, Travis Smith, et. al.
//  
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed 
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace MassTransit.SubscriptionConnectors
{
    using System;
    using Magnum.Caching;
    using Magnum.Reflection;

    public class ConsumerConnectorCache
    {
        [ThreadStatic]
        static ConsumerConnectorCache _current;

        readonly GenericTypeCache<ConsumerConnector> _connectors;

        ConsumerConnectorCache()
        {
            _connectors = new GenericTypeCache<ConsumerConnector>(typeof (ConsumerConnector<>));
        }

        static ConsumerConnectorCache Instance
        {
            get
            {
                if (_current == null)
                    _current = new ConsumerConnectorCache();

                return _current;
            }
        }

        public static ConsumerConnector GetConsumerConnector<T>()
            where T : class
        {
            return Instance._connectors.Get(typeof (T), _ => new ConsumerConnector<T>());
        }

        public static ConsumerConnector GetConsumerConnector(Type type)
        {
            return Instance._connectors.Get(type, ConsumerConnectorFactory);
        }

        static ConsumerConnector ConsumerConnectorFactory(Type type)
        {
            var args = new object[] {};
            var connector = (ConsumerConnector) FastActivator.Create(typeof (ConsumerConnector<>), new[] {type}, args);

            return connector;
        }
    }
}