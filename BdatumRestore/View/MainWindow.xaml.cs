﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using BdatumRestore.View;
using BdatumRestore.ViewModel;

namespace BdatumRestore.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ListFolder _Update { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            DataContext=new ListFolder(this);
        }

        private void TreeView_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount > 1)
            {
    
                _Update = DataContext as ListFolder;
                _Update.UpdateCommand.Execute(null);
            }
        }

        private void TreeViewItem_Expanded(object sender, RoutedEventArgs e)
        {
                _Update = DataContext as ListFolder;
                _Update.UpdateCommand.Execute(null);
            
        }
       
    }
}
