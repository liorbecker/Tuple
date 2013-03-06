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
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Popups;
using Windows.UI.Xaml.Media.Imaging;

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
        private SolidColorBrush brushYellowGreen = new SolidColorBrush(new Windows.UI.Color() { A = 0xFF, R = 0x00, G = 0xb2, B = 0xf0 });
        private Brush brushOriginal;
        private readonly int delaymilisec = 400;
        private uint setFoundCounter = 0;

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
            orderButtinDic[15] = Button15;
            orderButtinDic[16] = Button16;
            orderButtinDic[17] = Button17;

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

        private void Grid_Loaded_1(object sender, RoutedEventArgs e)
        {
            while (game.ShouldOpenCard() ) 
            {
                var card = game.OpenCard();
                var position = (uint)card.Position.Row + (uint)card.Position.Col*3;

                orderButtinDic[position].Visibility = Windows.UI.Xaml.Visibility.Visible;
                ((TextBlock)((StackPanel)orderButtinDic[position].Content).Children[0]).Text = card.ToString();
                var imageUriForCard = new Uri("ms-appx:///Images/" + card.Card.GetHashCode() + ".png");
                ((Image)((StackPanel)orderButtinDic[position].Content).Children[1]).Source = new BitmapImage(imageUriForCard); 
                ((Button)((StackPanel)orderButtinDic[position].Content).Children[2]).Content = card;


                

            }
        }


        private ICardWithPosition GetCardFromButton(Button b)
        {
            return ((Button)((StackPanel)b.Content).Children[2]).Content as ICardWithPosition;
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

                //Check for 3 SET
                if (presedButtonsWithPosition.Count == 3)
                {
                    if (game.RemoveSet(GetCardFromButton(presedButtonsWithPosition[0]) ,
                        GetCardFromButton(presedButtonsWithPosition[1]),
                        GetCardFromButton(presedButtonsWithPosition[2])))
                    {

                        ////////
                        //Inc counter
                        SetFoundTextBlock.Text = "SET Found: " + ++setFoundCounter;

                        //Fade 3 cards out
                        FadeOutCards(presedButtonsWithPosition[0].Name, presedButtonsWithPosition[1].Name, presedButtonsWithPosition[2].Name);
                        await Task.Delay(100);

                     
                        presedButtonsWithPosition.Clear();

                        //Check if game is over
                        if (game.IsGameOver())
                        {
                            this.pageTitle.Text = "Game finished!";
                            MessageDialog md = new MessageDialog(TimerTextBox.Text + Environment.NewLine + SetFoundTextBlock.Text, "Game completed");
                            md.Commands.Add(new UICommand("OK"));
                            await md.ShowAsync();
                            //TODO: init new Game
                        }

                        ////////////////////////////
                        //Open new cards
                        //match  - remove 3 cards
                        while (game.ShouldOpenCard())
                        {
                            await Task.Delay(delaymilisec);
                            var card = game.OpenCard();
                            var position = (uint)card.Position.Row + (uint)card.Position.Col * 3;
                            //Open the Card
                            //orderButtinDic[position].Content = card;
                            ((TextBlock)((StackPanel)orderButtinDic[position].Content).Children[0]).Text = card.ToString();
                            var imageUriForCard = new Uri("ms-appx:///Images/" + card.Card.GetHashCode() + ".png");
                            ((Image)((StackPanel)orderButtinDic[position].Content).Children[1]).Source = new BitmapImage(imageUriForCard); 
                            ((Button)((StackPanel)orderButtinDic[position].Content).Children[2]).Content = card;
                            orderButtinDic[position].BorderBrush = brushOriginal;
                            orderButtinDic[position].Visibility = Windows.UI.Xaml.Visibility.Visible;
                            FadeInCard(orderButtinDic[position].Name);
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

            //TimeSpan t = new TimeSpan(0, 0, 0, tickinsec++);

            TimerTextBox.Text = "Time: " + TimeSpan.FromSeconds(++tickinsec).ToString();
        }
        #endregion 

        #region Animation Procedure
        private void FadeOutCards(String b1, String b2, String b3)
        {
           
            // Create a duration of 2 seconds.
            Duration duration = new Duration(TimeSpan.FromSeconds(25));

            // Create two DoubleAnimations and set their properties.
            FadeOutThemeAnimation fOut1 = new FadeOutThemeAnimation();
            FadeOutThemeAnimation fOut2 = new FadeOutThemeAnimation();
            FadeOutThemeAnimation fOut3 = new FadeOutThemeAnimation();

            fOut1.TargetName = b1;
            fOut1.Duration = duration;
            fOut2.TargetName = b2;
            fOut2.Duration = duration;
            fOut3.TargetName = b3;
            fOut3.Duration = duration;
            
            Storyboard sb = new Storyboard();
            sb.Duration = duration;
            sb.Children.Add(fOut1);
            sb.Children.Add(fOut2);
            sb.Children.Add(fOut3);


            // Make the Storyboard a resource.
            Grid_Button.Resources["unique_id_out"] = sb;

            // Begin the animation.
            sb.Begin();
        }

        private void FadeInCard(String b1)
        {

            // Create a duration of 2 seconds.
            Duration duration = new Duration(TimeSpan.FromSeconds(25));

            // Create two DoubleAnimations and set their properties.
            FadeInThemeAnimation fIn = new FadeInThemeAnimation();
            fIn.TargetName = b1;
            fIn.Duration = duration;

            Storyboard sb = new Storyboard();
            sb.Duration = duration;
            sb.Children.Add(fIn);


            // Make the Storyboard a resource.
            Grid_Button.Resources["unique_id_in"] = sb;

            // Begin the animation.
            sb.Begin();
        }
        #endregion

    }
}
