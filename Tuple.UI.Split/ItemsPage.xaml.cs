using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tuple.Infra.Log;
using Tuple.Logic.Interfaces;
using Tuple.Logic.Mock;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
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
        private Button[] orderButtonDic = new Button[18];
        private Dictionary<Button, ICardWithPosition> orderCardDic = new Dictionary<Button, ICardWithPosition>();
        private SolidColorBrush brushYellowGreen = new SolidColorBrush(new Windows.UI.Color() { A = 0xFF, R = 0x00, G = 0x71, B = 0xbc });

        private Brush brushOriginal;
        private readonly int delaymilisec = 400;
        private uint setFoundCounter = 0;
        private Boolean IsActiveGame = false;
        private String share = "empty";

        //Timer
        private int tickinsec = 0;
        DispatcherTimer myDispatcherTimer = new DispatcherTimer();

        public ItemsPage()
        {
            myDispatcherTimer.Interval = new TimeSpan(0, 0, 0, 1, 0); // 100 Milliseconds 
            myDispatcherTimer.Tick += new EventHandler<object>(Each_Tick);

            MetroEventSource.Log.Debug("Initializing the ItemsPage");
            game = new Game();
            this.InitializeComponent();

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
            DataTransferManager dataTransferManager = DataTransferManager.GetForCurrentView();
            dataTransferManager.DataRequested += new TypedEventHandler<DataTransferManager,
                DataRequestedEventArgs>(this.ShareTextHandler);
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

        private async void ButtonN_Click(object sender, RoutedEventArgs e)
        {
            //New cards added on this call will be added here
            var PositionsLst = new List<uint>();

            lock (game)
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
                        MetroEventSource.Log.Info("SET found " + orderCardDic[presedButtonsWithPosition[0]] +
                            orderCardDic[presedButtonsWithPosition[1]] +
                            orderCardDic[presedButtonsWithPosition[2]]);
                        ////////
                        //Inc counter
                        SetFoundTextBlock.Text = "SET Found: " + ++setFoundCounter;

                        //Fade 3 cards out
                        FadeOutCards(presedButtonsWithPosition[0].Name, presedButtonsWithPosition[1].Name, presedButtonsWithPosition[2].Name);
                        //await Task.Delay(100);

                        //////
                        //Remove the 3 cards & Buttons from Map
                        orderCardDic.Remove(presedButtonsWithPosition[0]);
                        orderCardDic.Remove(presedButtonsWithPosition[1]);
                        orderCardDic.Remove(presedButtonsWithPosition[2]);
                        presedButtonsWithPosition.Clear();

                        //Check if game is over
                        if (game.IsGameOver())
                        {
                            MetroEventSource.Log.Info("GAME IS OVER");
                            GameEndedAsync();
                        }

                        ////////////////////////////
                        //Open new cards
                        //match  - remove 3 cards
                        
                        while (game.ShouldOpenCard())
                        {
                            var card = game.OpenCard();
                            var position = (uint)card.Position.Row + (uint)card.Position.Col * 3;
                            PositionsLst.Add(position);
                            orderCardDic[orderButtonDic[position]] = card;
                            
                            //Open the Card with Image
                            var imageUriForCard = new Uri("ms-appx:///Images/" + card.Card.GetHashCode() + ".png");
                            ((Image)orderButtonDic[position].Content).Source = new BitmapImage(imageUriForCard); 
                            orderButtonDic[position].BorderBrush = brushOriginal;

                            //Tool Tip 
                            ToolTip toolTip = new ToolTip();
                            toolTip.Content = card.ToString();
                            ToolTipService.SetToolTip(orderButtonDic[position], toolTip);
                            
                            MetroEventSource.Log.Info("New Card added to UI - " + card);
                        }
                    }
                }
            }

            ///////////////////////////////
            //Display new cards
            //This logic will take time
            //And should not block the UI Thread
            //It will be done after the lock
            foreach (var pos in PositionsLst)
            {
                await Task.Delay(delaymilisec);
                orderButtonDic[pos].Visibility = Visibility.Visible;
                FadeInCard(orderButtonDic[pos].Name);
            }
            PositionsLst.Clear();
        }

        /// <summary>
        /// Game has Ended
        /// Ask user to quite or start new Game
        /// </summary>
        private async void GameEndedAsync()
        {
            //Stop Timer
            myDispatcherTimer.Stop();

            //Build Text box for end of game.
            StringBuilder sb = new StringBuilder();
            var stat = game.GetGameStats();
            sb.AppendLine(setFoundCounter + " Sets");
            sb.AppendLine(stat.SameColor + " Set with same color");
            sb.AppendLine(stat.SameSymbol + " Set with same shape");
            sb.AppendLine(stat.SameShading + " Set with same fill");
            sb.AppendLine(stat.Different + " Set completely different");
            //Build Text box for end of game - Title
            StringBuilder sbtitle = new StringBuilder();
            sbtitle.AppendFormat("Game Completed - {0} (new high score!)", TimeSpan.FromSeconds(tickinsec));

            share = sbtitle.ToString();


            MessageDialog md = new MessageDialog(sb.ToString(), sbtitle.ToString());

            md.Commands.Add(new UICommand("New Game", (command) =>
            {
                StartNewGame();
            }, 0));

            md.Commands.Add(new UICommand("Exit", (command) =>
            {
                Application.Current.Exit();
            }, 1));


            // Set the command that will be invoked by default
            md.DefaultCommandIndex = 0;

            // Set the command to be invoked when escape is pressed
            md.CancelCommandIndex = 1;
            await md.ShowAsync();
        }

        # region Timer

        private void StartTimer()
        {
            myDispatcherTimer.Start();
        }

        // Raised every 100 miliseconds while the DispatcherTimer is active.
        private void Each_Tick(object o, object sender)
        {
            TimerTextBox.Text = "Time: " + TimeSpan.FromSeconds(++tickinsec).ToString();
        }
        
        # endregion 

        # region Animation Procedure

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
            //Duration duration = new Duration(TimeSpan.FromSeconds(25));

            // Create two DoubleAnimations and set their properties.
            FadeInThemeAnimation fIn = new FadeInThemeAnimation();
            fIn.TargetName = b1;
            //fIn.Duration = duration;

            Storyboard sb = new Storyboard();
            //sb.Duration = duration;
            sb.Children.Add(fIn);

            // Make the Storyboard a resource.
            Grid_Button.Resources["unique_id_in"] = sb;

            // Begin the animation.
            sb.Begin();
        }

        private void FlyInAllCard()
        {
            Storyboard sb = new Storyboard();
            int Milisec = 0;

            foreach (var elem in orderButtonDic)
            {
                // Create two DoubleAnimations and set their properties.
                var anim = new PopInThemeAnimation();
                anim.TargetName = elem.Name;
                anim.FromHorizontalOffset = 500;
                anim.BeginTime = TimeSpan.FromMilliseconds(Milisec);
                Milisec += 25;
                sb.Children.Add(anim);
                elem.Visibility = Visibility.Visible;
                
            }

            // Make the Storyboard a resource.
            Grid_Button.Resources["unique_id_in_all"] = sb;
            // Begin the animation.
            sb.Begin();
        }

        # endregion

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

        private async void Button_Bar_Play_Click(object sender, RoutedEventArgs e)
        {
            //Hide App Bar
            this.bottomAppBar.IsOpen = false;

            //Replay ongoing game.
            if (IsActiveGame)
            {
                MessageDialog md = new MessageDialog("Game is in progress, would like to replay?");
                md.Commands.Add(new UICommand("OK",null,0));
                var CancelCmd = new UICommand("Cancel", null, 1);
                md.Commands.Add(CancelCmd);

                // Set the command that will be invoked by default
                md.DefaultCommandIndex = 0;

                // Set the command to be invoked when escape is pressed
                md.CancelCommandIndex = 1;

                var ret = await md.ShowAsync();
                if (ret == CancelCmd)

                    return;
            }

            //Start new Game
            StartNewGame();
        }

        private void StartNewGame()
        {
            //Clear all members
            game = new Game();
            presedButtonsWithPosition.Clear();
            orderCardDic.Clear();
            setFoundCounter = 0;
            IsActiveGame = true;
            tickinsec = 0;
            myDispatcherTimer.Stop();
            SetFoundTextBlock.Text = "SET Found: 0";
            share = "empty";

            //reset the open buttons
            foreach (var b in orderButtonDic)
            {
                b.Visibility = Visibility.Collapsed;
                b.BorderBrush = brushOriginal;
            }

            //Open the cards
            while (game.ShouldOpenCard())
            {
                var card = game.OpenCard();
                var position = (uint)card.Position.Row + (uint)card.Position.Col * 3;
                orderCardDic[orderButtonDic[position]] = card;

                //Set Image
                var imageUriForCard = new Uri("ms-appx:///Images/" + card.Card.GetHashCode() + ".png");
                ((Image)orderButtonDic[position].Content).Source = new BitmapImage(imageUriForCard);
                orderButtonDic[position].Visibility = Visibility.Visible;
                //FlyInAllCard();
                FadeInCard(orderButtonDic[position].Name);
                
                //Tool Tip 
                ToolTip toolTip = new ToolTip();
                toolTip.Content = card.ToString();
                ToolTipService.SetToolTip(orderButtonDic[position], toolTip);
            }
            
            //Start the timer

            StartTimer();
        }

        private void ShareTextHandler(DataTransferManager sender, DataRequestedEventArgs e)
        {
            DataRequest request = e.Request;
            request.Data.Properties.Title = "Share SET Result";
            request.Data.Properties.Description = "Share your high score with your friends.";
            request.Data.SetText(share);
        }
    }
}
