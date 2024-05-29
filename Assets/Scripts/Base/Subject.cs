using System;
using System.Collections.Generic;

namespace Base
{
    public static class MessageList
    {
        public const string Test = "Test";
        public const string VsTest = "VsTest";
    }

    public interface IObserver
    {
        void OnNotify(string message, object sender, params object[] args);
    }

    public class Subject
    {
        private readonly Dictionary<string, List<ObserverEntry>> _observers =
            new Dictionary<string, List<ObserverEntry>>();
    
        public void RegisterObserver(string message, IObserver observer, object sender = null)
        {
            if (!_observers.ContainsKey(message))
            {
                _observers[message] = new List<ObserverEntry>();
            }
            _observers[message].Add(new ObserverEntry(observer,sender));
        }

        public void RegisterObserver(string[] messages, IObserver observer, object sender = null)
        {
            foreach (var message in messages)
            {
                RegisterObserver(message,observer,sender);
            }
        }

        public void UnregisterObserver(string message, IObserver observer)
        {
            if (_observers.TryGetValue(message, out var observerEntries))
            {
                observerEntries.RemoveAll(entry => entry.Observer.Target == observer);
            }
        }

        public void NotifyObservers(string message, object senderObject, params object[] args)
        {
            if(!_observers.ContainsKey(message))
                return;

            var deadEntries = new List<ObserverEntry>();

            foreach (var entry in _observers[message])
            {
                var observer = entry.Observer.Target as IObserver;
                var sender = entry.Sender.Target;

                if (observer != null)
                {
                    if (entry.Sender.IsAlive)
                    {
                        if (sender == null || sender == senderObject)
                        {
                            observer.OnNotify(message,senderObject,args);
                        }
                    }
                    else if (sender == null)
                    {
                        observer.OnNotify(message, senderObject, args);
                    }
                }
                else
                {
                    deadEntries.Add(entry);
                }

                //Remove Dead Entries
                foreach (var dead in deadEntries)
                {
                    _observers[message].Remove(dead);
                }
            }
        }
    
        public void UnregisterObserver(IObserver observer)
        {
            foreach (var observersValue in _observers.Values)
            {
                observersValue.RemoveAll(entry => entry.Observer.Target == observer);
            }
        }
    
        private class ObserverEntry
        {
            public readonly WeakReference Observer;
            public readonly WeakReference Sender;

            public ObserverEntry(IObserver observer, object sender)
            {
                Observer = new WeakReference(observer);
                Sender = new WeakReference(sender);
            }
        }
    }
}