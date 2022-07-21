# Lua Automation IDE

### Roadmap

- Error Logging Function
- Color coding in console
- File Lua script class
- Ability to accept a file as a startup parameter to load
- More example programs
- Filter search on programs dialog

#### Filter Code

``` xml
<UserControl x:Class="Avalon.Controls.AliasList"
             xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
             xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:ui="http://schemas.modernwpf.com/2019"
             xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
             xmlns:models="clr-namespace:Avalon.Common.Models;assembly=Avalon.Common"
             xmlns:local="clr-namespace:Avalon.Controls"
             xmlns:utilities="clr-namespace:Avalon.Utilities"
             mc:Ignorable="d" ui:ThemeManager.RequestedTheme="Dark" 
             d:DesignHeight="450" d:DesignWidth="800"
             Loaded="AliasList_OnLoaded"
             Unloaded="AliasList_OnUnloaded">
    <Grid>
        <Grid.Resources>
            <ObjectDataProvider x:Key="ExecuteTypes"
                                MethodName="GetValuesAndDescriptions"
                                ObjectType="utilities:EnumUtility">
                <ObjectDataProvider.MethodParameters>
                    <x:TypeExtension TypeName="models:ExecuteType" />
                </ObjectDataProvider.MethodParameters>
            </ObjectDataProvider>
        </Grid.Resources>

        <Grid.RowDefinitions>
            <RowDefinition Height="Auto"></RowDefinition>
            <RowDefinition Height="*"></RowDefinition>
        </Grid.RowDefinitions>

        <StackPanel Grid.Row="0" Orientation="Horizontal" HorizontalAlignment="Right">
            <CheckBox x:Name="CheckBoxAliasesEnabled" Content="Enabled" >
            </CheckBox>
            <Grid Margin="0,10,5,10">
                <Grid.ColumnDefinitions>
                    <ColumnDefinition />
                    <ColumnDefinition />
                </Grid.ColumnDefinitions>
                <Label Grid.Column="0" Padding="5,5,10,0">Filter:</Label>
                <local:SearchBox x:Name="TextFilter" Grid.Column="1" Width="200"
                                 TextChanged="TextFilter_TextChanged"
                                 FontSize="14">
                </local:SearchBox>
            </Grid>
        </StackPanel>

        <DataGrid x:Name="DataList" x:FieldModifier="public" RowHeight="10"
                  Grid.Row="1" HeadersVisibility="Column" BorderBrush="Gray" BorderThickness="1"
                  Margin="5,0,5,5"
                  CanUserAddRows="True"
                  AutoGenerateColumns="False"
                  GridLinesVisibility="All"
                  EnableColumnVirtualization="True"
                  EnableRowVirtualization="True"
                  VirtualizingStackPanel.IsVirtualizing="True"
                  VirtualizingPanel.VirtualizationMode="Recycling">
            <DataGrid.Resources>
                <ResourceDictionary>
                    <ResourceDictionary.MergedDictionaries>
                        <ResourceDictionary Source="/Resources/DataGridStyles.xaml" />
                    </ResourceDictionary.MergedDictionaries>
                </ResourceDictionary>
            </DataGrid.Resources>
            <DataGrid.Columns>
                <DataGridTemplateColumn>
                    <DataGridTemplateColumn.CellTemplate>
                        <DataTemplate>
                            <Button Click="ButtonEdit_OnClick" ui:ControlHelper.CornerRadius="0" Margin="0,0,0,0" HorizontalAlignment="Stretch">Edit</Button>
                        </DataTemplate>
                    </DataGridTemplateColumn.CellTemplate>
                </DataGridTemplateColumn>
                <DataGridTextColumn Header="Name" Binding="{Binding AliasExpression,UpdateSourceTrigger=PropertyChanged}" />
                <DataGridTextColumn Header="Command" Binding="{Binding Command,UpdateSourceTrigger=PropertyChanged}" Width="15*">
                    <DataGridTextColumn.ElementStyle>
                        <Style TargetType="{x:Type TextBlock}" BasedOn="{StaticResource {x:Type TextBlock}}">
                            <Setter Property="Padding" Value="6"></Setter>
                            <Setter Property="TextTrimming" Value="CharacterEllipsis"/>
                        </Style>
                    </DataGridTextColumn.ElementStyle>
                </DataGridTextColumn>
                <DataGridCheckBoxColumn Header="Enabled" Binding="{Binding Enabled,UpdateSourceTrigger=PropertyChanged}"></DataGridCheckBoxColumn>
                <DataGridTextColumn Header="Character" Binding="{Binding Character,UpdateSourceTrigger=PropertyChanged}" />
                <DataGridTextColumn Header="Group" Binding="{Binding Group,UpdateSourceTrigger=PropertyChanged}" />
                <DataGridComboBoxColumn Header="Execute As" 
                                        ItemsSource="{Binding Source={StaticResource ExecuteTypes}}" 
                                        SelectedValuePath="Value" DisplayMemberPath="Description" SelectedValueBinding="{Binding ExecuteAs, UpdateSourceTrigger=PropertyChanged}">
                </DataGridComboBoxColumn>
                <DataGridCheckBoxColumn Header="Lock" Binding="{Binding Lock,UpdateSourceTrigger=PropertyChanged}"></DataGridCheckBoxColumn>
                <DataGridTextColumn Header="Count" Binding="{Binding Count,UpdateSourceTrigger=PropertyChanged}" IsReadOnly="True"></DataGridTextColumn>
            </DataGrid.Columns>
        </DataGrid>
    </Grid>
</UserControl>
```

