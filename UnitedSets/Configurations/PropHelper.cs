using System;
using EasyCSharp;
using System.Collections;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
#pragma warning disable CS8604 // Possible null reference argument.
#pragma warning disable CS8602 // 
namespace UnitedSets.Configurations;
public class PropHelper
{
    private static ConcurrentDictionary<Type, ConcurrentDictionary<string, PropertyInfo>> type_to_properties = new();
    private static ConcurrentDictionary<string, PropertyInfo> GetTypeDictionary(Type t)
    {
        if (!type_to_properties.TryGetValue(t, out var dict))
        {
            dict = new();
            AddPropsToDict(dict, t);
            type_to_properties.TryAdd(t, dict);

        }
        return dict;
    }
    private static void AddPropsToDict(ConcurrentDictionary<string, PropertyInfo> dict, Type t)
    {
        var plist = from prop in t.GetRuntimeProperties() where prop.CanRead select prop;
        foreach (var prop in plist)
            dict[prop.Name] = prop;
    }
    public static T CopyNotNullPropertiesTo<T>(T source, T dest, bool recurse = false)
    {
        DoPropertyCopy(typeof(T), source, dest, true, recurse);
        return dest;
    }
    public static T UnsetDstPropertiesEqualToSrcOrEmptyCollections<T>(T source, T dest, bool recurse = false)
    {
        DoPropertyAction(typeof(T), source, dest, (val, dest_instance, dst_prop) =>
        {
            if (!dst_prop.CanWrite)
                return;
            object? dst_val = null;
            if (val == null)
            {
                dst_val = dst_prop.GetValue(dest_instance);
                if (dst_val is not ICollection col2 || col2.Count != 0)
                    return;
            }
            var dtype = dst_prop.GetType();
            if (dtype.IsClass == false && (dtype.IsGenericType == false || dtype.GetGenericTypeDefinition() == typeof(Nullable<>)))
                return;

            if (dst_val == null)
                dst_val = dst_prop.GetValue(dest_instance);


            if (val.Equals(dst_val) || dst_val is ICollection col && col.Count == 0)
                dst_prop.SetValue(dest_instance, null);

        }, recurse);
        return dest;
    }
    private static void DoPropertyCopy<S, D>(Type common_type, S source, D dest, bool not_null_only, bool recurse)
    {
        DoPropertyAction(common_type, source, dest, (val, dest_instance, dst_prop) =>
        {
            if (!dst_prop.CanWrite)
                return;
            if (not_null_only && val == null)
                return;
            dst_prop.SetValue(dest_instance, val);

        }, recurse);
    }
    private delegate void PropAction(object? src_val, object dest_instance, PropertyInfo dst_prop);

    private static bool ShouldRecurseType(Type srcType)
    {
        if (srcType.Equals(typeof(string)) || srcType.IsArray)
            return false;

        if (srcType.Namespace?.StartsWith("UnitedSets.Classes.PreservedDataClasses") != true)
            return false;

        if (srcType.IsClass)
            return true;

        if (srcType.IsPrimitive == false && srcType.IsEnum == false)
        {
            if (srcType.IsGenericType)
            {
                var underType = Nullable.GetUnderlyingType(srcType);
                if (underType != null)
                    return ShouldRecurseType(underType);
            }
            return true;
        }
        return false;
    }
    private static void DoPropertyAction<S, D>(Type common_type, S source, D dest, PropAction onProp, bool recurse = false)
    {
        var common_dict = GetTypeDictionary(common_type ?? source.GetType());
        foreach (PropertyInfo prop in common_dict.Values)
        {
            PropertyInfo src_prop = prop;
            PropertyInfo dst_prop = prop;
            var val = src_prop.GetValue(source);
            if (recurse && val != null)
            {
                var srcType = src_prop.PropertyType;
                if (ShouldRecurseType(srcType) && dst_prop.CanRead)
                {

                    var destVal = dst_prop.GetValue(dest);
                    if (destVal != null && destVal.Equals(val) == false)
                    {
                        DoPropertyAction(srcType, val, destVal, onProp, true);//hacky but works for this, recursion is probably a bad idea outside of our few use cases
                        continue;
                    }

                }
            }


            onProp(val, dest, dst_prop);

        }
    }
}

#pragma warning restore CS8604 //
#pragma warning restore CS8602
