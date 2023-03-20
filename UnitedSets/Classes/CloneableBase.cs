using System;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning disable CS8604 // Possible null reference argument.
#pragma warning disable CS8603 // Possible null reference return.
#pragma warning disable CS8605 // Unboxing a possibly null value.
#pragma warning disable CS8602 // 
#pragma warning disable CS8601 // Possible null reference assignment.

namespace UnitedSets.Classes {
	public abstract class CloneableBase : ICloneable {
		object ICloneable.Clone() => MemberwiseClone();
		protected virtual void PostClone() { }
		public object DeepClone() {
			var clone = ((ICloneable)this).Clone();
			var props = GetType().GetProperties();
			foreach (var prop in props) {

				if (prop.GetSetMethod() == null)
					continue;
				var val = prop.GetValue(this);
				if (val == null)
					continue;
				if (val is CloneableBase cb)
					prop.SetValue(this, cb.DeepClone());

			}
			PostClone();
			return clone;
		}

	}
}

#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning restore CS8603 //
#pragma warning restore CS8601 //
#pragma warning restore CS8604 //
#pragma warning restore CS8605
#pragma warning restore CS8602
