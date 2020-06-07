using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using CefSharp;
using CefSharp.Wpf;

namespace RevitTemplate
{
    /// <summary>
    /// Interaction logic for UI.xaml
    /// </summary>
    public partial class Ui : Window
    {
        private readonly Document _doc;

        //private readonly UIApplication _uiApp;
        //private readonly Autodesk.Revit.ApplicationServices.Application _app;
        private readonly UIDocument _uiDoc;

        private readonly EventHandlerWithStringArg _mExternalMethodStringArg;
        private readonly EventHandlerWithWpfArg _mExternalMethodWpfArg;

        private ChromiumWebBrowser _webBrowser;



        public Ui(UIApplication uiApp, EventHandlerWithStringArg evExternalMethodStringArg,
            EventHandlerWithWpfArg eExternalMethodWpfArg)
        {
            _uiDoc = uiApp.ActiveUIDocument;
            _doc = _uiDoc.Document;
            //_app = _doc.Application;
            //_uiApp = _doc.Application;
            Closed += MainWindow_Closed;

            InitializeComponent();
            _mExternalMethodStringArg = evExternalMethodStringArg;
            _mExternalMethodWpfArg = eExternalMethodWpfArg;

            _webBrowser = Browser;
            Browser.Address = "https://github.com";
            // UI Html file
            Browser.Address = @"D:\Users\egreen\source\repos\cefSharpUI\cefSharpUI\test.html";
            // Browser.ShowDevTools(); // only works if Browser is finished initializing
            //InitializeCef();

            Browser.RegisterJsObject("callbackObj", new CallbackObjectForJs(this));
        }

        public class CallbackObjectForJs
        {
            private readonly Document _doc;
            private ChromiumWebBrowser _browser;
            public CallbackObjectForJs(Ui ui)
            {
                _doc = ui._uiDoc.Document;
                _browser = ui._webBrowser;
            }

            public void showMessage(string msg)
            {//Read Note
                MessageBox.Show(msg);
            }

            public void countWalls()
            {
                
                
                // get all walls in the document
                ICollection<Wall> walls = new FilteredElementCollector(_doc)
                    .OfCategory(BuiltInCategory.OST_Walls).WhereElementIsNotElementType()
                    .Select(p => (Wall)p).ToList();

                // format the message to show the number of walls in the project
                string message = $"There are {walls.Count} Walls in the project";
                MessageBox.Show(message);
                // _browser.ExecuteScriptAsync("document.body.style.background='red';");
                // _browser.ExecuteScriptAsync("document.getElementById('tb1').value ='"+message+"';");
                _browser.ExecuteScriptAsync("updateTextbox('"+ message + "');");
            }
        }



        //private void InitializeCef()
        //{
        //    if (Cef.IsInitialized) return;

        //    Cef.EnableHighDPISupport();

        //    var assemblyLocation = Assembly.GetExecutingAssembly().Location;
        //    var assemblyPath = System.IO.Path.GetDirectoryName(assemblyLocation);
        //    var pathSubprocess = System.IO.Path.Combine(assemblyPath, "CefSharp.BrowserSubprocess.exe");
        //    var settings = new CefSettings
        //    {
        //        BrowserSubprocessPath = pathSubprocess,
        //    };

        //    Cef.Initialize(settings);
        //}


        private void MainWindow_Closed(object sender, EventArgs e)
        {
            Close();
        }

        #region External Project Methods

        private void BExtString_Click(object sender, RoutedEventArgs e)
        {
            // Raise external event with a string argument. The string MAY
            // be pulled from a Revit API context because this is an external event
            _mExternalMethodStringArg.Raise($"Title: {_doc.Title}");
        }

        private void BExternalMethod1_Click(object sender, RoutedEventArgs e)
        {
            // Raise external event with this UI instance (WPF) as an argument
            _mExternalMethodWpfArg.Raise(this);
        }

        #endregion

        #region Non-External Project Methods

        private void UserAlert()
        {
            //TaskDialog.Show("Non-External Method", "Non-External Method Executed Successfully");
            MessageBox.Show("Non-External Method Executed Successfully", "Non-External Method");

            //Dispatcher.Invoke(() =>
            //{
            //    TaskDialog mainDialog = new TaskDialog("Non-External Method")
            //    {
            //        MainInstruction = "Hello, Revit!",
            //        MainContent = "Non-External Method Executed Successfully",
            //        CommonButtons = TaskDialogCommonButtons.Ok,
            //        FooterText = "<a href=\"http://usa.autodesk.com/adsk/servlet/index?siteID=123112&id=2484975 \">"
            //                     + "Click here for the Revit API Developer Center</a>"
            //    };


            //    TaskDialogResult tResult = mainDialog.Show();
            //    Debug.WriteLine(tResult.ToString());
            //});
        }

        private void BNonExternal3_Click(object sender, RoutedEventArgs e)
        {
            // the sheet takeoff + delete method won't work here because it's not in a valid Revit api context
            // and we need to do a transaction
            // Methods.SheetRename(this, _doc); <- WON'T WORK HERE
            UserAlert();
        }

        private void BNonExternal1_Click(object sender, RoutedEventArgs e)
        {
            Methods.DocumentInfo(this, _doc);
            UserAlert();
        }

        private void BNonExternal2_Click(object sender, RoutedEventArgs e)
        {
            Methods.WallInfo(this, _doc);
            UserAlert();
        }

        #endregion
    }
}