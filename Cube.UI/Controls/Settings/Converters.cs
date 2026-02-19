namespace Cube.UI.Controls.Settings;

/// <summary>
/// The generic base implementation of a value converter.
/// </summary>
/// <typeparam name="TSource">The source type.</typeparam>
/// <typeparam name="TTarget">The target type.</typeparam>
public abstract class ValueConverter<TSource, TTarget>
    : IValueConverter
{
    /// <summary>
    /// Converts a source value to the target type.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public TTarget? Convert(TSource? value) => Convert(value, null, null);

    /// <summary>
    /// Converts a target value back to the source type.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public TSource? ConvertBack(TTarget? value) => ConvertBack(value, null, null);

    /// <summary>
    /// Modifies the source data before passing it to the target for display in the UI.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="targetType"></param>
    /// <param name="parameter"></param>
    /// <param name="language"></param>
    /// <returns></returns>
    public object? Convert(object? value, Type? targetType, object? parameter, string? language)
    {
        // CastExceptions will occur when invalid value, or target type provided.
        return Convert((TSource?)value, parameter, language);
    }

    /// <summary>
    /// Modifies the target data before passing it to the source object. This method is called only in TwoWay bindings.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="targetType"></param>
    /// <param name="parameter"></param>
    /// <param name="language"></param>
    /// <returns></returns>
    public object? ConvertBack(object? value, Type? targetType, object? parameter, string? language)
    {
        // CastExceptions will occur when invalid value, or target type provided.
        return ConvertBack((TTarget?)value, parameter, language);
    }

    /// <summary>
    /// Converts a source value to the target type.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="parameter"></param>
    /// <param name="language"></param>
    /// <returns></returns>
    protected virtual TTarget? Convert(TSource? value, object? parameter, string? language)
        => throw new NotSupportedException();

    /// <summary>
    /// Converts a target value back to the source type.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="parameter"></param>
    /// <param name="language"></param>
    /// <returns></returns>
    protected virtual TSource? ConvertBack(TTarget? value, object? parameter, string? language)
        => throw new NotSupportedException();
}

/// <summary>
/// The base class for converting instances of type T to object and vice versa.
/// </summary>
public abstract class ToObjectConverter<T>
    : ValueConverter<T?, object?>
{
    protected override object? Convert(T? value, object? parameter, string? language) => value;
    protected override T? ConvertBack(object? value, object? parameter, string? language) => (T?)value;
}

/// <summary>
/// Converts a boolean to and from a visibility value.
/// </summary>
public partial class InverseBooleanConverter
    : ValueConverter<bool, bool>
{
    protected override bool Convert(bool value, object? parameter, string? language) => !value;
    protected override bool ConvertBack(bool value, object? parameter, string? language) => !value;
}

public partial class NullToTrueConverter
    : ValueConverter<object?, bool>
{
    /// <summary>
    /// Determines whether an inverse conversion should take place.
    /// </summary>
    /// <remarks>If set, the value True results in <see cref="Visibility.Collapsed"/>, and false in <see cref="Visibility.Visible"/>.</remarks>
    public bool Inverse { get; set; }

    protected override bool Convert(object? value, object? parameter, string? language) => Inverse ? value != null : value == null;

    protected override object? ConvertBack(bool value, object? parameter, string? language) => null;
}

public partial class StringNullOrWhiteSpaceToTrueConverter
    : ValueConverter<string, bool>
{
    /// <summary>
    /// Determines whether an inverse conversion should take place.
    /// </summary>
    /// <remarks>If set, the value True results in <see cref="Visibility.Collapsed"/>, and false in <see cref="Visibility.Visible"/>.</remarks>
    public bool Inverse { get; set; }

    protected override bool Convert(string? value, object? parameter, string? language)
        => Inverse ? !string.IsNullOrWhiteSpace(value) : string.IsNullOrWhiteSpace(value);

    protected override string ConvertBack(bool value, object? parameter, string? language) => string.Empty;
}
