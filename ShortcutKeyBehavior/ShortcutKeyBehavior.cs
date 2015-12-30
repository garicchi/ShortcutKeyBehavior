using Microsoft.Xaml.Interactivity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;

namespace ShortcutKey
{
    public class PressedKeyList
    {
        private static PressedKeyList _instance=null;

        public List<VirtualKey> PressedDic { get; set; }
        private PressedKeyList()
        {
            PressedDic = new List<VirtualKey>();
        }

        public static PressedKeyList GetInstance()
        {
            if (_instance == null)
            {
                return new PressedKeyList();
            }
            else
            {
                return _instance;
            }
        }

        public void SetKeyDown(VirtualKey key)
        {
            if (!PressedDic.Contains(key))
            {
                PressedDic.Add(key);
            }
        }

        public void SetKeyUp(VirtualKey key)
        {
            if (PressedDic.Contains(key))
            {
                PressedDic.Remove(key);
            }
        }

    }
    public class ShortcutKeyBehavior:DependencyObject,IBehavior
    {
        public event EventHandler OnShortcutDown;

        private PressedKeyList _pressedKeyList;
        public VirtualKey Key
        {
            get { return (VirtualKey)GetValue(KeyProperty); }
            set { SetValue(KeyProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Key.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty KeyProperty =
            DependencyProperty.Register("Key", typeof(VirtualKey), typeof(ShortcutKeyBehavior), new PropertyMetadata(VirtualKey.None));



        public VirtualKey ModifierKey1
        {
            get { return (VirtualKey)GetValue(ModifierKey1Property); }
            set { SetValue(ModifierKey1Property, value); }
        }

        // Using a DependencyProperty as the backing store for ModifierKey1.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ModifierKey1Property =
            DependencyProperty.Register("ModifierKey1", typeof(VirtualKey), typeof(ShortcutKeyBehavior), new PropertyMetadata(VirtualKey.None));



        public VirtualKey ModifierKey2
        {
            get { return (VirtualKey)GetValue(ModifierKey2Property); }
            set { SetValue(ModifierKey2Property, value); }
        }

        // Using a DependencyProperty as the backing store for ModifierKey2.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ModifierKey2Property =
            DependencyProperty.Register("ModifierKey2", typeof(VirtualKey), typeof(ShortcutKeyBehavior), new PropertyMetadata(VirtualKey.None));

        

        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Command.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(ShortcutKeyBehavior), new PropertyMetadata(null));


        public object CommandParameter
        {
            get { return (object)GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CommandParameter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register("CommandParameter", typeof(object), typeof(ShortcutKeyBehavior), new PropertyMetadata(null));

        private bool _isPressed;
        private bool _isFocus;
        public ShortcutKeyBehavior()
        {
            _isPressed = false;
            _isFocus = false;
            _pressedKeyList = PressedKeyList.GetInstance();
        }

        //ビヘイビアとして適用されるコントロールが入る
        private DependencyObject associatedObject;
        public DependencyObject AssociatedObject
        {
            get { return this.associatedObject; }
        }

        public void Attach(DependencyObject associatedObject)
        {
            this.associatedObject = associatedObject;
            var element = associatedObject as FrameworkElement;
            element.GotFocus += Element_GotFocus;
            element.LostFocus += Element_LostFocus;
            element.Unloaded += Element_Unloaded;
            element.PointerPressed += Element_PointerPressed;
            Dispatcher.AcceleratorKeyActivated += OnKeyActivated;
        }

        private void Element_PointerPressed(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            //GridなどのElementはFocusがあたらないため
            _isFocus = true;
        }

        private void Element_Unloaded(object sender, RoutedEventArgs e)
        {
            //Page遷移したときにFocusが外れないため
            _isFocus = false;
        }

        private void Element_LostFocus(object sender, RoutedEventArgs e)
        {
            //フォーカスが外れたとき
            _isFocus = false;
        }

        private void Element_GotFocus(object sender, RoutedEventArgs e)
        {
            //フォースがきたとき
            _isFocus = true;
        }

        public void Detach()
        {
            var element = associatedObject as FrameworkElement;
            element.GotFocus -= Element_GotFocus;
            element.LostFocus -= Element_LostFocus;
            element.Unloaded -= Element_Unloaded;
            element.PointerPressed -= Element_PointerPressed;
            this.associatedObject = null;
            Dispatcher.AcceleratorKeyActivated -= OnKeyActivated;

        }
 

        private void OnKeyActivated(CoreDispatcher dispatcher,AcceleratorKeyEventArgs e)
        {
            if (e.EventType == CoreAcceleratorKeyEventType.KeyDown)
            {
                _pressedKeyList.SetKeyDown(e.VirtualKey);
            }
            else if(e.EventType==CoreAcceleratorKeyEventType.KeyUp) 
            {
                _pressedKeyList.SetKeyUp(e.VirtualKey);
            }

            if (e.EventType == CoreAcceleratorKeyEventType.KeyDown&&e.VirtualKey==Key&&_isPressed==false&&_isFocus)
            {
                var modi1State = Window.Current.CoreWindow.GetKeyState(ModifierKey1);
                var modi2State = Window.Current.CoreWindow.GetKeyState(ModifierKey2);

                bool isModi1On = (modi1State == (CoreVirtualKeyStates.Down | CoreVirtualKeyStates.Locked) || modi1State == CoreVirtualKeyStates.Down);
                bool isModi2On = (modi2State == (CoreVirtualKeyStates.Down | CoreVirtualKeyStates.Locked) || modi2State == CoreVirtualKeyStates.Down);
            
                if (ModifierKey1 != VirtualKey.None && ModifierKey2 != VirtualKey.None && Key != VirtualKey.None&&_pressedKeyList.PressedDic.Count==3)
                {
                    //2つの修飾キーと1つめの修飾キーがある場合
                    if (isModi1On && isModi2On && e.VirtualKey == Key)
                    {
                        CommandExecute();
                        
                    }
                }
                else if (ModifierKey1 != VirtualKey.None && ModifierKey2 == VirtualKey.None && Key != VirtualKey.None && _pressedKeyList.PressedDic.Count == 2)
                {
                    //1つめの修飾キーがある場合
                    if (isModi1On && e.VirtualKey == Key)
                    {
                        CommandExecute();
                        
                    }
                }
                else if (ModifierKey1 == VirtualKey.None && ModifierKey2 == VirtualKey.None && Key != VirtualKey.None && _pressedKeyList.PressedDic.Count == 1)
                {
                    //2つの修飾キーがない場合(キーのみ)
                    if (e.VirtualKey == Key)
                    {
                        CommandExecute();
                    }
                }
                else
                {
                    //それ以外
                }
                _isPressed = true;
                
            }
            else if (e.EventType == CoreAcceleratorKeyEventType.KeyUp&&e.VirtualKey==Key&&_isFocus)
            {
                _isPressed = false;
                
            }
        }

        private void CommandExecute()
        {
            if (Command != null)
            {
                Command.Execute(CommandParameter);
            }

            if(OnShortcutDown!=null)
            {
                OnShortcutDown(this,new EventArgs());
            }
        }

        
    }
}