``` csharp
/*
 * Avalon Mud Client
 *
 * @project lead      : Blake Pell
 * @website           : http://www.blakepell.com
 * @copyright         : Copyright (c), 2018-2021 All rights reserved.
 * @license           : MIT
 */

using Avalon.Common.Interfaces;
using Avalon.Common.Utilities;
using ModernWpf;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Threading;

namespace Avalon.Controls
{
    /// <summary>
    /// Base class with shared functionality for Editor controls that implement a <see cref="DataGrid"/>
    /// and some kind of entry panel for each item.
    /// </summary>
    /// <remarks>
    /// In order to hide the Generic nature of this from the XAML which does not support Generics we'll have
    /// to have a class inherit from this specifying the generic type and then a class that inherits from that
    /// one that will inherit it and not specify the type, effectively making the XAML think it's a regular
    /// old class with a generic declaration.  Seems hacky, but works. ¯\_(ツ)_/¯
    /// </remarks>
    public class EditorControlBase<T> : UserControl, IShellControl
    {
        /// <summary>
        /// Timer that sets the delay on your filtering TextBox.
        /// </summary>
        private DispatcherTimer _typingTimer;

        /// <summary>
        /// A reference to the parent window.
        /// </summary>
        public Shell ParentWindow { get; set; }

        /// <summary>
        /// A reference to the <see cref="DataGrid"/> for the editor control.
        /// </summary>
        public DataGrid DataList { get; set; }

        /// <summary>
        /// A reference to the <see cref="SearchBox"/> control if one exits on the form.  If one does
        /// not this will be null.
        /// </summary>
        public SearchBox SearchBox { get; set; }

        public static readonly DependencyProperty SourceListProperty = DependencyProperty.Register(
            "PropertyType", typeof(FullyObservableCollection<T>), typeof(EditorControlBase<T>), new PropertyMetadata(new FullyObservableCollection<T>()));

        /// <summary>
        /// The source generic FullyObservableCollection list.
        /// </summary>
        public FullyObservableCollection<T> SourceList
        {
            get => (FullyObservableCollection<T>)GetValue(SourceListProperty);
            set => SetValue(SourceListProperty, value);
        }

        public EditorControlBase(FullyObservableCollection<T> sourceList)
        {
            this.SourceList = sourceList;
            this.Loaded += EditorControlBase_Loaded;
            this.Unloaded += EditorControlBase_Unloaded;

            _typingTimer = new DispatcherTimer();
            _typingTimer.Tick += this._typingTimer_Tick;
        }

        private void EditorControlBase_Loaded(object sender, RoutedEventArgs e)
        {
            // Find all of the controls we need to exist from the child control.
            this.ParentWindow = this.FindAscendant<Shell>();
            this.DataList = this.FindDescendant<DataGrid>();
            this.SearchBox = this.FindDescendant<SearchBox>();

            // We had to find this before wiring it up.
            this.SearchBox.TextChanged += SearchBoxFilter_TextChanged;

            this.FocusFilter();

            // Load the variable list the first time that it's requested.
            DataList.ItemsSource = new ListCollectionView(this.SourceList)
            {
                Filter = Filter
            };

            DataList.SelectedItem = null;

            var win = this.FindAscendant<Shell>();

            if (win != null)
            {
                win.StatusBarRightText = $"{this.SourceList.Count.ToString()} items listed.";
            }
        }

        /// <summary>
        /// Unwire events, cleanup resources and release references.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EditorControlBase_Unloaded(object sender, RoutedEventArgs e)
        {
            this.Unbind();

            if (typeof(T).GetInterfaces().Contains(typeof(IModelInfo)))
            {
                // Cleanup any items from the source list if they are empty (and that object implements ModelInfo).
                // This will allow the model to tell us if it's considered empty (as an example, if a Variable doesn't
                // have a key I consider it empty (even if other properties are there).
                for (int i = this.SourceList.Count - 1; i >= 0; i--)
                {
                    if (((IModelInfo)this.SourceList[i]).IsEmpty())
                    {
                        this.SourceList.RemoveAt(i);
                    }
                }
            }

            // Unsubscribe all events we manually subscribed to.
            _typingTimer.Stop();
            _typingTimer.Tick -= this._typingTimer_Tick;
            this.Loaded -= EditorControlBase_Loaded;
            this.Unloaded -= EditorControlBase_Unloaded;
            this.SearchBox.TextChanged -= SearchBoxFilter_TextChanged;

            this.ParentWindow = null;
            this.DataList = null;
            this.SearchBox = null;
            this.SourceList = null;
            _typingTimer = null;
        }

        /// <summary>
        /// The typing delay timer's tick that will refresh the filter after 300ms.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks>
        /// It's important to stop this timer when this fires so that it doesn't continue
        /// to fire until it's needed again.
        /// </remarks>
        private void _typingTimer_Tick(object sender, EventArgs e)
        {
            _typingTimer.Stop();
            ((ListCollectionView)DataList?.ItemsSource)?.Refresh();
        }

        /// <summary>
        /// The filter's text changed event that will setup the delay timer and effective callback.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SearchBoxFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            _typingTimer.Stop();
            _typingTimer.Interval = TimeSpan.FromMilliseconds(300);
            _typingTimer.Start();
        }

        /// <summary>
        /// Removes and cleans up the binding if one exists on the DataGrid.
        /// </summary>
        public void Unbind()
        {
            if (this.DataList == null)
            {
                return;
            }

            if (DataList.ItemsSource is ListCollectionView lcv)
            {
                // Detach, clear the value and remove the reference.  I've read in a few places doing so
                // helps with the garbage collector cleaning this stuff up.
                lcv.DetachFromSourceCollection();
                DataList.ClearValue(ItemsControl.ItemsSourceProperty);
                DataList.ItemsSource = null;
            }
        }

        /// <summary>
        /// Reloads the DataList's ItemSource if it's changed.
        /// </summary>
        public void Reload()
        {
            this.Unbind();

            DataList.ItemsSource = new ListCollectionView(this.SourceList)
            {
                Filter = Filter
            };

            DataList.Items.Refresh();
        }

        /// <summary>
        /// Sets the focus onto the filter text box.
        /// </summary>
        public void FocusFilter()
        {
            Dispatcher.BeginInvoke(
                DispatcherPriority.ContextIdle,
                new Action(() => this.SearchBox.Focus()));
        }

        /// <summary>
        /// The filter that runs when someone types in the search box.  This should be overriden to
        /// meet the requirements of each control.  If not overriden true is always returned.
        /// </summary>
        /// <param name="item"></param>
        public virtual bool Filter(object item)
        {
            return true;
        }

        public virtual void PrimaryButtonClick()
        {
        }

        public virtual void SecondaryButtonClick()
        {
        }
    }

}
```
