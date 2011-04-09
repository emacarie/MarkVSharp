using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using MarkVSharp;
using System.IO;

namespace Gui.MarkVSharp
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        private GeneratorFacade _gen;
        public Window1()
        {
            InitializeComponent();
        }

        private void buttonBrowse_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog();
            bool? result = dialog.ShowDialog();
            if (result.HasValue)
            {
                if (result.Value)
                {
                    try
                    {
                        _gen = new GeneratorFacade(new MarkovGenerator(File.ReadAllText(dialog.FileName)));
                        buttonGenerate.IsEnabled = true;
                        textStatus.Text = "Loaded " + dialog.FileName;
                    }
                    catch (Exception ex)
                    {
                        buttonGenerate.IsEnabled = false;
                        MessageBox.Show("Error: " + ex.Message);
                    }
                }
            }
        }

        private void buttonGenerate_Click(object sender, RoutedEventArgs e)
        {
            string resultString = string.Empty;
            try
            {
                if (radioSentence.IsChecked.Value)
                {
                    resultString = _gen.GenerateSentence(Convert.ToInt32(textSentence_MinWords.Text));
                }
                else if (radioWords.IsChecked.Value)
                {
                    resultString = _gen.GenerateWords(Convert.ToInt32(textWords_NumWords.Text));
                }
                else if (radioTitle.IsChecked.Value)
                {
                    resultString = _gen.GenerateTitle(Convert.ToInt32(textTitle_NumWords.Text));
                }
                else if (radioParagraphs.IsChecked.Value)
                {
                    resultString = _gen.GenerateParagraphs(Convert.ToInt32(textParagraphs_NumParagraphs.Text));
                }
                else
                {
                    throw new NotImplementedException();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
            textOutput.Text = resultString;
        }

        private void radioWords_Checked(object sender, RoutedEventArgs e)
        {
            EnableSelectTab(tabWords);
        }

        private void radioSentence_Checked(object sender, RoutedEventArgs e)
        {
            //Since sentence is checked by default tab may not have been initialized
            if (tabSentence != null)
            {
                EnableSelectTab(tabSentence);
            }
        }

        private void radioTitle_Checked(object sender, RoutedEventArgs e)
        {
            EnableSelectTab(tabTitle);
        }

        private void radioParagraphs_Checked(object sender, RoutedEventArgs e)
        {
            EnableSelectTab(tabParagraphs);
        }

        private void EnableSelectTab(TabItem enabledTab)
        {
            tabWords.IsEnabled = false;
            tabSentence.IsEnabled = false;
            tabTitle.IsEnabled = false;
            tabParagraphs.IsEnabled = false;
            enabledTab.IsEnabled = true;
            enabledTab.IsSelected = true;
        }
    }
}
