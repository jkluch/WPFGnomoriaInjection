using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Faark.Gnomoria.Modding;
using Game;

namespace WPFDLLTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

    }

    //public class ThreadMaker
    //{
    //    public ThreadMaker()
    //    {
    //        Thread t = new Thread(ThreadProc);
    //        t.SetApartmentState(ApartmentState.STA);
    //        t.IsBackground = true;
    //        t.Start();
    //    }

    //    private static void ThreadProc()
    //    {
    //        WPFDLLTest.MainWindow test = new WPFDLLTest.MainWindow();
    //        System.Windows.Application appTest = new System.Windows.Application();
    //        appTest.Run(test);
    //    }

    //}

    public class GnomeTherapist: Mod
    {
        public override IEnumerable<IModification> Modifications
        {
            get
            {
                yield return new MethodHook(
                    typeof(GnomanEmpire).GetMethod("FinishLoadingGame"),
                    Method.Of<GnomanEmpire>(TherapistStart)
                    );
            }
        }
        public override string Author
        {
            get
            {
                return "ItsComcastic";
            }
        }
        public override string Description
        {
            get
            {
                return "This mod allows players to leave the current world without saving";
            }
        }

        public static void TherapistStart(GnomanEmpire self)
        {
            Thread t = new Thread(() => ThreadProc(self));
            t.SetApartmentState(ApartmentState.STA);
            t.IsBackground = true;
            t.Start();
        }
        private static void ThreadProc(GnomanEmpire theEmp)
        {
            Thread.Sleep(10000);
            WPFDLLTest.MainWindow test = new WPFDLLTest.MainWindow();
            test.lblPlayerFac.Content = "Empire Name:\n"+theEmp.World.AIDirector.PlayerFaction.Name;
            Dictionary<uint, Character> MyGnomes = theEmp.World.AIDirector.PlayerFaction.Members;
            List<Character> gnomeList = new List<Character>();
            foreach(KeyValuePair<uint, Character> gnome in MyGnomes)
            {
                gnomeList.Add(gnome.Value);
                //test.lstBxGnomes.Items.Add(gnome.Value.Name());
            }
            try
            {
                test.dgGnomes.ItemsSource = gnomeList;
            }
            catch (Exception e)
            {
                MessageBox.Show(
                "Sorry, but Gnomoria has crashed." + Environment.NewLine
                + Environment.NewLine
                + Environment.NewLine
                + "Check out these logfiles for more information:" + Environment.NewLine
                + Faark.Gnomoria.Modding.RuntimeModController.Log.GetLogfile().FullName + Environment.NewLine
                + Faark.Gnomoria.Modding.RuntimeModController.Log.GetGameLogfile().FullName,
                "Gnomoria [modded] has crashed.");
            }

            Application appTest = new Application();
            appTest.Run(test);
        }

    }
}
