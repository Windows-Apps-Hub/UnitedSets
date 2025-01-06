using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace UnitedSets.Configurations;

/*
Originally I tried doing this with just overloaded functions but to get all the combinations even with heavy default parameters on each you were looking at 8 functions to handle structs + classes ie:
    public static RET_TYPE DoOrThrow<SRC_TYPE, RET_TYPE>(SRC_TYPE? data, Func<SRC_TYPE, RET_TYPE?> dataConvertFunc, Action<SRC_TYPE, RET_TYPE> OnSuccess, Func<SRC_TYPE, bool>? MustBeTrue = null, Func<Exception>? OnFail = null) where SRC_TYPE : class {
with lots of duplicated code.
*/

public class OnNotNull
{
    protected static NullType<T> GetVal<T>(T? val) where T : struct => new NullableValueType<T>(val);
    protected static NullType<T> GetVal<T>(T? val) where T : class => new NullableClassType<T>(val);

    protected class NullableClassType<T>(T? inst) : NullType<T>(inst) where T : class
    {
    }
    protected class NullableValueType<T>(T? value) : NullType<T>(value == null ? default : value.Value) where T : struct
    {
        private readonly bool wasNull = value == null;
        public override bool IsNull() => wasNull;
    }
    protected abstract class NullType<T>(T? notNullValue)
    {
        public virtual bool IsNull() => NotNullValue is null;
        [AllowNull]
        public T NotNullValue { get; set; } = notNullValue;
    }
    protected class MyHandleClass<SRC_TYPE>(SRC_TYPE? value) : OnNotNull<SRC_TYPE>(GetVal(value)) where SRC_TYPE : class;
    protected class MyHandleValueType<SRC_TYPE>(SRC_TYPE? value) : OnNotNull<SRC_TYPE>(GetVal(value)) where SRC_TYPE : struct;
    public static OnNotNull<SRC_TYPE>? Get<SRC_TYPE>(SRC_TYPE? data) where SRC_TYPE : class => new MyHandleClass<SRC_TYPE>(data).NotNull();

    public static OnNotNull<SRC_TYPE>? Get<SRC_TYPE>(SRC_TYPE? data) where SRC_TYPE : struct => new MyHandleValueType<SRC_TYPE>(data).NotNull();
    public class ConvertResult<SRC_TYPE, RESULT_TYPE>(SRC_TYPE src, RESULT_TYPE result)
    {
        //while we could use a generic tuple by doing this we can name the items to make it more clear to callers
        public SRC_TYPE src = src;
        public RESULT_TYPE result = result;
    }
    protected Func<string, Exception> OnFail = (str) => new ArgumentException(str);
}
public abstract class OnNotNull<SRC_TYPE>(OnNotNull.NullType<SRC_TYPE> value) : OnNotNull
{
    public OnNotNull<SRC_TYPE>? NotNull()
    {
        if (Value.IsNull())
            return null;
        return this;
    }
    /// <summary>
    /// Set the exception to throw if the conversion fails or if MustSucceed doesn't succeed
    /// </summary>
    /// <param name="ToGenerate"></param>
    /// <returns></returns>
    public OnNotNull<SRC_TYPE>? IfFailException(Func<string, Exception> ToGenerate)
    {
        OnFail = ToGenerate;
        return this;
    }

    /// <summary>
    /// Do additional validation on the source data if not null (or source and converted result if called after the converter), if it isn't return true it throws
    /// </summary>
    /// <param name="MustBeTrueVal"></param>
    /// <param name="expression"></param>
    /// <returns></returns>
    public OnNotNull<SRC_TYPE>? MustBeTrue(bool MustBeTrueVal, [CallerArgumentExpression(nameof(MustBeTrueVal))] string expression = "")
    {
        if (!MustBeTrueVal)
            throw OnFail($"{expression} was not true");
        return this;
    }
    /// <summary>
    /// Do additional validation on the source data if not null (or source and converted result if called after the converter), if the function doesn't return true it throws
    /// </summary>
    /// <param name="MustBeTrue"></param>
    /// <param name="expression"></param>
    /// <returns></returns>
    public OnNotNull<SRC_TYPE>? MustBeTrue(Func<SRC_TYPE, bool> MustBeTrueFunc, [CallerArgumentExpression(nameof(MustBeTrue))] string expression = "") => MustBeTrue(MustBeTrueFunc(Value.NotNullValue), expression);
    /// <summary>
    /// Can apply a converter to the src data type to get another form
    /// </summary>
    /// <typeparam name="CONVERTED_TYPE"></typeparam>
    /// <param name="Converter"></param>
    /// <returns></returns>
    public OnNotNull<ConvertResult<SRC_TYPE, CONVERTED_TYPE>>? Convert<CONVERTED_TYPE>(Func<SRC_TYPE, CONVERTED_TYPE?> Converter)
    {
        var ret = Converter(Value.NotNullValue) ?? throw OnFail("Converter failed, returned null");
        var newInst = Get(new ConvertResult<SRC_TYPE, CONVERTED_TYPE>(Value.NotNullValue, ret));
        newInst!.OnFail = OnFail;
        return newInst;
    }
    protected NullType<SRC_TYPE> Value = value;

    //OnNotNull<SRC_TYPE>?
    /// <summary>
    /// This should be the last item called, it is what happens when everything else has succeeded.  It does not run the action right away but rather adds it to the list of Delays to be called once that item is ready.
    /// </summary>
    /// <typeparam name="DELAY_ITEM"></typeparam>
    /// <param name="delays"></param>
    /// <param name="action"></param>
    public void DelayAction<DELAY_ITEM>(IList<Action<DELAY_ITEM>> delays, Action<DELAY_ITEM, SRC_TYPE> action)
    {
        delays.Add((tab) => action(tab, Value.NotNullValue));
        //return this;
    }
    //OnNotNull<SRC_TYPE>?
    /// <summary>
    /// This should be the last item called, it is what happens when everything else has succeeded
    /// </summary>
    /// <param name="action"></param>
    public void Action(Action<SRC_TYPE> action)
    {
        action(Value.NotNullValue);
        //return this;
    }
}
