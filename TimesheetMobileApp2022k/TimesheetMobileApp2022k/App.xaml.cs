using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TimesheetMobileApp2022k
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new EmployeePage();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
