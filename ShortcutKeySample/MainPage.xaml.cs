using ShortcutKeySample.ViewModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// 空白ページのアイテム テンプレートについては、http://go.microsoft.com/fwlink/?LinkId=234238 を参照してください

namespace ShortcutKeySample
{
    /// <summary>
    /// それ自体で使用できる空白ページまたはフレーム内に移動できる空白ページ。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            this.DataContext = new MainViewModel();
            Dispatcher.AcceleratorKeyActivated += (s, e) =>
            {
                CoreVirtualKeyStates ctrlState=Window.Current.CoreWindow.GetKeyState(VirtualKey.Control);
                CoreVirtualKeyStates aState = Window.Current.CoreWindow.GetKeyState(VirtualKey.A);
                if (ctrlState == (CoreVirtualKeyStates.Down|CoreVirtualKeyStates.Locked) && e.VirtualKey == VirtualKey.A&&e.EventType==CoreAcceleratorKeyEventType.KeyDown)
                {
                    
                    /*Debug.WriteLine("----------------------------------------");
                    Debug.WriteLine("EventType=" + e.EventType);
                    Debug.WriteLine("Handled=" + e.Handled);
                    Debug.WriteLine("KeyStatus=" + e.KeyStatus);
                    Debug.WriteLine("VirtualKey=" + e.VirtualKey);
                 */   
                }

                
            };
            

        }
    }
}
