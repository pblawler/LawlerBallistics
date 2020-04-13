/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocator xmlns:vm="clr-namespace:LawlerBallisticsDesk"
                           x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"

  You can also use Blend to do all this with the tool's support.
  See http://www.galasoft.ch/mvvm
*/

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using CommonServiceLocator;
using System;

namespace LawlerBallisticsDesk.ViewModel
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    public class ViewModelLocator
    {
        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            ////if (ViewModelBase.IsInDesignModeStatic)
            ////{
            ////    // Create design time view services and models
            ////    SimpleIoc.Default.Register<IDataService, DesignDataService>();
            ////}
            ////else
            ////{
            ////    // Create run time view services and models
            ////    SimpleIoc.Default.Register<IDataService, DataService>();
            ////}

            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<SolutionViewModel>();
            SimpleIoc.Default.Register<CartridgesViewModel>();
            SimpleIoc.Default.Register<GunsViewModel>();
            SimpleIoc.Default.Register<RecipeViewModel>();
            SimpleIoc.Default.Register<BulletsViewModel>();
            SimpleIoc.Default.Register<CasesViewModel>();
            SimpleIoc.Default.Register<PrimersViewModel>();
            SimpleIoc.Default.Register<PowdersViewModel>();
        }

        public MainViewModel Main
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MainViewModel>();
            }
        }
        public SolutionViewModel BC
        {
            get
            {
                return ServiceLocator.Current.GetInstance<SolutionViewModel> (Guid.NewGuid().ToString());
            }
        }
        public CartridgesViewModel CVM
        {
            get
            {
                return ServiceLocator.Current.GetInstance<CartridgesViewModel>();
            }
        }
        public GunsViewModel GUNS
        {
            get
            {
                return ServiceLocator.Current.GetInstance<GunsViewModel>();
            }
        }
        public RecipeViewModel RECIPES
        {
            get
            {
                return ServiceLocator.Current.GetInstance<RecipeViewModel>();
            }
        }
        public BulletsViewModel BVM
        {
            get
            {
                return ServiceLocator.Current.GetInstance<BulletsViewModel>();
            }
        }
        public CasesViewModel CasesVM
        {
            get
            {
                return ServiceLocator.Current.GetInstance<CasesViewModel>();
            }
        }
        public PrimersViewModel PVM
        {
            get
            {
                return ServiceLocator.Current.GetInstance<PrimersViewModel>();
            }
        }
        public PowdersViewModel PDRVM
        {
            get
            {
                return ServiceLocator.Current.GetInstance<PowdersViewModel>();
            }
        }

        public static void Cleanup()
        {
            
            
        }
    }
}