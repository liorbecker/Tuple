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
            // TODO: Create an appropriate data model for your problem domain to replace the sample data
            var sampleDataGroups = SampleDataSource.GetGroups((String)navigationParameter);
            this.DefaultViewModel["Items"] = sampleDataGroups;

            ICardWithPosition cardWithPosition;

            //do
            //{
            //    cardWithPosition = game.OpenCard();

            //    if (card != null)
            //    {
            //        //add to deck
            //    }

            //} while (card != null);


        }

        /// <summary>
        /// Invoked when an item is clicked.
        /// </summary>
        /// <param name="sender">The GridView (or ListView when the application is snapped)
        /// displaying the item clicked.</param>
        /// <param name="e">Event data that describes the item clicked.</param>
        void ItemView_ItemClick(object sender, ItemClickEventArgs e)
        {
            // Navigate to the appropriate destination page, configuring the new page
            // by passing required information as a navigation parameter
            var groupId = ((SampleDataGroup)e.ClickedItem).UniqueId;
            //this.Frame.Navigate(typeof(SplitPage), groupId);

            var item = e.ClickedItem as SampleDataGroup;
            item.Title = "Click ";
            item.Subtitle = "Click ";


            //if (game.RemoveSet(new Position(0, 0), new Position(0, 1), new Position(0, 2)))
            //{
            //    //Remove cards

            //    ICardWithPosition cardWithPosition;

            //    do
            //    {
            //        cardWithPosition = game.OpenCard();

            //        if (card != null)
            //        {
            //            //add to deck
            //        }

            //    } while (card != null);


            //}
            game.IsGameOver();
            

            MetroEventSource.Log.Debug("Item clicked: " + item.UniqueId);


        }

    }
}
