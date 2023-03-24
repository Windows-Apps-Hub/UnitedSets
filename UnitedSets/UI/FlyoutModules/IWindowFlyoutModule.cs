using System;

namespace UnitedSets.UI.FlyoutModules;

interface IWindowFlyoutModule
{
    void OnActivated();
    event Action RequestClose;
}