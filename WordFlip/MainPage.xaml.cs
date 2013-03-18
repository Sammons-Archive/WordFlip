using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace WordFlip
{
    /// <summary>
    ///     An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">
        ///     Event data that describes how this page was reached.  The Parameter
        ///     property is typically used to configure the page.
        /// </param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            EntryBox.MaxLength = 10;
        }

        private void update_Background(object sender, TextChangedEventArgs e)
        {
            if (EntryBox.Text.Length > 2)
            {
                string entry = EntryBox.Text;
                foreach (
                    TextBlock block in
                        BackgroundGrid.Children.ToArray().OfType<TextBlock>().Where(block => block != Warning))
                {
                    block.Text = entry;
                }
                Warning.Text = "";
            }
            else
            {
                const string wordFlip = "Word Flip";
                foreach (
                    TextBlock block in
                        BackgroundGrid.Children.ToArray().OfType<TextBlock>().Where(block => block != Warning))
                {
                    block.Text = wordFlip;
                }
                Warning.Text = "Please enter a longer word";
            }
        }

        private void SetResultTextBoxText(String text)
        {
            resultTextBox.Text = text;
        }

        private async void GetResults(object sender, TappedRoutedEventArgs e)
        {
            // TODO: Add event handler implementation here.
            if (EntryBox.Text == "Loading") return;
            SetResultTextBoxText("Loading");
            string txt = EntryBox.Text;
            List<string> words = await Task.Run(() => WordFinder.GetPerms(txt));
            Frame.Navigate(typeof (ResultPage), words);
            SetResultTextBoxText("Result Page");
        }

        private void ToAboutPage(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof (AboutPage));
        }
    }
}