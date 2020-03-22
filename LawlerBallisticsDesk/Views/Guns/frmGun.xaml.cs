﻿using LawlerBallisticsDesk.ViewModel;
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

namespace LawlerBallisticsDesk.Views
{
    /// <summary>
    /// Interaction logic for frmGun.xaml
    /// </summary>
    public partial class frmGun : Window
    {
        //TODO:  Add a message for when a barrel is selected and listen for it in this form to
        // send the barrelID to the recipes control for this form.
        public frmGun()
        {
            InitializeComponent();
        }

        public void RegClose()
        {
            GunsViewModel lvm;
            lvm = (GunsViewModel)this.DataContext;
            lvm.CloseGunAction = new Action(this.Close);

        }
    }
}
