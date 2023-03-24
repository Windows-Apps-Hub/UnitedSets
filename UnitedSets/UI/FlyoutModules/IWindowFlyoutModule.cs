using System;

namespace UnitedSets.Windows.Flyout;

interface IWindowFlyoutModule
{
    void OnActivated();
    event Action RequestClose;
}