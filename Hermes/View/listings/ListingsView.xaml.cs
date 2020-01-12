﻿using Hermes.Model.Models;
using System.Collections.Generic;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Diagnostics;
using Hermes.View.listings;
using System.Windows.Input;

namespace Hermes.View
{
    public partial class ListingsView : Page, IListingsView
    {
        private readonly ListingsPresenter _presenter;
        private List<String> _checkedBoxes;
        private List<Listing> favorites;

        public ListingsView()
        {
            InitializeComponent();

            _presenter = new ListingsPresenter(this);

            _presenter.GetListings();

            _presenter.GetFavorites();

            _checkedBoxes = new List<string>();

            comboxCategories.SelectedIndex = 0;

            ButtonEnable(false);
        }

        public ListingsView(string search)
        {
            InitializeComponent();

            ButtonEnable(false);

            _presenter = new ListingsPresenter(this);

            _presenter.GetSearchResults(search);

            _checkedBoxes = new List<string>();

            resetPriceRanges();

            resetDateRanges();

            resetCategoriesCheckboxes();

            radbtnListingsDatePick.IsEnabled = false;
            radbtnListingsDatePick2.IsEnabled = false;
            radbtnListingsPricePick.IsEnabled = false;
            radbtnListingsPriceCustom.IsEnabled = false;
        }

        public ListingsView(int subCategory,int category)
        {
            InitializeComponent();

            ButtonEnable(false);

            _presenter = new ListingsPresenter(this);
            _checkedBoxes = new List<string>();

            comboxCategories.SelectedIndex=category;

            _changeComboBox(subCategory);
        }

        public List<Listing> Listings
        {
            set 
            {
                listviewListings.ItemsSource = null; // Needed to reset any attached items
                listviewListings.ItemsSource = value;
            }
        }

        public bool Navigate
        {
            set
            {
                if (value)
                {
                    this.NavigationService.Navigate(new LoginPage());
                }
            }
        }

        public List<Listing> Favorites
        {
            set
            {
                favorites = value;
            }
        }

        private void comboxListingsSortBy_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(_presenter != null)
            {
                if (labelCancelPriceRanges.IsVisible & labelCancelDateRanges.IsVisible)
                {
                    DateAndPriceFilteredListings();
                }
                else if (labelCancelPriceRanges.IsVisible & !labelCancelDateRanges.IsVisible)
                {
                    PriceFilteredListings();
                }
                else if (!labelCancelPriceRanges.IsVisible & labelCancelDateRanges.IsVisible)
                {
                    comboxListingsDatePick_SelectionChanged(null, null);
                }
                else
                {
                    _presenter.GetFilteredListings(_checkedBoxes, getCatId(), comboxListingsSortBy.SelectedIndex);
                }
            }
        }

        private void BtnListingSelectedFavorite_Click(object sender, RoutedEventArgs e)
        {
            _presenter.AddToFavourites(((Listing) listviewListings.SelectedItem).Id);
            btnListingSelectedFavorite.IsEnabled = false;
        }

        private void btnListingSelectedContact_Click(object sender, RoutedEventArgs e)
        {
            Listing listing = (Listing)listviewListings.SelectedItem;
            User uploader = _presenter.GetUploader(listing.Id);

            if (listing != null && uploader != null)
            {
                var url = "mailto:" + (string) uploader.Email + "?Subject=Intrested on this item: " + listing.Name;
                Process.Start(url);
            }
        }

        private void ButtonEnable(bool action)
        {
            if (_presenter.GetCurrentUser() != null)
            {
                btnListingSelectedFavorite.IsEnabled = action;
            }
            else
            {
                btnListingSelectedFavorite.IsEnabled = false;
            }
            btnListingSelectedContact.IsEnabled = action;
        }

