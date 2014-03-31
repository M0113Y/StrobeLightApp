using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

/****************************************************************************
 * VERSION 1.0
 * 
 * AUTHOR:       Jonathon Molley
 * 
 * 
 * PURPOSE:     To create an app that acts as a flash light, strobe light, and
 *              flash Morse Code SOS. 
 * 
 *****************************************************************************/


namespace StrobeLightApp
{
    /*************************************************************************************************************
     * 
     * This one class handles all the app functions. It creates a flash light, strobe light, and sos light. 
     * The methods are for the sos button, strobe light button, sos timer, strobe light timer, and the color menu.   
     *
     *************************************************************************************************************/

    public partial class MainPage : PhoneApplicationPage
    {
        //Variables
        SolidColorBrush onColor, offColor;
        IApplicationBarIconButton strobeBtn, sosBtn;
        IApplicationBarMenuItem menuItemBtn;
        DispatcherTimer strobeLightTimer, sosTimer;
        TimeSpan dot, dash, space;          //Morse code 
        int sosCounter;
        bool strobeBtnFlag, sosBtnFlag;     //on and off button flags to make sure the strobe light and sos buttons don't run at the same time 

        // Constructor
        public MainPage()
        {
            InitializeComponent();

            //time span variables 
            dot = new TimeSpan(0, 0, 0, 0, 100);
            dash = new TimeSpan(0, 0, 0, 0, 500);
            space = new TimeSpan(0, 0, 1);

            //on and off brushes 
            onColor = new SolidColorBrush(Colors.White);
            offColor = new SolidColorBrush(Colors.Black);

            sosCounter = 0;

            //on and off flags
            strobeBtnFlag = false;
            sosBtnFlag = false;

            //create strobe light timer and sos timer 
            strobeLightTimer = new DispatcherTimer();
            strobeLightTimer.Tick += new EventHandler(StrobeLightTimer_Tick);
            strobeLightTimer.Interval = new TimeSpan(0, 0, 0, 0, 100);

            sosTimer = new DispatcherTimer();
            sosTimer.Tick += new EventHandler(SosTimer_Tick);
            sosTimer.Interval = new TimeSpan(0);


            //intialize all menu items to the same click event
            for (int i = 0; i < ApplicationBar.MenuItems.Count; i++)
            {
                menuItemBtn = (ApplicationBarMenuItem)ApplicationBar.MenuItems[i];
                menuItemBtn.Click += new EventHandler(ApplicationBarMenuItem_Click);
            }

            strobeBtn = (ApplicationBarIconButton)ApplicationBar.Buttons[0]; //strobe light button
            sosBtn = (ApplicationBarIconButton)ApplicationBar.Buttons[1]; //sos button 

            //intialize grid background to white
            LayoutRoot.Background = onColor;
        }

        //SOS timer uses a switch case statement to go through the diferent time intervals of the SOS sequence 
        //It also sets the background to the on color or off color brushes
        private void SosTimer_Tick(object sender, EventArgs e)
        {
            switch (sosCounter)
            {
                //Space
                case 0:
                case 2:
                case 4:
                case 6:
                case 8:
                case 10:
                case 12:
                case 14:
                case 16:
                    LayoutRoot.Background = offColor;
                    sosTimer.Interval = space;
                    break;
                //Dot
                case 1:
                case 3:
                case 5:
                case 13:
                case 15:
                case 17:
                    LayoutRoot.Background = onColor;
                    sosTimer.Interval = dot;
                    break;
                //Dash
                case 7:
                case 9:
                case 11:
                    LayoutRoot.Background = onColor;
                    sosTimer.Interval = dash;
                    break;
                //ends the sequence and starts it up again 
                case 18:
                    LayoutRoot.Background = offColor;
                    sosTimer.Interval = new TimeSpan(0, 0, 5);
                    sosCounter = 0;
                    break;
            }
            sosCounter++;


        }

        //Strobe light timer flashes between on and off color grid backgrounds
        private void StrobeLightTimer_Tick(object sender, EventArgs e)
        {
            if (LayoutRoot.Background == offColor)
            {
                LayoutRoot.Background = onColor;
            }
            else
            {
                LayoutRoot.Background = offColor;
            }
        }

        //Menu item colors that are indexed through the xaml phone application page resource 
        private void ApplicationBarMenuItem_Click(object sender, EventArgs e)
        {
            string chosenColor = (sender as IApplicationBarMenuItem).Text; // get string with color name
            this.onColor = (SolidColorBrush)this.Resources[chosenColor]; // get brush from Resources
            this.LayoutRoot.Background = onColor;
        }

        //Handles the strobe light on and off functions 
        private void StrobeLightButton_Click(object sender, EventArgs e)
        {
            if (strobeBtnFlag) //if strobe light is on then turn off button 
            {
                strobeLightTimer.Stop();
                LayoutRoot.Background = onColor;
                strobeBtn.Text = "Strobe Light";
                strobeBtn.IconUri = new Uri("/Images/strobelight.png", UriKind.Relative);
                strobeBtnFlag = false;
            }
            else if (sosBtnFlag == false && strobeBtnFlag == false) //if sos button is off and strobe light is off then turn on strobe light
            {
                strobeLightTimer.Start();
                strobeBtn.Text = "Cancel";
                strobeBtn.IconUri = new Uri("/Images/cancel.png", UriKind.Relative);
                strobeBtnFlag = true;
            }
        }

        //Handles the sos button on and off functions 
        private void SosButton_Click(object sender, EventArgs e)
        {
            if (sosBtnFlag) //if sos button is on then turn off sos button
            {
                ApplicationBar.IsMenuEnabled = true; //allow the user to change color 
                sosTimer.Stop();
                LayoutRoot.Background = onColor;
                sosBtn.Text = "SOS";
                sosBtn.IconUri = new Uri("/Images/sos.png", UriKind.Relative);
                sosBtnFlag = false;
            }
            else if (strobeBtnFlag == false && sosBtnFlag == false) //if strobe light button is off and sos button is off then turn on sos button 
            {
                ApplicationBar.IsMenuEnabled = false; //Don't allow the user to change the color durning the SOS sequence
                sosBtn.Text = "Cancel";
                sosBtn.IconUri = new Uri("/Images/cancel.png", UriKind.Relative);
                sosCounter = 0;
                sosTimer.Interval = new TimeSpan(0);
                sosTimer.Start();
                sosBtnFlag = true;
            }

        }
    }
}