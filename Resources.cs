/*
<Window x:Class="WpfApp4.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
        xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
        xmlns:local="clr-namespace:WpfApp4"
        mc:Ignorable="d"
        Title="MainWindow" Height="450" Width="900">
    <Grid>
        <Grid.RowDefinitions>
            <RowDefinition Height="Auto"/>
            <RowDefinition Height="*"/>
        </Grid.RowDefinitions>

        <StackPanel Orientation="Horizontal" Margin="10">
            <TextBox x:Name="SearchBox" Width="200" TextChanged="SearchBox_TextChanged" Margin="0,0,10,0" ToolTip="Поиск по названию"/>
            <Button Content="Добавить" x:Name="BtnAdd" Click="BtnAdd_Click" Width="100" Margin="0,0,10,0"/>
        </StackPanel>

        <DataGrid x:Name="DStationary" Grid.Row="1" IsReadOnly="True" AutoGenerateColumns="False" Margin="10">
            <DataGrid.Columns>
                <DataGridTextColumn Header="Название" Binding="{Binding ProductName}" Width="*"/>
                <DataGridTextColumn Header="Описание" Binding="{Binding Description}" Width="*"/>
                <DataGridTextColumn Header="Цена" Binding="{Binding Price}" Width="80"/>
                <DataGridTextColumn Header="Артикл" Binding="{Binding Article}" Width="100"/>
                <DataGridTextColumn Header="Количество" Binding="{Binding Quantity}" Width="120"/>
                <DataGridTemplateColumn Width="100">
                    <DataGridTemplateColumn.CellTemplate>
                        <DataTemplate>
                            <Button Content="Редактировать" Click="EditBtn_Click" Margin="2"/>
                        </DataTemplate>
                    </DataGridTemplateColumn.CellTemplate>
                </DataGridTemplateColumn>
                <DataGridTemplateColumn Width="80">
                    <DataGridTemplateColumn.CellTemplate>
                        <DataTemplate>
                            <Button Content="Удалить" Click="DelBtn_Click" Margin="2"/>
                        </DataTemplate>
                    </DataGridTemplateColumn.CellTemplate>
                </DataGridTemplateColumn>
            </DataGrid.Columns>
        </DataGrid>
    </Grid>
</Window>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApp4
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData(string searchText = "")
        {
            var products = StationeryPlusEntities.GetContext().Products.ToList();
            if (!string.IsNullOrWhiteSpace(searchText))
                products = products.Where(p => p.ProductName.ToLower().Contains(searchText.ToLower())).ToList();
            DStationary.ItemsSource = products;
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            LoadData(SearchBox.Text);
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            new AddWindow(null).ShowDialog();
            LoadData(SearchBox.Text);
        }

        private void EditBtn_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button)?.DataContext is Products selectedProduct)
            {
                new AddWindow(selectedProduct).ShowDialog();
                LoadData(SearchBox.Text);
            }
        }

        private void DelBtn_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button)?.DataContext is Products selectedProduct &&
                MessageBox.Show("Удалить продукт?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                StationeryPlusEntities.GetContext().Products.Remove(selectedProduct);
                StationeryPlusEntities.GetContext().SaveChanges();
                LoadData(SearchBox.Text);
            }
        }
    }
}

<Window x:Class="WpfApp4.AddWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
        xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
        xmlns:local="clr-namespace:WpfApp4"
        mc:Ignorable="d"
        Title="AddWindow" Height="450" Width="800">
    <Grid Margin="10">
        <Grid.RowDefinitions>
            <RowDefinition Height="Auto"/>
            <RowDefinition Height="Auto"/>
            <RowDefinition Height="Auto"/>
            <RowDefinition Height="Auto"/>
            <RowDefinition Height="Auto"/>
            <RowDefinition Height="Auto"/>
        </Grid.RowDefinitions>
        <Grid.ColumnDefinitions>
            <ColumnDefinition Width="100"/>
            <ColumnDefinition Width="*"/>
        </Grid.ColumnDefinitions>

        <TextBlock Text="Название:" Grid.Row="0" VerticalAlignment="Center"/>
        <TextBlock Text="Описание:" Grid.Row="1" VerticalAlignment="Center"/>
        <TextBlock Text="Цена:" Grid.Row="2" VerticalAlignment="Center"/>
        <TextBlock Text="Артикл:" Grid.Row="3" VerticalAlignment="Center"/>
        <TextBlock Text="Количество:" Grid.Row="4" VerticalAlignment="Center"/>

        <TextBox Text="{Binding ProductName}" Grid.Column="1" Grid.Row="0" Margin="5"/>
        <TextBox Text="{Binding Description}" Grid.Column="1" Grid.Row="1" Margin="5"/>
        <TextBox Text="{Binding Price}" Grid.Column="1" Grid.Row="2" Margin="5"/>
        <TextBox Text="{Binding Article}" Grid.Column="1" Grid.Row="3" Margin="5"/>
        <TextBox Text="{Binding Quantity}" Grid.Column="1" Grid.Row="4" Margin="5"/>

        <Button Content="Сохранить" Grid.Row="5" Grid.Column="1" Click="BtnSave_Click" Margin="5,10,5,0" HorizontalAlignment="Right" Width="100"/>
    </Grid>
</Window>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WpfApp4
{
    /// <summary>
    /// Логика взаимодействия для AddWindow.xaml
    /// </summary>
    public partial class AddWindow : Window
    {
        private readonly Products _currentProduct;

        public AddWindow(Products product)
        {
            InitializeComponent();
            _currentProduct = product ?? new Products();
            DataContext = _currentProduct;
            Title = product == null ? "Добавить продукт" : "Редактировать продукт";
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            var errors = new StringBuilder();

            if (string.IsNullOrWhiteSpace(_currentProduct.ProductName))
                errors.AppendLine("Введите название");
            if (string.IsNullOrWhiteSpace(_currentProduct.Description))
                errors.AppendLine("Введите описание");
            if (string.IsNullOrWhiteSpace(_currentProduct.Article))
                errors.AppendLine("Введите артикл");
            if (_currentProduct.Quantity < 1 || _currentProduct.Quantity > 5)
                errors.AppendLine("Количество от 1 до 5");
            if (_currentProduct.Price < 1 || _currentProduct.Price > 5)
                errors.AppendLine("Цена от 1 до 5");

            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString(), "Ошибка");
                return;
            }

            try
            {
                if (_currentProduct.ProductID == 0)
                    StationeryPlusEntities.GetContext().Products.Add(_currentProduct);
                StationeryPlusEntities.GetContext().SaveChanges();
                MessageBox.Show("Сохранено");
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }
    }
}

<Application x:Class="WpfApp4.App"
             xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:local="clr-namespace:WpfApp4"
             StartupUri="MainWindow.xaml">
    <Application.Resources>
        <Style TargetType="Window">
            <Setter Property="Background" Value="#F5F5F5"/>
            <Setter Property="FontFamily" Value="Segoe UI"/>
            <Setter Property="FontSize" Value="14"/>
        </Style>

        <Style TargetType="DataGrid">
            <Setter Property="Background" Value="White"/>
            <Setter Property="BorderBrush" Value="#E0E0E0"/>
            <Setter Property="BorderThickness" Value="1"/>
            <Setter Property="RowBackground" Value="White"/>
            <Setter Property="AlternatingRowBackground" Value="#F9F9F9"/>
            <Setter Property="GridLinesVisibility" Value="Horizontal"/>
            <Setter Property="HorizontalGridLinesBrush" Value="#E0E0E0"/>
            <Setter Property="Margin" Value="10"/>
            <Setter Property="AutoGenerateColumns" Value="False"/>
            <Setter Property="IsReadOnly" Value="True"/>
            <Setter Property="ColumnHeaderStyle">
                <Setter.Value>
                    <Style TargetType="DataGridColumnHeader">
                        <Setter Property="Background" Value="#E0E0E0"/>
                        <Setter Property="Padding" Value="8"/>
                        <Setter Property="FontWeight" Value="SemiBold"/>
                    </Style>
                </Setter.Value>
            </Setter>
            <Setter Property="CellStyle">
                <Setter.Value>
                    <Style TargetType="DataGridCell">
                        <Setter Property="Padding" Value="8"/>
                        <Setter Property="BorderThickness" Value="0"/>
                    </Style>
                </Setter.Value>
            </Setter>
        </Style>

        <Style TargetType="Button">
            <Setter Property="Background" Value="#0078D7"/>
            <Setter Property="Foreground" Value="White"/>
            <Setter Property="Padding" Value="10,5"/>
            <Setter Property="Margin" Value="5"/>
            <Setter Property="BorderThickness" Value="0"/>
            <Setter Property="Cursor" Value="Hand"/>
            <Setter Property="Template">
                <Setter.Value>
                    <ControlTemplate TargetType="Button">
                        <Border Background="{TemplateBinding Background}" CornerRadius="5">
                            <ContentPresenter HorizontalAlignment="Center" VerticalAlignment="Center"/>
                        </Border>
                        <ControlTemplate.Triggers>
                            <Trigger Property="IsMouseOver" Value="True">
                                <Setter Property="Background" Value="#005EA6"/>
                            </Trigger>
                            <Trigger Property="IsPressed" Value="True">
                                <Setter Property="Background" Value="#003087"/>
                            </Trigger>
                        </ControlTemplate.Triggers>
                    </ControlTemplate>
                </Setter.Value>
            </Setter>
        </Style>

        <Style TargetType="TextBox">
            <Setter Property="Background" Value="White"/>
            <Setter Property="BorderBrush" Value="#E0E0E0"/>
            <Setter Property="BorderThickness" Value="1"/>
            <Setter Property="Padding" Value="5"/>
            <Setter Property="Margin" Value="5"/>
            <Setter Property="VerticalContentAlignment" Value="Center"/>
            <Setter Property="Template">
                <Setter.Value>
                    <ControlTemplate TargetType="TextBox">
                        <Border Background="{TemplateBinding Background}" 
                                BorderBrush="{TemplateBinding BorderBrush}" 
                                BorderThickness="{TemplateBinding BorderThickness}" 
                                CornerRadius="3">
                            <ScrollViewer x:Name="PART_ContentHost"/>
                        </Border>
                    </ControlTemplate>
                </Setter.Value>
            </Setter>
        </Style>

        <Style TargetType="TextBlock">
            <Setter Property="Margin" Value="5"/>
            <Setter Property="VerticalAlignment" Value="Center"/>
        </Style>
    </Application.Resources>
</Application>
*/