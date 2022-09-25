using System;
using System.Collections.Generic;

namespace Example
{
	internal class Observable<TType>
	{
		public Observable() { }

		public Observable(TType value) => this.value = value;

		//public void DependsOn<TOther>(Observable<TOther> observable) => observable.OnChange += _ => OnChange?.Invoke(this);

		public TType Get() => (TType)this;

		public bool HasValue => value != null;

		public void Set(TType value)
		{
			this.value = value;
			foreach (var sub in subscriptions) sub.Invoke(value);
		}

		public IDisposable Subscribe(Action<TType> subscription) => new Subscription(subscriptions, subscription);

		public static implicit operator TType(Observable<TType> observable)
		{
			return observable.value ?? throw new ArgumentException(
				$"Observable {observable.GetType().FullName} value not set");
		}

		private sealed class Subscriptions : HashSet<Action<TType>> { }

		private sealed class Subscription : IDisposable
		{
			private readonly Subscriptions subscriptions;
			private readonly Action<TType> subscription;

			public Subscription(Subscriptions subscriptions, Action<TType> subscription)
			{
				this.subscriptions = subscriptions;
				this.subscription = subscription;
				this.subscriptions.Add(subscription);
			}

			public void Dispose()
			{
				subscriptions.Remove(subscription);
			}
		}

		private TType? value;
		private readonly Subscriptions subscriptions = new();
	}
}
