﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using WpfApp1.Model1;

namespace WpfApp1.NavigationControls
{
    /// <summary>
    /// Interaction logic for NavigationControls.xaml
    /// </summary>
    public partial class NavigationControls : UserControl
    {
        public NavigationControls()
        {
            InitializeComponent();
            CurrentPageModel currentClass = CurrentPageModel.getcurrentclass();
            PageNumber.Text = currentPageNumber(currentClass.currentpage);
            buttonManipulation(currentClass.currentpage);
        }
        public void buttonManipulation(string currentPage)
        {
            disableButtons();
            if (currentPage == "0")
            {
                pg1Button.IsChecked = true;
            }
            else if (currentPage == "1")
            {
                
                pg2Button.IsChecked = true;
            }
            else if (currentPage == "2")
            {
              
                pg3Button.IsChecked = true;
            }
            else if (currentPage == "3")
            {
               
                pg4Button.IsChecked = true;
            }
            else if (currentPage == "4")
            {
                pg5Button.IsChecked = true;
            }
            else if (currentPage == "5")
            {
              
                pg6Button.IsChecked = true;
            }
        }

        private void disableButtons()
        {
            pg1Button.IsChecked= false;
            pg2Button.IsChecked = false;
            pg3Button.IsChecked = false;
            pg4Button.IsChecked = false;
            pg5Button.IsChecked = false;
            pg6Button.IsChecked = false;
        }

        public string currentPageNumber(string currentPage)
        {
            string returnValue = null;
            if (currentPage == "0")
            {
                returnValue = "Pg1";
            }
            else if (currentPage == "1")
            {
                returnValue = "Pg2";
            }
            else if (currentPage == "2")
            {
                returnValue = "Pg3";
            }
            else if (currentPage == "3")
            {
                returnValue = "Pg4";
            }
            else if (currentPage == "4")
            {
                returnValue = "Pg5";
            }
            else if (currentPage == "5")
            {
                returnValue = "Pg6";
            }

            return returnValue;
        }

        private void NavigationButton_Click(object sender, RoutedEventArgs e)
        {
            CurrentPageModel currentClass = CurrentPageModel.getcurrentclass();
            MainWindow currentWindow = CurrentPageModel.getMainWindow();
            RadioButton currentButton = sender as RadioButton;
            if (currentButton.IsChecked == true)
            {
                if (currentButton.Name == "pg1Button")
                {
                    currentClass.currentpage = "0";
                    Page page1 = CurrentPageModel.firstPage;
                    if (page1 == null)
                    {
                        currentWindow.rootBox.NavigationService.Navigate(new Uri(@"\ProfilePages\ProfileCreationPage1.xaml", UriKind.RelativeOrAbsolute));
                    }
                    else
                    {
                        currentWindow.rootBox.NavigationService.Navigate(page1);
                        PageNumber.Text = currentPageNumber(currentClass.currentpage);
                        NavigationControls firstControl = (NavigationControls)CurrentPageModel.firstControl;
                        firstControl.buttonManipulation(currentClass.currentpage);
                        firstControl.PageNumber.Text = firstControl.currentPageNumber(currentClass.currentpage);
                    }

                }
                else if (currentButton.Name == "pg2Button")
                {
                    currentClass.currentpage = "1";
                    Page page2 = CurrentPageModel.secondPage;
                    if (page2 == null)
                    {
                        currentWindow.rootBox.Source = new Uri(@"\ProfilePages\ProfileCreationPage2.xaml", UriKind.RelativeOrAbsolute);
                      
                    }
                    else
                    {
                        currentWindow.rootBox.NavigationService.Navigate(page2);
                        PageNumber.Text = currentPageNumber(currentClass.currentpage);
                        NavigationControls secondControl = (NavigationControls) CurrentPageModel.secondControl;
                        secondControl.buttonManipulation(currentClass.currentpage);
                        secondControl.PageNumber.Text = secondControl.currentPageNumber(currentClass.currentpage);

                    }
                }

                else if (currentButton.Name == "pg3Button")
                {
                    currentClass.currentpage = "2";
                    Page page3 = CurrentPageModel.thirdPage;
                    if (page3 == null)
                    {
                        currentWindow.rootBox.Source = new Uri(@"\ProfilePages\ProfileCreationPage3.xaml", UriKind.RelativeOrAbsolute);

                    }
                    else
                    {
                        currentWindow.rootBox.NavigationService.Navigate(page3);
                        PageNumber.Text = currentPageNumber(currentClass.currentpage);
                        NavigationControls thirdControl = (NavigationControls)CurrentPageModel.thirdControl;
                        thirdControl.buttonManipulation(currentClass.currentpage);
                        thirdControl.PageNumber.Text = thirdControl.currentPageNumber(currentClass.currentpage);

                    }
                }

                else if (currentButton.Name == "pg4Button")
                {
                    currentClass.currentpage = "3";
                    Page page4 = CurrentPageModel.fourthPage;
                    if (page4 == null)
                    {
                        currentWindow.rootBox.Source = new Uri(@"\ProfilePages\ProfileCreationPage4.xaml", UriKind.RelativeOrAbsolute);

                    }
                    else
                    {
                        currentWindow.rootBox.NavigationService.Navigate(page4);
                        PageNumber.Text = currentPageNumber(currentClass.currentpage);
                        NavigationControls fourthControl = (NavigationControls)CurrentPageModel.fourthControl;
                        fourthControl.buttonManipulation(currentClass.currentpage);
                        fourthControl.PageNumber.Text = fourthControl.currentPageNumber(currentClass.currentpage);

                    }
                }

                else if (currentButton.Name == "pg5Button")
                {
                    currentClass.currentpage = "4";
                    Page page5 = CurrentPageModel.fifthPage;
                    if (page5 == null)
                    {
                        currentWindow.rootBox.Source = new Uri(@"\ProfilePages\ProfileCreationPage5.xaml", UriKind.RelativeOrAbsolute);

                    }
                    else
                    {
                        currentWindow.rootBox.NavigationService.Navigate(page5);
                        PageNumber.Text = currentPageNumber(currentClass.currentpage);
                        NavigationControls fifthControl = (NavigationControls)CurrentPageModel.fifthControl;
                        fifthControl.buttonManipulation(currentClass.currentpage);
                        fifthControl.PageNumber.Text = fifthControl.currentPageNumber(currentClass.currentpage);

                    }
                }
            }


        }

       
    }


}





