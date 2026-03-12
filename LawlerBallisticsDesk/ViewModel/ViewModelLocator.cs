using Microsoft.Extensions.DependencyInjection;
using System;

namespace LawlerBallisticsDesk.ViewModel
{
    /// <summary>
    /// Service provider for dependency injection
    /// </summary>
    public static class ServiceProviderHolder
    {
        public static IServiceProvider ServiceProvider { get; private set; }

        public static void Initialize(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }
    }

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
            var services = new ServiceCollection();
            services.AddSingleton<MainViewModel>();
            services.AddTransient<SolutionViewModel>();
            services.AddSingleton<CartridgesViewModel>();
            services.AddSingleton<GunsViewModel>();
            services.AddSingleton<RecipeViewModel>();
            services.AddSingleton<BulletsViewModel>();
            services.AddSingleton<CasesViewModel>();
            services.AddSingleton<PrimersViewModel>();
            services.AddSingleton<PowdersViewModel>();

            ServiceProviderHolder.Initialize(services.BuildServiceProvider());
        }

        public MainViewModel Main
        {
            get
            {
                return ServiceProviderHolder.ServiceProvider.GetRequiredService<MainViewModel>();
            }
        }
        public SolutionViewModel BC
        {
            get
            {
                return ServiceProviderHolder.ServiceProvider.GetRequiredService<SolutionViewModel>();
            }
        }
        public CartridgesViewModel CVM
        {
            get
            {
                return ServiceProviderHolder.ServiceProvider.GetRequiredService<CartridgesViewModel>();
            }
        }
        public GunsViewModel GUNS
        {
            get
            {
                return ServiceProviderHolder.ServiceProvider.GetRequiredService<GunsViewModel>();
            }
        }
        public RecipeViewModel RECIPES
        {
            get
            {
                return ServiceProviderHolder.ServiceProvider.GetRequiredService<RecipeViewModel>();
            }
        }
        public BulletsViewModel BVM
        {
            get
            {
                return ServiceProviderHolder.ServiceProvider.GetRequiredService<BulletsViewModel>();
            }
        }
        public CasesViewModel CasesVM
        {
            get
            {
                return ServiceProviderHolder.ServiceProvider.GetRequiredService<CasesViewModel>();
            }
        }
        public PrimersViewModel PVM
        {
            get
            {
                return ServiceProviderHolder.ServiceProvider.GetRequiredService<PrimersViewModel>();
            }
        }
        public PowdersViewModel PDRVM
        {
            get
            {
                return ServiceProviderHolder.ServiceProvider.GetRequiredService<PowdersViewModel>();
            }
        }

        public static void Cleanup()
        {
            
            
        }
    }
}