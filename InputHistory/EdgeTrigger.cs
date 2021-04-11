using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InputHistory {
	class EdgeTrigger<T, U> {
		readonly Predicate<T> Predicate;
		private readonly Action<U> OnTrue, OnFalse;
		bool Level = false;
		public EdgeTrigger(Predicate<T> predicate, Action<U> onTrue, Action<U> onFalse) {
			Predicate = predicate;
			OnTrue = onTrue;
			OnFalse = onFalse;
		}
		public bool Update(T t, U u) {
			bool currentLevel = Predicate(t);
			if(currentLevel == true && Level == false) OnTrue(u);
			else if(currentLevel == false && Level == true) OnFalse(u);
			return Level = currentLevel;
		}
	}
	class EdgeTrigger<T> {
		readonly Predicate<T> Predicate;
		private readonly Action OnTrue, OnFalse;
		bool Level = false;
		public EdgeTrigger(Predicate<T> predicate, Action onTrue, Action onFalse) {
			Predicate = predicate;
			OnTrue = onTrue;
			OnFalse = onFalse;
		}
		public bool Update(T t) {
			bool currentLevel = Predicate(t);
			if(currentLevel == true && Level == false) OnTrue();
			else if(currentLevel == false && Level == true) OnFalse();
			return Level = currentLevel;
		}
	}
}
