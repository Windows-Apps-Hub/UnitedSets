using System;

namespace UnitedSets.Configurations;
public abstract class CloneableBase : ICloneable
{
    object ICloneable.Clone() => MemberwiseClone();
    protected virtual void PostClone() { }
    public object DeepClone()
    {
        var clone = ((ICloneable)this).Clone();
        var props = GetType().GetProperties();
        foreach (var prop in props)
        {

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
