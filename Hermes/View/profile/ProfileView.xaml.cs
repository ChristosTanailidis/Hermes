using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Runtime.Caching;
using Hermes.View.login;

namespace Hermes.View.profile
{
    public partial class ProfileView : Page, IProfileView
    {
        private readonly ProfilePresenter _presenter;
        public ProfileView()
        {
            InitializeComponent();

            _presenter = new ProfilePresenter(this);
            _presenter.GetLocations();
            _presenter.LoadFields();

            comboxRegisterAddress.SelectedIndex = 0;
            comboxRegisterAddressTK.IsEditable = false;
        }

        public string Username 
        {
            set
            {
                txtboxRegisterUsername2.Text = null;
                txtboxRegisterUsername2.Text = value;
            }
            get
            {
                return txtboxRegisterUsername2.Text;
            } 
        }
        public string Password1
        {
            set
            {
                txtboxRegisterPassword5.Text = null;
                txtboxRegisterPassword5.Text = value;
            }
            get
            {
                return txtboxRegisterPassword5.Text;
            }
        }
        public string Password2
        {
            set
            {
                txtboxRegisterPassword6.Text = null;
                txtboxRegisterPassword6.Text = value;
            }
            get
            {
                return txtboxRegisterPassword6.Text;
            }
        }
        public string Email
        {
            set
            {
                txtboxRegisterEmail.Text = null;
                txtboxRegisterEmail.Text = value;
            }
            get
            {
                return txtboxRegisterEmail.Text;
            }
        }
        public string Name
        {
            set
            {
                txtboxRegisterName.Text = null;
                txtboxRegisterName.Text = value;
            }
            get
            {
                return txtboxRegisterName.Text;
            }
        }
        public string Surname
        {
            set
            {
                txtboxRegisterSurname.Text = null;
                txtboxRegisterSurname.Text = value;
            }
            get
            {
                return txtboxRegisterSurname.Text;
            }
        }
        public string Telephone
        {
            set
            {
                txtboxRegisterPhone.Text = null;
                txtboxRegisterPhone.Text = value;
            }
            get
            {
                return txtboxRegisterPhone.Text;
            }
        }
        public string SetSelectedLocation
        {
            set
            {
                comboxRegisterAddress.SelectedItem = value;
            }
        }
        public List<string> Locations
        {
            set
            {
                comboxRegisterAddress.ItemsSource = null;
                comboxRegisterAddress.ItemsSource = value;
            }
        }

        public List<string> LocationsTK
        {
            set
            {
                comboxRegisterAddressTK.ItemsSource = null;
                comboxRegisterAddressTK.ItemsSource = value;
            }
        }

        public string SetSelectedLocationTK
        {
            set
            {
                comboxRegisterAddressTK.SelectedItem = value;
            }
        }

        public string SelectedLocationTK
        {
            get
            {
                if (comboxRegisterAddressTK.SelectedItem != null)
                {
                    return comboxRegisterAddressTK.SelectedItem.ToString();
                }
                else
                {
                    return null;
                }
            }
        }

        public string ErrorDialog
        {
            set
            {
                MessageBox.Show(value, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public string WarningDialog
        {
            set
            {
                MessageBox.Show(value, "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        public bool Navigate
        {
            set
            {
                if (value)
                {
                    this.NavigationService.Navigate(new LoginView());
                }
            }
        }

        private void btnProfileHistory_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new Uri("View/history/HistoryView.xaml", UriKind.RelativeOrAbsolute));
        }

        private void btnProfileMyProfile_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new Uri("View/profile/ProfileView.xaml", UriKind.RelativeOrAbsolute));
        }

        private void btnProfileFavorites_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new Uri("View/favourites/FavoritesView.xaml", UriKind.RelativeOrAbsolute));
        }

        private void btnProfileListings_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new Uri("View/mylistings/MyListingsView.xaml", UriKind.RelativeOrAbsolute));
        }

        private void comboxRegisterAddress_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            comboxRegisterAddressTK.IsEnabled = true;
            _presenter.GetOnlyLocationTK((string)comboxRegisterAddress.SelectedItem);
            comboxRegisterAddressTK.SelectedIndex = 0;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (Username.ToString().Length > 6 && Password1.ToString().Length > 6)
            {
                if (Password1.Equals(Password2))
                {
                    if (comboxRegisterAddressTK.SelectedItem != null)
                    {
                        if(MessageBox.Show("You are about to change your personal data.\nYou will log out if you continue.\nAre you sure","Change of data", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                        {
                            _presenter.EditUser();
                            _presenter.Logout();
                            this.NavigationService.Navigate(new Uri("View/login/LoginView.xaml", UriKind.RelativeOrAbsolute));
                        }
                    }
                    else
                    {
                        MessageBox.Show("Please select a ZIP code.", "Error");
                    }
                }
                else
                {
                    MessageBox.Show("Passwords dont match.", "Error");
                }
            }
            else
            {
                MessageBox.Show("Username or Password is too small.\nTry something bigger.", "Error");
            }
        }

        private void btnProfileSignout_Click(object sender, RoutedEventArgs e)
        {
            _presenter.Logout();
        }


    }

}