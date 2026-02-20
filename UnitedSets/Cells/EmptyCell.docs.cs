using WindowHoster;

namespace UnitedSets.Cells;

partial class EmptyCell
{
    /// <summary>
    /// Removes this cell and replace with a ContainerCell with <paramref name="amount"/> EmptyCell and given <paramref name="orientation"/>
    /// </summary>
    /// <param name="amount"></param>
    /// <param name="orientation"></param>
    /// <exception cref="InvalidOperationException">
    /// Throws if this cell is in an invalid state.
    /// </exception>
    public partial void Split(int amount, Orientation orientation);
    /// <summary>
    /// Removes this cell and replace with a WindowCell with given <paramref name="window"/>
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Throws if this cell is in an invalid state.
    /// </exception>
    public partial void RegisterWindow(RegisteredWindow window);
}
