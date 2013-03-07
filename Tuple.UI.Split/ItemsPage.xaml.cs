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
using Windows.UI.Xaml.Shapes;

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
        private Dictionary<uint, Button> orderButtonDic = new Dictionary<uint, Button>();
        private Dictionary<Button, ICardWithPosition> orderCardDic = new Dictionary<Button, ICardWithPosition>();
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
            orderButtonDic[0] = Button0;
            orderButtonDic[1] = Button1;
            orderButtonDic[2] = Button2;
            orderButtonDic[3] = Button3;
            orderButtonDic[4] = Button4;
            orderButtonDic[5] = Button5;
            orderButtonDic[6] = Button6;
            orderButtonDic[7] = Button7;
            orderButtonDic[8] = Button8;
            orderButtonDic[9] = Button9;
            orderButtonDic[10] = Button10;
            orderButtonDic[11] = Button11;
            orderButtonDic[12] = Button12;
            orderButtonDic[13] = Button13;
            orderButtonDic[14] = Button14;
            orderButtonDic[15] = Button15;
            orderButtonDic[16] = Button16;
            orderButtonDic[17] = Button17;

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
                orderCardDic[orderButtonDic[position]] = card;
                
                //Set Image
                orderButtonDic[position].Visibility = Visibility.Visible;
                var imageUriForCard = new Uri("ms-appx:///Images/" + card.Card.GetHashCode() + ".png");
                if (orderButtonDic[position].Content is Image)
                    ((Image)orderButtonDic[position].Content).Source = new BitmapImage(imageUriForCard);

                //Tool Tip 
                ToolTip toolTip = new ToolTip();
                toolTip.Content = card.ToString();
                ToolTipService.SetToolTip(orderButtonDic[position], toolTip);

                //this.Grid_Button.ColumnDefinitions

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

                //Check for 3 SET
                if (presedButtonsWithPosition.Count == 3)
                {
                    if (game.RemoveSet(orderCardDic[presedButtonsWithPosition[0]],
                        orderCardDic[presedButtonsWithPosition[1]],
                        orderCardDic[presedButtonsWithPosition[2]]))
                    {

                        ////////
                        //Inc counter
                        SetFoundTextBlock.Text = "SET Found: " + ++setFoundCounter;

                        //Fade 3 cards out
                        FadeOutCards(presedButtonsWithPosition[0].Name, presedButtonsWithPosition[1].Name, presedButtonsWithPosition[2].Name);
                        await Task.Delay(100);

                        //////
                        //Remove the 3 cards & Buttons from Map
                        orderCardDic.Remove(presedButtonsWithPosition[0]);
                        orderCardDic.Remove(presedButtonsWithPosition[1]);
                        orderCardDic.Remove(presedButtonsWithPosition[2]);
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
                            orderCardDic[orderButtonDic[position]] = card;
                            
                            //Open the Card with Image
                            var imageUriForCard = new Uri("ms-appx:///Images/" + card.Card.GetHashCode() + ".png");
                            ((Image)orderButtonDic[position].Content).Source = new BitmapImage(imageUriForCard); 
                            orderButtonDic[position].BorderBrush = brushOriginal;
                            orderButtonDic[position].Visibility = Visibility.Visible;
                            FadeInCard(orderButtonDic[position].Name);

                            //Tool Tip 
                            ToolTip toolTip = new ToolTip();
                            toolTip.Content = card.ToString();
                            ToolTipService.SetToolTip(orderButtonDic[position], toolTip);
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


        /// <summary>
        /// Creates a blue ellipse with black border
        /// </summary>
        public void CreateAnEllipse(Button b)
        {
            // Create an Ellipse
            Ellipse blueRectangle = new Ellipse();
            blueRectangle.Height = b.ActualHeight;
            blueRectangle.Width = b.ActualWidth;

            // Create a blue and a black Brush
            SolidColorBrush blueBrush = new SolidColorBrush();
            blueBrush.Color = Colors.Blue;
            SolidColorBrush blackBrush = new SolidColorBrush();
            blackBrush.Color = Colors.Black;

            // Set Ellipse's width and color
            blueRectangle.StrokeThickness = 4;
            blueRectangle.Stroke = blackBrush;
            // Fill rectangle with blue color
            blueRectangle.Fill = blueBrush;

            // Add Ellipse to the Grid.
            b.Content = blueRectangle;
        }
    }
}
