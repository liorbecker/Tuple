using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tuple.Infra.Log;
using Tuple.Logic.Mock;
using Windows.ApplicationModel.DataTransfer;
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
        # region Members

        private Game game;
        private List<int> presedButtonsPosition = new List<int>();
        private Button[] orderButtonDic = new Button[18];
        private SolidColorBrush brushYellowGreen = new SolidColorBrush(new Windows.UI.Color() { A = 0xFF, R = 0x00, G = 0x71, B = 0xbc });

        private Brush brushOriginal;
        private readonly int delaymilisec = 400;
        private Boolean IsActiveGame = false;
        private String share = "empty";

        # endregion

        # region Constructor

        public ItemsPage()
        {
            //MetroEventSource.Log.Critical(s.ToString());

            MetroEventSource.Log.Debug("Initializing the ItemsPage");

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

            Window.Current.VisibilityChanged += Current_VisibilityChanged;
        }

        # endregion

        # region Game

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="pageState">An empty dictionary to be populated with serializable state.</param>
        protected override void SaveState(Dictionary<String, Object> pageState)
        {
            try
            {

                if (game != null)
                    game.StopTimer();

                /////////////////////
                //Save Session Data
                pageState["IsActiveGame"] = IsActiveGame;

                ///////////////////////////
                //Save the game object
                // http://www.silverlightshow.net/items/Windows-8-Metro-Something-about-application-life-cycle.aspx
                if (IsActiveGame)
                {
                    pageState["game"] = game;
                    pageState["presedButtonsPosition"] = presedButtonsPosition;
                }

            }
            catch (Exception exp)
            {
                MetroEventSource.Log.Critical(exp.Message);
            }


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

            // Restore values stored in session state.
            if (pageState != null)
            {
                try
                {
                    IsActiveGame = (Boolean)pageState["IsActiveGame"];

                    ///////////////////////////
                    //Restore the game object
                    if (pageState.ContainsKey("game") && pageState.ContainsKey("presedButtonsPosition"))
                    {
                        game = pageState["game"] as Game;
                        presedButtonsPosition = pageState["presedButtonsPosition"] as List<int>;
                        ReloadSuspendedGame();
                    }
                }
                catch (Exception exp)
                {
                    MetroEventSource.Log.Critical("Unable to Load suspended game, starting new one.");
                    MetroEventSource.Log.Critical(exp.Message);

                    //reset all saved data
                    game = null;
                    presedButtonsPosition = new List<int>();
                    pageState["game"] = null;
                    pageState["presedButtonsPosition"] = null;
                    pageState["IsActiveGame"] = false;

                    //Start new Game
                    StartNewGame();
                }
            }

            //DataTransferManager dataTransferManager = DataTransferManager.GetForCurrentView();
            //dataTransferManager.DataRequested += new TypedEventHandler<DataTransferManager,
            //    DataRequestedEventArgs>(this.ShareTextHandler);
        }



        private async void ButtonN_Click(object sender, RoutedEventArgs e)
        {
           

            //New cards added on this call will be added here
            var PositionsLst = new List<uint>();

            lock (game)
            {
                
                var button = (Button)e.OriginalSource;
                //CreateAnEllipse(button);

                //Flip color BorderBrush 
                if (button.BorderBrush == brushYellowGreen)
                {
                    button.BorderBrush = brushOriginal;
                    presedButtonsPosition.Remove(button.TabIndex);
                }
                else
                {
                    if (presedButtonsPosition.Count == 3)
                        return;

                    button.BorderBrush = brushYellowGreen;
                    presedButtonsPosition.Add(button.TabIndex);
                    
                }

                //Check for 3 SET
                if (presedButtonsPosition.Count == 3)
                {

                    if (game.RemoveSet(new Position(presedButtonsPosition[0]),
                        new Position(presedButtonsPosition[1]),
                        new Position(presedButtonsPosition[2])))
                    {
                        MetroEventSource.Log.Info("SET found " +
                            presedButtonsPosition[0] + " " +
                            presedButtonsPosition[1] + " " +
                            presedButtonsPosition[2]);
                        ////////
                        //Inc counter
                        SetFoundTextBlock.Text = "SET Found: " + game.GetGameStats().Sets;

                        //Fade 3 cards out
                        FadeOutCards(  orderButtonDic[presedButtonsPosition[0]].Name, orderButtonDic[presedButtonsPosition[1]].Name, orderButtonDic[presedButtonsPosition[2]].Name);
                        presedButtonsPosition.Clear();

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
                            
                            //Open the Card with Image
                            var imageUriForCard = new Uri("ms-appx:///Images/" + card.Card.GetHashCode() + ".png");
                            ((Image)orderButtonDic[position].Content).Source = new BitmapImage(imageUriForCard);
                            orderButtonDic[position].Visibility = Visibility.Collapsed;
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
            game.StopTimer();

            //Build Text box for end of game.
            StringBuilder sb = new StringBuilder();
            var stat = game.GetGameStats();
            sb.AppendLine(stat.Sets + " Sets");
            sb.AppendLine(stat.SameColor + " Sets with same color");
            sb.AppendLine(stat.SameSymbol + " Sets with same shape");
            sb.AppendLine(stat.SameShading + " Sets with same fill");
            sb.AppendLine(stat.SameNumber + " Sets with same number");
            sb.AppendLine(stat.Different + " Sets completely different");
            sb.AppendLine(stat.Time + " time"); // TODO
            //Build Text box for end of game - Title
            var sbtitle = String.Format("Game Completed - {0} ", stat.Time.ToString());
            share = sbtitle;


            ////////////////
            //Save High Score
            //Save App Data
            Windows.Storage.ApplicationDataContainer roamingSettings = Windows.Storage.ApplicationData.Current.RoamingSettings;
            if (!roamingSettings.Values.ContainsKey("HighScore"))/*First game Ever !!!*/
            {
                roamingSettings.Values["HighScore"] = stat.Time.ToString();
            }
            else
            {
                if (roamingSettings.Values.ContainsKey("HighScore") &&
                 TimeSpan.Compare(TimeSpan.Parse(roamingSettings.Values["HighScore"].ToString()), stat.Time) > 0) /*New High Score*/
                {
                    //New High score save it 
                    roamingSettings.Values["HighScore"] = stat.Time.ToString();
                    sbtitle = String.Format("Game Completed, New High Score - {0} ", stat.Time.ToString());
                }
            }

            /////////////////
            //Show dialog Box
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

        private void StartNewGame()
        {
            //Clear all members
            game = new Game();
            game.SecondPassed += new EventHandler<object>(Each_Tick);

            presedButtonsPosition.Clear();
            IsActiveGame = true;
            SetFoundTextBlock.Text = "SET Found: 0";
            TimerTextBox.Text = "Time: 00:00:00";
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
            game.StartTimer();
        }


        /// <summary>
        /// Reload Existing game from saved data
        /// </summary>
        private async void ReloadSuspendedGame()
        {
            //Hide the App Bar
            this.bottomAppBar.IsOpen = false;

            //Set  Text Box values
            SetFoundTextBlock.Text = "SET Found: " + game.GetGameStats().Sets;
            Each_Tick(null, null);


            /////////////////
            //Show dialog Box
            MessageDialog md = new MessageDialog("Press continue to keep playing...", "Game was suspended");
            md.Commands.Add(new UICommand("CONTINUE", null, 0));
            await md.ShowAsync();

            //reset the open buttons
            foreach (var b in orderButtonDic)
            {
                b.Visibility = Visibility.Collapsed;
                b.BorderBrush = brushOriginal;
            }

            //Check if game is over
            if (game.IsGameOver())
            {
                MetroEventSource.Log.Info("GAME IS OVER");
                GameEndedAsync();
            }

            //Dispaly open Cards with No animation
            foreach(var card in game.GetAllOpenedCards())
            {
                var position = (uint)card.Position.Row + (uint)card.Position.Col * 3;
                //Set Image
                var imageUriForCard = new Uri("ms-appx:///Images/" + card.Card.GetHashCode() + ".png");
                ((Image)orderButtonDic[position].Content).Source = new BitmapImage(imageUriForCard);
                orderButtonDic[position].Visibility = Visibility.Visible;

                //Tool Tip 
                ToolTip toolTip = new ToolTip();
                toolTip.Content = card.ToString();
                ToolTipService.SetToolTip(orderButtonDic[position], toolTip);
            }

            //Set Pressed Cards
            foreach (var pos in presedButtonsPosition)
            {
                orderButtonDic[pos].BorderBrush = brushYellowGreen;
            }

            //Open new cards if needed
            while (game.ShouldOpenCard())
            {
                var card = game.OpenCard();
                var position = (uint)card.Position.Row + (uint)card.Position.Col * 3;

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
            Each_Tick(null,null);
            game.SecondPassed += new EventHandler<object>(Each_Tick);
            game.StartTimer();
        }


        /// <summary>
        /// User chamged the visability mode
        /// Stop\Start the timer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Current_VisibilityChanged(object sender, Windows.UI.Core.VisibilityChangedEventArgs e)
        {
            if (!e.Visible)
                StopTimer();
            else
                StartTimer();
        }

        # endregion

        # region Timer

        // Raised every second while the DispatcherTimer is active.
        private void Each_Tick(object o, object sender)
        {
            TimerTextBox.Text = "Time: " + game.GetGameStats().Time.ToString();
        }


        /// <summary>
        /// Will be called from the App after Resuming
        /// </summary>
        public void StartTimer()
        {
            if (game != null && IsActiveGame)
                game.StartTimer();
        }

        /// <summary>
        /// Will be called from the App after Suspended\VisibilityChangedEventArgs
        /// </summary>
        public void StopTimer()
        {
            if (game != null && IsActiveGame)
                game.StopTimer();
        }

        # endregion 

        # region Animation Procedures

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

        # region App bar

        private async void Button_Bar_HighScore_Click(object sender, RoutedEventArgs e)
        {
            // Restore values stored in app data.
            Windows.Storage.ApplicationDataContainer roamingSettings = Windows.Storage.ApplicationData.Current.RoamingSettings;
            if (roamingSettings.Values.ContainsKey("HighScore"))
            {
                MessageDialog md = new MessageDialog(roamingSettings.Values["HighScore"].ToString(), "Highest Score");
                md.Commands.Add(new UICommand("OK", null, 0));
                // Set the command that will be invoked by default
                md.DefaultCommandIndex = 0;
                // Set the command to be invoked when escape is pressed
                md.CancelCommandIndex = 0;
                var ret = await md.ShowAsync();
            }

            //Display App Bar
            this.bottomAppBar.IsOpen = true;

        }

        private async void Button_Bar_Play_Click(object sender, RoutedEventArgs e)
        {
            //Hide App Bar
            this.bottomAppBar.IsOpen = false;

            //Replay ongoing game.
            if (IsActiveGame)
            {
                MessageDialog md = new MessageDialog("Game is in progress, would like to replay?");
                md.Commands.Add(new UICommand("OK", null, 0));
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

        # endregion

        # region Share

        private void ShareTextHandler(DataTransferManager sender, DataRequestedEventArgs e)
        {
            DataRequest request = e.Request;
            request.Data.Properties.Title = "Share SET Result";
            request.Data.Properties.Description = "Share your high score with your friends.";
            request.Data.SetText(share);
        }

        # endregion

        # region Roi UI tests

        /// <summary>
        /// Creates a blue ellipse with black border
        /// </summary>
        public void CreateAnEllipse(Button b)
        {
            Ellipse el = new Ellipse()
            {

                StrokeThickness = 4,
                Stroke = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 145, 0, 145))
            };

            el.Height = b.ActualHeight / 2;
            el.Width = el.Height / 2;

            var image = new ImageBrush()
            {
                ImageSource = new BitmapImage(new Uri("ms-appx:/Assets/test.png")),
                Stretch = Stretch.None
            };
            el.Fill = image;

            // Add Ellipse to the Grid.
            b.Content = el;
        }

        # endregion
    }
}
