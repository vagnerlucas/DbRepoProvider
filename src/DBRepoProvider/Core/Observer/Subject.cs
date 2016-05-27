using System.Collections.Generic;

namespace DBRepoProvider.Core.Observer
{
    public class Subject
    {
        private List<IObserver> _observers = new List<IObserver>();

        internal void Attach(IObserver observer)
        {
            _observers.Add(observer);
        }

        internal void Detach(IObserver observer)
        {
            _observers.Remove(observer);
        }

        internal void NotifyObjects()
        {
            foreach (IObserver observer in _observers)
            {
                observer.Update();
            }
        }
    }
}
