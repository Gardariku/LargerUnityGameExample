using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Common.DataStructures
{
    public interface IEventSubscriber { }
    
    public class EventBus
    {
        private Dictionary<Type, List<IEventSubscriber>> _subscribers = new ();

        private Dictionary<Type, List<Type>> _cashedSubscriberTypes = new ();
        private List<Type> _typesToRemove = new ();

        public void Subscribe(IEventSubscriber subscriber)
        {
            List<Type> subscriberTypes = GetSubscriberTypes(subscriber);
            foreach (Type t in subscriberTypes)
            {
                if (!_subscribers.ContainsKey(t))
                    _subscribers[t] = new List<IEventSubscriber>();
                _subscribers[t].Add(subscriber);
            }
        }
        
        public List<Type> GetSubscriberTypes(IEventSubscriber eventSubscriber)
        {
            Type type = eventSubscriber.GetType();
            
            if (_cashedSubscriberTypes.ContainsKey(type))
                return _cashedSubscriberTypes[type];
            
            List<Type> subscriberTypes = type
                .GetInterfaces()
                .Where(it =>
                    typeof(IEventSubscriber).IsAssignableFrom(it) &&
                    it != typeof(IEventSubscriber))
                .ToList();
            
            _cashedSubscriberTypes[type] = subscriberTypes;
            return subscriberTypes;
        }
        
        public void RaiseEvent<TSubscriber>(Action<TSubscriber> action) 
            where TSubscriber : IEventSubscriber
        {
            if (!_subscribers.TryGetValue(typeof(TSubscriber), out var subscribers))
                return;
            
            foreach (IEventSubscriber subscriber in subscribers)
            {
                try
                {
                    action.Invoke((TSubscriber)subscriber);
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }
            }
        }
        
        public void Unsubscribe(IEventSubscriber subscriber)
        {
            List<Type> subscriberTypes = GetSubscriberTypes(subscriber);
            foreach (Type t in subscriberTypes)
            {
                if (_subscribers.ContainsKey(t))
                    _typesToRemove.Add(t);
            }

            foreach (var t in _typesToRemove)
            {
                _subscribers[t].Remove(subscriber);
            }
            _typesToRemove.Clear();
        }
    }
}