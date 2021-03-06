﻿using Hermes.Model;
using Hermes.Model.Models;
using System;
using System.Collections.Generic;
using System.Runtime.Caching;
/* ListingsPresenter connect view with model classes
 *  it gets data from the repositories 
 *  and pass them to view
 */

namespace Hermes.View.listings
{
    class ListingsPresenter
    {
        private readonly IListingsView _view;
        private readonly ListingRepository _listingsRepository;
        private readonly FavoritesRepository _favouritesRepository;

        public ListingsPresenter(IListingsView view)
        {
            _view = view;

            _listingsRepository = new ListingRepository();
            _favouritesRepository = new FavoritesRepository();
        }

        public void GetListings()
        {
            List<Listing> list = _listingsRepository.GetListings(0, "creationDate DESC",-1);

            if(list != null && list.Count > 0)
            {
                _view.Listings = list;
            }
        }

        public User GetUploader(int id)
        {
            return _listingsRepository.GetUploader(id);
        }

        public User GetCurrentUser()
        {
            return (User) MemoryCache.Default["User"];
        }

        private String SortListing(int option)
        {
            return option switch
            {
                1 => "price",
                2 => "listViews DESC",
                _ => "creationDate DESC",
            };
        }

        private int SellLooking(int option)
        {
            return option switch
            {
                1 => 0,
                2 => 1,
                _ => -1,
            };
        }

        public void AddToFavourites(int listingId)
        {
            User user = GetCurrentUser();

            if (user != null)
            {
                Favourite favourite = new Favourite(listingId, user.Id);

                _listingsRepository.AddToFavourite(favourite);
            }
        }

        public void GetSearchResults(string query)
        {
            List<Listing> list = _listingsRepository.GetSearchResult(query);

            if (list != null)
            {
                _view.Listings = list;
            }
        }

        public void GetFilteredListings(List<string> catIds, int category, int order, int type)
        {
            List<Listing> list = _listingsRepository.FilteredListings(catIds, category, SortListing(order), SellLooking(type));

            if (list != null)
            {
                _view.Listings = list;
            }
        }

        public void PriceFilteredListings(List<string> catIds, int priceOption, int category, int order, int type)
        {
            switch(priceOption)
            {
                case 1:
                    GetPriceFilteredListings(catIds, "<=", 100, category, SortListing(order), SellLooking(type));
                    break;
                case 2:
                    GetPriceFilteredListings(catIds, ">", 100, category, SortListing(order), SellLooking(type));
                    break;
                default:
                    GetPriceFilteredListings(catIds, "=", 0, category, SortListing(order), SellLooking(type));
                    break;
            }
        }

        public void DynamicPriceFilteredListings(List<string> catIds, int price, int category, int order, int type)
        {
            
            GetPriceFilteredListings(catIds, ">=", price, category, SortListing(order), SellLooking(type));
        }

        private void GetPriceFilteredListings(List<string> catIds, string comparisonOperator, int price, int category, string order, int type)
        {
            List<Listing> list = _listingsRepository.PriceFilteredListings(catIds, comparisonOperator, price, category, order, type);

            if (list != null)
            {
                _view.Listings = list;
            }
        }

        public void DateFilteredListings(List<string> catIds, int dateOption, int category, int order, int type)
        {
            GetDateFilteredListings(catIds, GetDateChoice(dateOption), category, SortListing(order), SellLooking(type));
        }

        public void DateAndPriceFilteredListings(List<string> catIds, int priceOption, bool comboxPrice, int dateOption, int category, int order, int type)
        {
            string date = GetDateChoice(dateOption);

            if (comboxPrice)
            {
                switch (priceOption)
                {
                    case 1:
                        GetDateAndPriceFilteredListings(catIds, "<=", 100, date, category, SortListing(order), SellLooking(type));
                        break;
                    case 2:
                        GetDateAndPriceFilteredListings(catIds, ">", 100, date, category, SortListing(order), SellLooking(type));
                        break;
                    default:
                        GetDateAndPriceFilteredListings(catIds, "=", 0, date, category, SortListing(order), SellLooking(type));
                        break;
                }
            }
            else
            {
                GetDateAndPriceFilteredListings(catIds, ">=", priceOption, date, category, SortListing(order), SellLooking(type));
            }
        }

        private string GetDateChoice(int dateOption)
        {
            var date = dateOption switch
            {
                1 => "DAY",
                2 => "WEEK",
                3 => "MONTH",
                4 => "YEAR",
                _ => "HOUR",
            };

            return date;
        }
        private void GetDateAndPriceFilteredListings(List<string> catIds, string comparisonOperator, float price, string dateOption, int category, string order, int type)
        {
            List<Listing> list = _listingsRepository.GetDateAndPriceFilteredListings(catIds, comparisonOperator, price, dateOption, category, order, type);
            
            if (list != null)
            {
                _view.Listings = list;
            }
        }

        private void GetDateFilteredListings(List<string> catIds, string dateOption, int category, string order, int type)
        {
            List<Listing> list = _listingsRepository.GetDateFilteredListings(catIds, dateOption, category, order, type);

            if (list != null)
            {
                _view.Listings = list;
            }
        }

        public void IncreaseView(int id)
        {
            _listingsRepository.IncreaseView(id);
        }

        public void AddToHistory(int listingId)
        {
            User user = GetCurrentUser();

            if(user != null)
            {
                _listingsRepository.AddToHistory(listingId, user.Id);
            }
        }

        public void ChangeCategory(int category, int order, int type)
        {
            List<Listing> list = _listingsRepository.GetListings(category, SortListing(order), SellLooking(type));

            if (list != null)
            {
                _view.Listings = list;
            }
        }

        public List<SubCategory> GetSubcategoriesFromSpecificCategory(int category)
        {
            if (category != 0)
            {
                List<SubCategory> subCategories = _listingsRepository.GetSubcategoriesFromSpecificCategory(category);

                return subCategories;
            }

            return null;
        }

        public List<Listing> GetFavorites()
        {
            if (GetCurrentUser() != null)
            {
                List<Listing> favorites = _favouritesRepository.GetFavouriteListings(GetCurrentUser().Id);

                if (favorites != null)
                {
                    return favorites;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        public bool IsOnFavorites(Listing selectedListing)
        {
            bool result = false;
            if (GetCurrentUser() != null)
            {
                foreach (Listing lis in GetFavorites())
                {
                    if (selectedListing != null && selectedListing.Id == lis.Id)
                    {
                        result = true;
                    }
                }
            }
            return result;
        }
    }
}
