﻿#pragma checksum "C:\Users\Wesley Wei\Desktop\GazePianoPrototype\GazePianoPrototype\PianoPage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "449DE356CA8E137ECE96BC57C33E78DC"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace GazePianoPrototype
{
    partial class PianoPage : 
        global::Windows.UI.Xaml.Controls.Page, 
        global::Windows.UI.Xaml.Markup.IComponentConnector,
        global::Windows.UI.Xaml.Markup.IComponentConnector2
    {
        /// <summary>
        /// Connect()
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 10.0.17.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void Connect(int connectionId, object target)
        {
            switch(connectionId)
            {
            case 2: // PianoPage.xaml line 29
                {
                    this.PianoPlayer = (global::Windows.UI.Xaml.Controls.MediaElement)(target);
                }
                break;
            case 3: // PianoPage.xaml line 30
                {
                    this.ML = (global::Windows.UI.Xaml.Controls.Button)(target);
                    ((global::Windows.UI.Xaml.Controls.Button)this.ML).Click += this.Button_Click;
                }
                break;
            case 4: // PianoPage.xaml line 33
                {
                    this.TL = (global::Windows.UI.Xaml.Controls.Button)(target);
                    ((global::Windows.UI.Xaml.Controls.Button)this.TL).Click += this.Button_Click;
                }
                break;
            case 5: // PianoPage.xaml line 36
                {
                    this.TM = (global::Windows.UI.Xaml.Controls.Button)(target);
                    ((global::Windows.UI.Xaml.Controls.Button)this.TM).Click += this.Button_Click;
                }
                break;
            case 6: // PianoPage.xaml line 39
                {
                    this.TR = (global::Windows.UI.Xaml.Controls.Button)(target);
                    ((global::Windows.UI.Xaml.Controls.Button)this.TR).Click += this.Button_Click;
                }
                break;
            case 7: // PianoPage.xaml line 42
                {
                    this.MR = (global::Windows.UI.Xaml.Controls.Button)(target);
                    ((global::Windows.UI.Xaml.Controls.Button)this.MR).Click += this.Button_Click;
                }
                break;
            case 8: // PianoPage.xaml line 45
                {
                    this.BR = (global::Windows.UI.Xaml.Controls.Button)(target);
                    ((global::Windows.UI.Xaml.Controls.Button)this.BR).Click += this.Button_Click;
                }
                break;
            case 9: // PianoPage.xaml line 48
                {
                    this.BM = (global::Windows.UI.Xaml.Controls.Button)(target);
                    ((global::Windows.UI.Xaml.Controls.Button)this.BM).Click += this.Button_Click;
                }
                break;
            case 10: // PianoPage.xaml line 51
                {
                    this.BL = (global::Windows.UI.Xaml.Controls.Button)(target);
                    ((global::Windows.UI.Xaml.Controls.Button)this.BL).Click += this.Button_Click;
                }
                break;
            case 11: // PianoPage.xaml line 54
                {
                    this.MidGrid = (global::Windows.UI.Xaml.Controls.Grid)(target);
                    ((global::Windows.UI.Xaml.Controls.Grid)this.MidGrid).Tapped += this.Grid_Tapped;
                }
                break;
            case 12: // PianoPage.xaml line 82
                {
                    this.Center = (global::Windows.UI.Xaml.Controls.Button)(target);
                }
                break;
            default:
                break;
            }
            this._contentLoaded = true;
        }

        /// <summary>
        /// GetBindingConnector(int connectionId, object target)
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 10.0.17.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public global::Windows.UI.Xaml.Markup.IComponentConnector GetBindingConnector(int connectionId, object target)
        {
            global::Windows.UI.Xaml.Markup.IComponentConnector returnValue = null;
            return returnValue;
        }
    }
}

