using System;
using Get.Data.Helpers;
using Get.UI.Data;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;
using Get.Symbols;
using Microsoft.UI.Xaml.Shapes;
using Microsoft.UI;
using Microsoft.UI.Xaml.Media;
using Windows.ApplicationModel.DataTransfer;
using Get.Data.Properties;
using UnitedSets.Cells;
using UnitedSets.Apps;

namespace UnitedSets.UI.Controls.Cells;
using static Get.UI.Data.QuickCreate;
public partial class EmptyCellVisualizer(EmptyCell emptyCell) : TemplateControl<Grid>
{
    protected override void Initialize(Grid rootElement)
    {
        int splitCount = 2;
        rootElement.AllowDrop = true;
        rootElement.DragOver += OnDragOver;
        rootElement.Drop += emptyCell.OnItemDrop;
        rootElement.Children.Add(new ScrollViewer
        {
            HorizontalScrollMode = ScrollMode.Auto,
            VerticalScrollMode = ScrollMode.Auto,
            HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
            ZoomMode = ZoomMode.Enabled,
            MaxZoomFactor = 1.5f,
            MinZoomFactor = 0.1f,
            Content = new OrientedStack(Orientation.Vertical, spacing: 16)
            {
                Margin = new(0, 0, 0, -180), // oriented stack is questionable its size
                HorizontalAlignment = HorizontalAlignment.Center,
                Children =
                {
                    new TextBlock {
                        HorizontalAlignment = HorizontalAlignment.Center,
                        FontSize = 20,
                    }.AssignTo(out var hintTb),
                    new TextBlock {
                        HorizontalAlignment = HorizontalAlignment.Center,
                        FontSize = 16,
                        Text = "or"
                    },
                    new Button
                    {
                        HorizontalAlignment = HorizontalAlignment.Center,
                        Content = new StackPanel
                        {
                            Orientation = Orientation.Horizontal,
                            Children =
                            {
                                new SymbolExIcon { SymbolEx = SymbolEx.GripperBarHorizontal },
                                new TextBlock { Margin = new(4, 0, 4, 0), Text = "Split Horizontally" },
                                new SymbolExIcon { SymbolEx = SymbolEx.GripperBarHorizontal },
                            }
                        }
                    }.WithCustomCode(
                        x => x.Click += delegate
                        {
                            emptyCell.Split(splitCount, Orientation.Vertical); // intentionally swap orientation
                        }
                    ),
                    new Button
                    {
                        HorizontalAlignment = HorizontalAlignment.Center,
                        Content = new StackPanel
                        {
                            Orientation = Orientation.Horizontal,
                            Children =
                            {
                                new SymbolExIcon { SymbolEx = SymbolEx.GripperBarVertical },
                                new TextBlock { Margin = new(4, 0, 4, 0), Text = "Split Vertically" },
                                new SymbolExIcon { SymbolEx = SymbolEx.GripperBarVertical },
                            }
                        }
                    }.WithCustomCode(
                        x => x.Click += delegate
                        {
                            emptyCell.Split(splitCount, Orientation.Horizontal); // intentionally swap orientation
                        }
                    ),
                    new OrientedStack(Orientation.Horizontal, spacing: 8)
                    {
                        HorizontalAlignment = HorizontalAlignment.Center,
                        Children =
                        {
                            new TextBlock { VerticalAlignment = VerticalAlignment.Center, Text = "Number of Cells to Split:" },
                            new Button
                            {
                                VerticalAlignment = VerticalAlignment.Center,
                                Content = new SymbolIcon(Symbol.Add),
                                Padding = new(5)
                            }.AssignTo(out var plusbtn),
                            new TextBlock { VerticalAlignment = VerticalAlignment.Center, Text = splitCount.ToString() }
                            .AssignTo(out var splitCountDisplay),
                            new Button
                            {
                                VerticalAlignment = VerticalAlignment.Center,
                                Content = new SymbolIcon(Symbol.Remove),
                                Padding = new(5)
                            }.AssignTo(out var minusbtn),
                        }
                    }
                }
            }.WithCustomCode(x =>
            {
                x.DragOver += OnDragOver;
                x.Drop += emptyCell.OnItemDrop;
            })

        }.WithCustomCode(x => Canvas.SetZIndex(x, 1)));
        Rectangle rect;
        var transparent = Solid(Colors.Transparent);
        rootElement.Children.Add(rect = new Rectangle
        {
            Margin = new(8),
            RadiusX = 8,
            RadiusY = 8,
            StrokeDashCap = PenLineCap.Flat,
            StrokeDashOffset = 1.5,
            StrokeDashArray = [3],
            Stroke = Solid(Colors.Gray),
            StrokeThickness = 3
        });
        var layerBrushProp = ThemeResources.Get<Brush>("LayerFillColorDefaultBrush", this);
        emptyCell.HoverEffectProperty.ApplyAndRegisterForNewValue((_, hovering) =>
        {
            void Act()
            {
                if (hovering)
                {
                    rect.Fill = layerBrushProp.CurrentValue;
                    hintTb.Text = "Release mouse to drop window";
                }
                else
                {
                    rect.Fill = transparent;
                    hintTb.Text = "Hold CTRL and Drag Window Here";
                }
            }
            if (DispatcherQueue.HasThreadAccess)
                Act();
            else
                DispatcherQueue.TryEnqueue(Act);

        });
        plusbtn.Click += delegate
        {
            splitCount++;
            splitCountDisplay.Text = splitCount.ToString();
        };
        minusbtn.Click += delegate
        {
            if (splitCount <= 2) return;
            splitCount--;
            splitCountDisplay.Text = splitCount.ToString();
        };
    }
    public void OnDragOver(object? _, DragEventArgs e)
    {
        // There MUST BE NO SUBCELL AND CURRNETCELL
        if (!e.DataView.Properties.ContainsKey(Constants.UnitedSetsTabWindowDragProperty)) return;
        e.AcceptedOperation = DataPackageOperation.Move;
    }

}
