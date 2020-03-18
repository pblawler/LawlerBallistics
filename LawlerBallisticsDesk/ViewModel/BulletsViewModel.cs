using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using LawlerBallisticsDesk.Classes;
using LawlerBallisticsDesk.Views.Cartridges;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace LawlerBallisticsDesk.ViewModel
{
    public class BulletsViewModel : ViewModelBase
    {
        #region "Relay Commands"
        private RelayCommand _OpenBulletCommand;
        private RelayCommand _AddBulletCommand;
        private RelayCommand _SaveBulletsCommand;
        private RelayCommand _SaveBulletCommand;
        private RelayCommand<System.Windows.Input.KeyEventArgs> _KeyUpCommand;

        public RelayCommand OpenBulletCommand
        {
            get
            {
                return _OpenBulletCommand ?? (_OpenBulletCommand = new RelayCommand(() => OpenBullet()));
            }
        }
        public RelayCommand AddBulletCommand
        {
            get
            {
                return _AddBulletCommand ?? (_AddBulletCommand = new RelayCommand(() => AddBullet()));
            }
        }
        public RelayCommand SaveBulletsCommand
        {
            get
            {
                return _SaveBulletsCommand ?? (_SaveBulletsCommand = new RelayCommand(() => SaveBullets()));
            }
        }
        public RelayCommand SaveBulletCommand
        {
            get
            {
                return _SaveBulletCommand ?? (_SaveBulletCommand = new RelayCommand(() => SaveBullet()));
            }
        }
        public RelayCommand<System.Windows.Input.KeyEventArgs> KeyUpCommand
        {
            get
            {
                return _KeyUpCommand ?? (_KeyUpCommand = new RelayCommand<System.Windows.Input.KeyEventArgs>((X) => KeyUp(X)));
            }
        }
        #endregion

        #region "Private Variables"
        private Bullet _SelectedBullet;
        private frmBullet _frmBullet;
        private List<string> _BulletTypeNames = new List<string>();
        #endregion

        #region "Properties"
        public Bullet SelectedBullet 
        {
            get
            { 
                return _SelectedBullet;
            }
            set
            {
                _SelectedBullet = value;
                RaisePropertyChanged(nameof(SelectedBullet)); 
            }
        }
        public ObservableCollection<Bullet> MyBullets { get { return LawlerBallisticsFactory.MyBullets; } set { LawlerBallisticsFactory.MyBullets = value; RaisePropertyChanged(nameof(MyBullets)); } }
        public BulletShapeEnum BulletTypes { get; set; }
        public List<string> BulletTypeNames { get { return LawlerBallisticsFactory.BulletTypeNames; } }
        #endregion

        #region "Constructor"
        public BulletsViewModel()
        {
            BulletTypes = new BulletShapeEnum();
        }
        #endregion

        #region "Private Routines"
        private void OpenBullet()
        {
            _frmBullet = new frmBullet();
            _frmBullet.Show();
        }
        private void AddBullet()
        {
            if (frmBullet.IsOpen)
            {
                System.Windows.MessageBox.Show("An instance of the add bullet window is currently open.");
                return;
            }
            _SelectedBullet = new Bullet();
            _frmBullet = new frmBullet();
            _frmBullet.DataContext = this;
            _frmBullet.ResizeMode = System.Windows.ResizeMode.NoResize;
            _frmBullet.Show();
        }
        private void SaveBullet()
        {
            bool ls = false;
            foreach (Bullet lItr in MyBullets)
            {
                if (lItr.ID == _SelectedBullet.ID)
                {
                    ls = true;
                    break;
                }
            }
            if (!ls) MyBullets.Add(_SelectedBullet);
            _frmBullet.Close();
            RaisePropertyChanged(nameof(MyBullets));
            RaisePropertyChanged(nameof(SelectedBullet));
        }
        private void SaveBullets()
        {
            LawlerBallisticsFactory.SaveMyBullets();
        }
        private void KeyUp(System.Windows.Input.KeyEventArgs e)
        {
            try
            {
                switch (e.Key)
                {
                    case System.Windows.Input.Key.Delete:
                        string lmsg = "Delete the selected bullet?";
                        string lcaption = "Delete Bullet Data";
                        MessageBoxButtons lbtns = MessageBoxButtons.YesNo;
                        DialogResult lrst = System.Windows.Forms.MessageBox.Show(lmsg, lcaption, lbtns, MessageBoxIcon.Warning);
                        if (lrst == DialogResult.Yes)
                        {
                            foreach (Bullet lc in MyBullets)
                            {
                                if (SelectedBullet.ID == lc.ID)
                                {
                                    MyBullets.Remove(lc);
                                    SelectedBullet = null;
                                    break;
                                }
                            }
                        }
                        break;
                    case System.Windows.Input.Key.OemPlus:
                        AddBullet();
                        break;
                }
            }
            catch
            {

            }
        }
        #endregion
    }
}