        //Categories Filtering
        private void chboxListingsCategory(object sender, RoutedEventArgs e)
        {
            _checkedBoxes.Add(((CheckBox)sender).Uid);
            if(labelCancelPriceRanges.IsVisible & labelCancelDateRanges.IsVisible)
            {
                DateAndPriceFilteredListings();
            }
            else if(labelCancelPriceRanges.IsVisible & !labelCancelDateRanges.IsVisible)
            {
                PriceFilteredListings();
            }
            else if(!labelCancelPriceRanges.IsVisible & labelCancelDateRanges.IsVisible)
            {
                comboxListingsDatePick_SelectionChanged(null, null);
            }
            else
            {
                _presenter.GetFilteredListings(_checkedBoxes, getCatId(), comboxListingsSortBy.SelectedIndex);
            }
        }

        private void unChboxListingsCategory(object sender, RoutedEventArgs e)
        {
            _checkedBoxes.Remove(((CheckBox)sender).Uid);
            if (labelCancelPriceRanges.IsVisible & labelCancelDateRanges.IsVisible)
            {
                DateAndPriceFilteredListings();
            }
            else if (labelCancelPriceRanges.IsVisible & !labelCancelDateRanges.IsVisible)
            {
                PriceFilteredListings();
            }
            else if (!labelCancelPriceRanges.IsVisible & labelCancelDateRanges.IsVisible)
            {
                comboxListingsDatePick_SelectionChanged(null, null);
            }
            else
            {
                _presenter.GetFilteredListings(_checkedBoxes, getCatId(), comboxListingsSortBy.SelectedIndex);
            }
        }


