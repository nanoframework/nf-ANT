//
// Copyright (c) 2017 The nanoFramework project contributors
// See LICENSE file in the project root for full license information.
//
using Microsoft.Practices.ServiceLocation;
using nanoFramework.ANT.Services.NanoFrameworkService;
using nanoFramework.ANT.ViewModels;
using nanoFramework.ANT.Views.Config;
using nanoFramework.Tools.Debugger;
using System.Threading.Tasks;
using Template10.Controls;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace nanoFramework.ANT
{
    /// Documentation on APIs used in this page:
    /// https://github.com/Windows-XAML/Template10/wiki

    [Bindable]
    sealed partial class App : Template10.Common.BootStrapper
    {
        public App()
        {
            InitializeComponent();
            SplashFactory = (e) => new Views.Splash(e);

            #region App settings

            //var _settings = SettingsService.Instance;
            //RequestedTheme = _settings.AppTheme;
            //CacheMaxDuration = _settings.CacheMaxDuration;
            //ShowShellBackButton = _settings.UseShellBackButton;
            ShowShellBackButton = false;
            #endregion
        }

        public override async Task OnInitializeAsync(IActivatedEventArgs args)
        {
            if (Window.Current.Content as ModalDialog == null)
            {
                // setup Page keys for navigation on mvvm
                var keys = PageKeys<Pages>();
                PagesHelper.SetupPages(keys);

                // create a new frame 
                var nav = NavigationServiceFactory(BackButton.Attach, ExistingContent.Include);

                // create modal root
                Window.Current.Content = new ModalDialog
                {
                    DisableBackButtonWhenModal = true,                    
                    Content = new Views.Shell(nav),
                    ModalContent = new Views.Busy(),
                };

                var usbClient = CreateUSBDebugClient();
                ServiceLocator.Current.GetInstance<MainViewModel>().UsbDebugService = usbClient;

                var serialClient = CreateSerialDebugClient();
                ServiceLocator.Current.GetInstance<MainViewModel>().SerialDebugService = serialClient;
            }
            await Task.CompletedTask;
        }

        private  INFUsbDebugClientService CreateUSBDebugClient()
        {
            // TODO: check app lifecycle
            var usbDebugClient = PortBase.CreateInstanceForUsb("", App.Current);

            return new NFUsbDebugClientService(usbDebugClient);
        }

        private INFSerialDebugClientService CreateSerialDebugClient()
        {
            // TODO: check app lifecycle
            var serialDebugClient = PortBase.CreateInstanceForSerial("", App.Current);

            return new NFSerialDebugClientService(serialDebugClient);
        }

        public override async Task OnStartAsync(StartKind startKind, IActivatedEventArgs args)
        {
            //// long-running startup tasks go here
            //await Task.Delay(1000);

            NavigationService.Navigate(Pages.MainPage);
            await Task.CompletedTask;
        }
    }
}

