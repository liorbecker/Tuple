using Tuple.UI.Split.Data;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Tuple.Infra.Log;
using Tuple.Logic.Mock;
using Tuple.Logic.Interfaces;
using System.Threading.Tasks;
using Windows.UI;

// The Items Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234233

namespace Tuple.UI.Split
{
    /// <summary>
    /// A page that displays a collection of item previews.  In the Split Application this page
    /// is used to display and select one of the available groups.
    /// </summary>
    public sealed partial class ItemsPage : Tuple.UI.Split.Common.LayoutAwarePage
    {

        private IGame game;
        private List<Button> presedButtonsWithPosition = new List<Button>();
        private Dictionary<uint, Button> orderButtinDic = new Dictionary<uint, Button>();
        private SolidColorBrush brushYellowGreen = new SolidColorBrush(Colors.YellowGreen);
        private Brush brushOriginal;
        private readonly int delaymilisec = 400;

        public ItemsPage()
        {
            MetroEventSource.Log.Debug("Initializing the ItemsPage");
            game = new Game();
            this.InitializeComponent();
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="navigationParameter">The parameter value passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested.
        /// </param>
        /// <param name="pageState">A dictionary of state preserved by this page during an earlier
        /// session.  This will be null the first time a page is visited.</param>
        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {

            orderButtinDic[0] = Button0;
            orderButtinDic[1] = Button1;
            orderButtinDic[2] = Button2;
            orderButtinDic[3] = Button3;
            orderButtinDic[4] = Button4;
            orderButtinDic[5] = Button5;
            orderButtinDic[6] = Button6;
            orderButtinDic[7] = Button7;
            orderButtinDic[8] = Button8;
            orderButtinDic[9] = Button9;
            orderButtinDic[10] = Button10;
            orderButtinDic[11] = Button11;
            orderButtinDic[12] = Button12;
            orderButtinDic[13] = Button13;
            orderButtinDic[14] = Button14; 

            brushOriginal = Button1.BorderBrush;


        }

        /// <summary>
        /// Invoked when an item is clicked.
        /// </summary>
        /// <param name="sender">The GridView (or ListView when the application is snapped)
        /// displaying the item clicked.</param>
        /// <param name="e">Event data that describes the item clicked.</param>
        void ItemView_ItemClick(object sender, ItemClickEventArgs e)
        {
        }

        private async void Grid_Loaded_1(object sender, RoutedEventArgs e)
        {
            
           

            while (game.ShouldOpenCard() ) 
            {
              
  
                await Task.Delay(delaymilisec);
                var card = game.OpenCard();
                var position = (uint)card.Position.Row + (uint)card.Position.Col*3;

                orderButtinDic[position].Visibility = Windows.UI.Xaml.Visibility.Visible;
                orderButtinDic[position].Content = card;

            }

        }


        private async void ButtonN_Click(object sender, RoutedEventArgs e)
        {
            //lock (game)
            {

                var button = (Button)e.OriginalSource;

                //Flip color BorderBrush 
                if (button.BorderBrush == brushYellowGreen)
                {
                    button.BorderBrush = brushOriginal;
                    presedButtonsWithPosition.Remove(button);
                }
                else
                {
                    if (presedButtonsWithPosition.Count == 3)
                        return;

                    button.BorderBrush = brushYellowGreen;
                    presedButtonsWithPosition.Add(button);
                    
                }

                //Check for 3sum
                if (presedButtonsWithPosition.Count == 3)
                {
                    if (game.RemoveSet(presedButtonsWithPosition[0].Content as ICardWithPosition,
                        presedButtonsWithPosition[1].Content as ICardWithPosition,
                        presedButtonsWithPosition[2].Content as ICardWithPosition))
                    {

                        //Hide the 3 cards
                        presedButtonsWithPosition[0].Visibility = Visibility.Collapsed;
                        Task.Delay(delaymilisec);
                        presedButtonsWithPosition[1].Visibility = Visibility.Collapsed;
                        Task.Delay(delaymilisec);
                        presedButtonsWithPosition[2].Visibility = Visibility.Collapsed;

                        //Check if game is over
                        if (game.IsGameOver())
                        {
                            this.pageTitle.Text = "Game finished!";
                            //TODO: game finished
                        }

                        ////////////////////////////
                        //Open new cards
                        //match  - remove 3 cards
                        if (game.ShouldOpenCard())
                        {
                            await Task.Delay(delaymilisec);
                            //await Task.Delay(delaymilisec);
                            presedButtonsWithPosition[0].Content = game.OpenCard();
                            presedButtonsWithPosition[0].Visibility = Visibility.Visible;
                            presedButtonsWithPosition[0].BorderBrush = brushOriginal;
                        }
                        if (game.ShouldOpenCard())
                        {
                            await Task.Delay(delaymilisec);
                            //await Task.Delay(delaymilisec);
                            presedButtonsWithPosition[1].Content = game.OpenCard();
                            presedButtonsWithPosition[1].Visibility = Visibility.Visible;
                            presedButtonsWithPosition[1].BorderBrush = brushOriginal;
                            
                        }

                        if (game.ShouldOpenCard())
                        {
                            await Task.Delay(delaymilisec);
                            //await Task.Delay(delaymilisec);
                            presedButtonsWithPosition[2].Content = game.OpenCard();
                            presedButtonsWithPosition[2].Visibility = Visibility.Visible;
                            presedButtonsWithPosition[2].BorderBrush = brushOriginal;
                            
                        }
                    }
                }
            }
        }

        #region Timer
        public void StartTimer(object o, RoutedEventArgs sender)
        {


            DispatcherTimer myDispatcherTimer = new DispatcherTimer();
            myDispatcherTimer.Interval = new TimeSpan(0, 0, 0, 1, 0); // 100 Milliseconds 
            myDispatcherTimer.Tick += new EventHandler<object>(Each_Tick);
            myDispatcherTimer.Start();
        }

        // A variable to count with.
        int tickinsec = 0;

        // Raised every 100 miliseconds while the DispatcherTimer is active.
        public void Each_Tick(object o, object sender)
        {
            myTextBlock.Text = "Seconds: " + tickinsec++.ToString();
        }
        #endregion 

    }
}