        //Price Range Combobox
        private void comboxListingsPricePick_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int priceOption = comboxListingsPricePick.SelectedIndex;
            if(labelCancelDateRanges.IsVisible)
            {
                _presenter.DateAndPriceFilteredListings(_checkedBoxes, priceOption, true,comboxListingsDatePick.SelectedIndex, getCatId(), comboxListingsSortBy.SelectedIndex);
            }
            else
            {
                _presenter.PriceFilteredListings(_checkedBoxes, priceOption, getCatId(), comboxListingsSortBy.SelectedIndex);
            }
        }

        //Price Range Slider
        private void btnGoSlider_Click(object sender, RoutedEventArgs e)
        {
            int priceOption = (int)slidListingsPriceCustom.Value;
            if (labelCancelDateRanges.IsVisible)
            {
                _presenter.DateAndPriceFilteredListings(_checkedBoxes, priceOption, false, comboxListingsDatePick.SelectedIndex, getCatId(), comboxListingsSortBy.SelectedIndex);
            }
            else
            {
                _presenter.DynamicPriceFilteredListings(_checkedBoxes, priceOption, getCatId(), comboxListingsSortBy.SelectedIndex);
            }
        }

        //Price Radio Buttons Checked/Unchecked
        private void radbtnListingsPricePick_Checked(object sender, RoutedEventArgs e)
        {
            comboxListingsPricePick.IsEnabled = true;
            labelCancelPriceRanges.Visibility = Visibility.Visible;
        }

        private void radbtnListingsPriceCustom_Checked(object sender, RoutedEventArgs e)
        {
            slidListingsPriceCustom.IsEnabled = true;
            labelCancelPriceRanges.Visibility = Visibility.Visible;
            btnGoSlider.IsEnabled = true;
        }

        private void radbtnListingsPricePick_Unchecked(object sender, RoutedEventArgs e)
        {
            comboxListingsPricePick.IsEnabled = false;
            comboxListingsPricePick.SelectedIndex = -1;
        }
        private void radbtnListingsPriceCustom_Unchecked(object sender, RoutedEventArgs e)
        {
            slidListingsPriceCustom.IsEnabled = false;
            slidListingsPriceCustom.Value = 0;
            btnGoSlider.IsEnabled = false;
        }

        // Price Range Cancel
        private void labelCancelPriceRanges_PreviewDragOver(object sender, MouseButtonEventArgs e)
        {
            resetPriceRanges();
            if (labelCancelDateRanges.IsVisible)
            {
                comboxListingsDatePick_SelectionChanged(null, null);
            }
            else
            {
                _presenter.GetFilteredListings(_checkedBoxes, getCatId(), comboxListingsSortBy.SelectedIndex);
            }
        }

        // Reset Price Range Combobox & Slider
        private void resetPriceRanges()
        {
            labelCancelPriceRanges.Visibility = Visibility.Hidden;
            radbtnListingsPricePick.IsChecked = false;
            radbtnListingsPriceCustom.IsChecked = false;
            comboxListingsPricePick.IsEnabled = false;
            comboxListingsPricePick.SelectedIndex = -1;
            slidListingsPriceCustom.IsEnabled = false;
            slidListingsPriceCustom.Value = 0;
            btnGoSlider.IsEnabled = false;
        }

        // Reset Date Combobox
        private void resetDateRanges()
        {
            labelCancelDateRanges.Visibility = Visibility.Hidden;
            radbtnListingsDatePick.IsChecked = false;
            radbtnListingsDatePick2.IsChecked = false;
            comboxListingsDatePick.IsEnabled = false;
            comboxListingsDatePick.SelectedIndex = -1;
        }

        private void comboxListingsDatePick_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int dateOption = comboxListingsDatePick.SelectedIndex;
            if (labelCancelPriceRanges.IsVisible)
            {
                DateAndPriceFilteredListings();
            }
            else
            {
                _presenter.DateFilteredListings(_checkedBoxes, dateOption, getCatId(), comboxListingsSortBy.SelectedIndex);
            }
        }

        // Date Cancel
        private void labelCancelDateRanges_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            resetDateRanges();
            if(labelCancelPriceRanges.IsVisible)
            {
                PriceFilteredListings();
            }
            else
            {
                _presenter.GetFilteredListings(_checkedBoxes, getCatId(), comboxListingsSortBy.SelectedIndex);
            }
        }
        
        //Date Radio Buttons Checked/Unchecked
        private void radbtnListingsDatePick_Checked(object sender, RoutedEventArgs e)
        {
            comboxListingsDatePick.IsEnabled = true;
            labelCancelDateRanges.Visibility = Visibility.Visible;
        }

        private void radbtnListingsDatePick2_Checked(object sender, RoutedEventArgs e)
        {
            datePicker.IsEnabled = true;
            labelCancelDateRanges.Visibility = Visibility.Visible;
        }

        private void radbtnListingsDatePick_Unchecked(object sender, RoutedEventArgs e)
        {
            comboxListingsDatePick.IsEnabled = false;
            comboxListingsDatePick.SelectedIndex = -1;
        }

        private void radbtnListingsDatePick2_Unchecked(object sender, RoutedEventArgs e)
        {
            datePicker.IsEnabled = false;
            //
        }

        // Change Category
        private void comboxCategories_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            resetPriceRanges();
            resetDateRanges();
            _presenter.ChangeCategory(getCatId(), comboxListingsSortBy.SelectedIndex);
            updateCategoriesCheckboxes();
            radbtnListingsDatePick.IsEnabled = true;
            radbtnListingsDatePick2.IsEnabled = true;
            radbtnListingsPricePick.IsEnabled = true;
            radbtnListingsPriceCustom.IsEnabled = true;
        }

        // Get Category ID from combobox
        private int getCatId()
        {
            return Int32.Parse(((ComboBoxItem)comboxCategories.SelectedItem).Uid);
        }

        private void _changeComboBox(int subCategory) {
            switch (subCategory)
            {
                case 1:
                    chboxListingsCategory0.IsChecked=true;
                    break;
                case 2:
                    chboxListingsCategory1.IsChecked = true;
                    break;
                case 3:
                    chboxListingsCategory2.IsChecked = true;
                    break;
                default:
                    break;
            }
                
        }

        private void updateCategoriesCheckboxes()
        {
            resetCategoriesCheckboxes();

            if (getCatId() != 0)
            {
                List<SubCategory> subCategories = _presenter.GetSubcategoriesFromSpecificCategory(getCatId());

                int i = 0;
                
                foreach (object child in LogicalTreeHelper.GetChildren(canvasListingsFilters))
                {
                    if (i < subCategories.Count)
                    {
                        if (child is CheckBox)
                        {
                            ((CheckBox)child).IsEnabled = true;
                            ((CheckBox)child).Content = subCategories[i].Name;
                            ((CheckBox)child).Uid = "" + subCategories[i].Id;
                            i++;
                        }
                    } 
                    else
                    {
                        if (child is CheckBox)
                        {
                            ((CheckBox)child).Visibility = Visibility.Hidden;
                        }
                    }
                }
            }
        }

        private void resetCategoriesCheckboxes()
        {
            foreach (object child in LogicalTreeHelper.GetChildren(canvasListingsFilters))
            {
                if (child is CheckBox)
                {
                    ((CheckBox)child).Visibility = Visibility.Visible;
                    ((CheckBox)child).Content = "";
                    ((CheckBox)child).IsChecked = false;
                    ((CheckBox)child).IsEnabled = false;
                }
            }
        }

        private void listviewListings_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ButtonEnable(true);
            if (_presenter.GetCurrentUser() != null)
            {
                _presenter.GetFavorites();
                foreach (Listing lis in favorites)
                {
                    if((listviewListings.SelectedItem as Listing).Id == lis.Id)
                    {
                        btnListingSelectedFavorite.IsEnabled = false;
                    }
                }
            }

            Listing listing = (Listing)listviewListings.SelectedItem;

            if (listing != null)
            {
                User uploader = _presenter.GetUploader(listing.Id);

                lblListingSelectedTitle.Content = listing.Name;
                tbListingSelectedDescription.Text = listing.Description;
                imgListingsSelected.Source = listing.Image;

                if (uploader != null)
                {
                    lblListingSelectedUploader.Content = uploader.Name + " " + uploader.Surname;
                    lblListingSelectedContactInfoEmail1.Content = "Telephone: " + uploader.Telephone;
                    lblListingSelectedContactInfoEmail.Content = "Email: " + uploader.Email;
                }
                else
                {
                    lblListingSelectedUploader.Content = "-";
                    lblListingSelectedContactInfoEmail1.Content = "Telephone: - ";
                    lblListingSelectedContactInfoEmail.Content = "Email: - ";
                    btnListingSelectedContact.IsEnabled = false;
                }

                _presenter.IncreaseView(listing.Id);
                _presenter.AddToHistory(listing.Id);
            }
        }

        private void Label_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (comboxCategories.SelectedIndex == 0)
            {
                comboxCategories_SelectionChanged(null, null);
            }
            else
            {
                comboxCategories.SelectedIndex = 0;
            }
        }

        private void DateAndPriceFilteredListings()
        {
            if ((bool)radbtnListingsPricePick.IsChecked)
            {
                _presenter.DateAndPriceFilteredListings(_checkedBoxes, comboxListingsPricePick.SelectedIndex, true, comboxListingsDatePick.SelectedIndex, getCatId(), comboxListingsSortBy.SelectedIndex);
            }
            else
            {
                _presenter.DateAndPriceFilteredListings(_checkedBoxes, (int)slidListingsPriceCustom.Value, false, comboxListingsDatePick.SelectedIndex, getCatId(), comboxListingsSortBy.SelectedIndex);
            }
        }

        private void PriceFilteredListings()
        {
            if ((bool)radbtnListingsPricePick.IsChecked)
            {
                comboxListingsPricePick_SelectionChanged(null, null);
            }
            else
            {
                btnGoSlider_Click(null, null);
            }
        }


    }
}
