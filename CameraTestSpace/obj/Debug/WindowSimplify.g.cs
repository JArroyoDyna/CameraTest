﻿#pragma checksum "..\..\WindowSimplify.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "1BCA56B7EFAC991AFE32F0C448BE40F83F0B44438F4E9B235BC2AD744A0BB85F"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using CameraTestSpace;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace CameraTestSpace {
    
    
    /// <summary>
    /// WindowSimplify
    /// </summary>
    public partial class WindowSimplify : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 25 "..\..\WindowSimplify.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnPrevius;
        
        #line default
        #line hidden
        
        
        #line 26 "..\..\WindowSimplify.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label lblSetting;
        
        #line default
        #line hidden
        
        
        #line 27 "..\..\WindowSimplify.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Slider slSetting;
        
        #line default
        #line hidden
        
        
        #line 28 "..\..\WindowSimplify.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox txbSetting;
        
        #line default
        #line hidden
        
        
        #line 29 "..\..\WindowSimplify.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnNext;
        
        #line default
        #line hidden
        
        
        #line 30 "..\..\WindowSimplify.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnReset;
        
        #line default
        #line hidden
        
        
        #line 35 "..\..\WindowSimplify.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Image Display;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/CameraTestSpace;component/windowsimplify.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\WindowSimplify.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            
            #line 9 "..\..\WindowSimplify.xaml"
            ((CameraTestSpace.WindowSimplify)(target)).Loaded += new System.Windows.RoutedEventHandler(this.Window_Loaded);
            
            #line default
            #line hidden
            return;
            case 2:
            this.btnPrevius = ((System.Windows.Controls.Button)(target));
            
            #line 25 "..\..\WindowSimplify.xaml"
            this.btnPrevius.Click += new System.Windows.RoutedEventHandler(this.btnPrevius_Click);
            
            #line default
            #line hidden
            return;
            case 3:
            this.lblSetting = ((System.Windows.Controls.Label)(target));
            return;
            case 4:
            this.slSetting = ((System.Windows.Controls.Slider)(target));
            
            #line 27 "..\..\WindowSimplify.xaml"
            this.slSetting.ValueChanged += new System.Windows.RoutedPropertyChangedEventHandler<double>(this.slSetting_ValueChanged);
            
            #line default
            #line hidden
            
            #line 27 "..\..\WindowSimplify.xaml"
            this.slSetting.Loaded += new System.Windows.RoutedEventHandler(this.slSetting_Loaded);
            
            #line default
            #line hidden
            return;
            case 5:
            this.txbSetting = ((System.Windows.Controls.TextBox)(target));
            
            #line 28 "..\..\WindowSimplify.xaml"
            this.txbSetting.TextChanged += new System.Windows.Controls.TextChangedEventHandler(this.TextBox_TextChanged);
            
            #line default
            #line hidden
            return;
            case 6:
            this.btnNext = ((System.Windows.Controls.Button)(target));
            
            #line 29 "..\..\WindowSimplify.xaml"
            this.btnNext.Click += new System.Windows.RoutedEventHandler(this.btnNext_Click);
            
            #line default
            #line hidden
            return;
            case 7:
            this.btnReset = ((System.Windows.Controls.Button)(target));
            
            #line 30 "..\..\WindowSimplify.xaml"
            this.btnReset.Click += new System.Windows.RoutedEventHandler(this.btnReset_Click);
            
            #line default
            #line hidden
            return;
            case 8:
            this.Display = ((System.Windows.Controls.Image)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

