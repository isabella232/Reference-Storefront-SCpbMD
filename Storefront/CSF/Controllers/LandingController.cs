﻿//-----------------------------------------------------------------------
// <copyright file="LandingController.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2015
// </copyright>
// <summary>Defines the LandingController class.</summary>
//-----------------------------------------------------------------------
// Copyright 2015 Sitecore Corporation A/S
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file 
// except in compliance with the License. You may obtain a copy of the License at
//       http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software distributed under the 
// License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, 
// either express or implied. See the License for the specific language governing permissions 
// and limitations under the License.
// -------------------------------------------------------------------------------------------

namespace Sitecore.Commerce.Storefront.Controllers
{
    using Models;
    using Sitecore.Commerce.Connect.CommerceServer;
    using Sitecore.Commerce.Connect.CommerceServer.Catalog;
    using Sitecore.Commerce.Connect.CommerceServer.Catalog.Fields;
    using Sitecore.Commerce.Connect.CommerceServer.Inventory.Models;
    using Sitecore.Commerce.Connect.CommerceServer.Search;
    using Sitecore.Commerce.Connect.CommerceServer.Search.Models;
    using Sitecore.Commerce.Contacts;
    using Sitecore.Commerce.Storefront.Managers;
    using Sitecore.Commerce.Storefront.Managers.Storefront;
    using Sitecore.Commerce.Storefront.Models.Promotions;
    using Sitecore.Commerce.Storefront.Models.SitecoreItemModels;
    using Sitecore.ContentSearch.Linq;
    using Sitecore.Data;
    using Sitecore.Data.Items;
    using Sitecore.Diagnostics;
    using Sitecore.Mvc.Presentation;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Web.Mvc;

    /// <summary>
    /// Used to manage the data and view retrieval for catalog pages
    /// </summary>
    public class LandingController : BaseController
    {
        #region Variables

        private const string CurrentCategoryViewModelKeyName = "CurrentCategoryViewModel";
        private const string CurrentProductViewModelKeyName = "CurrentProductViewModel";

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="CatalogController" /> class.
        /// </summary>
        /// <param name="accountManager">The account manager.</param>
        /// <param name="inventoryManager">The inventory manager.</param>
        /// <param name="contactFactory">The contact factory.</param>
        public LandingController([NotNull] AccountManager accountManager, [NotNull] InventoryManager inventoryManager, [NotNull] ContactFactory contactFactory)
            : base(contactFactory)
        {
            Assert.ArgumentNotNull(accountManager, "accountManager");
            Assert.ArgumentNotNull(inventoryManager, "inventoryManager");

            this.InventoryManager = inventoryManager;

            if (Context.User.IsAuthenticated)
            {
                CurrentVisitorContext.ResolveCommerceUser(accountManager);
            }
        }

        /// <summary>
        /// Gets or sets the inventory manager.
        /// </summary>
        /// <value>
        /// The inventory manager.
        /// </value>
        public InventoryManager InventoryManager { get; protected set; }

        #region Controller actions

        /// <summary>
        /// An action to manage data for the home page
        /// </summary>
        /// <returns>The view that represents the a landing page</returns>
        public ActionResult Page()
        {
            var productSearchOptions = new CommerceSearchOptions { NumberOfItemsToReturn = Constants.Settings.DefaultItemsPerPage, StartPageIndex = 0 };

            var item = Context.Item;

            var viewModel = GetLandingPageViewModel(productSearchOptions, null, null, item, CurrentRendering);

            var associatedProductIds = item["AssociatedProducts"];
            if (!string.IsNullOrEmpty(associatedProductIds))
            {
                var associatedProductIdArray = associatedProductIds.Split("|".ToCharArray());
                foreach (var associatedProduct in associatedProductIdArray)
                {
                    var productItem = Context.Database.GetItem(ID.Parse(associatedProduct));
                    if (productItem == null)
                    {
                        //Happens if there is an orphan (item deleted from Sitecore)
                        //TODO log

                    }
                    else
                    {
                        viewModel.ChildProducts.Add(productItem);
                    }
                }
            }

            return View(CurrentRenderingView, viewModel);
        }


        /// <summary>
        /// Promoes the rotator.
        /// </summary>
        /// <returns>The action result</returns>
        public ActionResult PromoRotator()
        {
            var datasource = CurrentRendering.DataSource;
            var datasourceItem = Context.Database.GetItem(ID.Parse(datasource));

            var item = Context.Item;
            var associatedItemIds = datasourceItem["Promotions"];
            var associatedItemIdArray = associatedItemIds.Split("|".ToCharArray());

            var viewModel = new PromoRotator(item);

            foreach (var associatedItemId in associatedItemIdArray)
            {
                if (!string.IsNullOrEmpty(associatedItemId))
                {
                    var associatedItem = Context.Database.GetItem(ID.Parse(associatedItemId));
                    var commercePromotion = new CommercePromotion(associatedItem);
                    viewModel.Promotions.Add(commercePromotion);
                }
            }

            return View(CurrentRenderingView, viewModel);
        }

        /// <summary>
        /// Promoes the list.
        /// </summary>
        /// <returns>The action result</returns>
        public ActionResult PromoList()
        {
            var datasource = CurrentRendering.DataSource;
            var datasourceItem = Context.Database.GetItem(ID.Parse(datasource));

            if (datasourceItem == null)
            {
                //Not a valid data source.
                //TODO log the error
                return View(CurrentRenderingView, new PromoList());
            }

            var associatedItemIds = datasourceItem["Promotions"];
            var associatedItemIdArray = associatedItemIds.Split("|".ToCharArray());

            var viewModel = new PromoList(datasourceItem);


            foreach (var associatedItemId in associatedItemIdArray)
            {
                var commercePromotion = SitecoreItemManager.Instance().GetItem<CommercePromotion>(associatedItemId);
                viewModel.Promotions.Add(commercePromotion);
            }

            return View(CurrentRenderingView, viewModel);
        }

        #endregion

        #region Protected helper methods

        /// <summary>
        /// This method returns child categories for this category
        /// </summary>
        /// <param name="searchOptions">The options to perform the search with</param>
        /// <param name="categoryItem">The category item whose children to retrieve</param>
        /// <returns>A list of child categories</returns>       
        protected CategorySearchResults GetChildCategories(CommerceSearchOptions searchOptions, Item categoryItem)
        {
            var returnList = new List<Item>();
            var totalPageCount = 0;
            var totalCategoryCount = 0;

            if (Item != null)
            {
                var searchResponse = SearchNavigation.GetCategoryChildCategories(categoryItem.ID, searchOptions);

                returnList.AddRange(searchResponse.ResponseItems);

                totalCategoryCount = searchResponse.TotalItemCount;
                totalPageCount = searchResponse.TotalPageCount;
            }

            var results = new CategorySearchResults(returnList, totalCategoryCount, totalPageCount, searchOptions.StartPageIndex, new List<FacetCategory>());

            return results;
        }

        /// <summary>
        /// Creates a product view model based on an Item and Rendering
        /// </summary>
        /// <param name="productItem">The product item to based the model on</param>
        /// <param name="rendering">The rendering to initialize the model with</param>
        /// <returns>A Product View Model</returns>
        protected ProductViewModel GetProductViewModel(Item productItem, Rendering rendering)
        {
            var siteContext = CommerceTypeLoader.CreateInstance<ISiteContext>();
            if (siteContext.Items[CurrentProductViewModelKeyName] == null)
            {
                var variants = new List<VariantViewModel>();
                if (productItem != null && productItem.HasChildren)
                {
                    foreach (Item item in productItem.Children)
                    {
                        var v = new VariantViewModel(item);
                        variants.Add(v);
                    }
                }

                var productViewModel = new ProductViewModel();
                productViewModel.Initialize(rendering, variants);
                PopulateStockInformation(productViewModel);

                // get upsells
                if (productItem != null && productItem.Fields.Contains(CommerceConstants.KnownFieldIds.RelationshipList) && !string.IsNullOrEmpty(productItem[CommerceConstants.KnownFieldIds.RelationshipList]))
                {
                    var upsells = new List<Item>();
                    var field = new RelationshipField(productItem.Fields[CommerceConstants.KnownFieldIds.RelationshipList]);

                    if (field != null)
                    {
                        // display the upsells for this productItem
                        //TODO make number to take configurable
                        var relationships = GetRelationshipsFromField(field, "upsell").Take(9).ToList();
                        if (relationships != null)
                        {
                            upsells = relationships;
                        }
                    }

                    productViewModel.Upsells = upsells;
                }

                siteContext.Items[CurrentProductViewModelKeyName] = productViewModel;
            }

            return (ProductViewModel)siteContext.Items[CurrentProductViewModelKeyName];
        }

        /// <summary>
        /// Populates the stock information
        /// </summary>
        /// <param name="model">The product model</param>
        protected void PopulateStockInformation(ProductViewModel model)
        {
            var stockInfos = this.InventoryManager.GetStockInformation(this.CurrentStorefront, new List<CommerceInventoryProduct> { new CommerceInventoryProduct { ProductId = model.ProductId, CatalogName = model.CatalogName } }).Result;
            var stockInfo = stockInfos.FirstOrDefault();
            if (stockInfo == null || stockInfo.Status == null)
            {
                return;
            }

            model.StockStatus = stockInfo.Status;
            this.InventoryManager.VisitedProductStockStatus(this.CurrentStorefront, stockInfo, string.Empty);
        }

        /// <summary>
        /// Gets a lists of target items from a relationship field
        /// </summary>
        /// <param name="field">the relationship field</param>
        /// <param name="relationshipName">the relationship name, for example upsell</param>
        /// <returns>a list of relationship targets or null if no items found</returns>
        protected List<Item> GetRelationshipsFromField(RelationshipField field, string relationshipName)
        {
            List<Item> items = null;

            IEnumerable<Item> relationships = field.GetRelationshipsTargets(relationshipName);

            if (relationships != null)
            {
                items = new List<Item>(relationships);
            }

            return items;
        }

        /// <summary>
        /// Builds a category view model or retrieves it if it already exists
        /// </summary>
        /// <param name="productSearchOptions">The product options class for finding child products</param>
        /// <param name="categorySearchOptions">The category options classf or finding child categories</param>
        /// <param name="sortFields">The fields to sort he results on</param>
        /// <param name="item">The item.</param>
        /// <param name="rendering">The rendering to initialize the model with</param>
        /// <returns>
        /// A category view model
        /// </returns>
        protected virtual LandingPageViewModel GetLandingPageViewModel(CommerceSearchOptions productSearchOptions, CommerceSearchOptions categorySearchOptions, IEnumerable<CommerceQuerySort> sortFields, Item item, Rendering rendering)
        {
            var siteContext = CommerceTypeLoader.CreateInstance<ISiteContext>();
            if (siteContext.Items[CurrentCategoryViewModelKeyName] == null)
            {
                var categoryViewModel = new LandingPageViewModel(item);

                // var childProducts = GetChildProducts(productSearchOptions, categoryItem);
                // ProductSearchResults childProducts = null;
                // CategorySearchResults childCategories = null;

                // if (categorySearchOptions != null)
                // {
                //    childCategories =GetChildCategories(categorySearchOptions, categoryItem);
                // }

                // categoryViewModel.Initialize(rendering, childProducts, childCategories, sortFields, productSearchOptions);

                siteContext.Items[CurrentCategoryViewModelKeyName] = categoryViewModel;
            }

            var viewModel = (LandingPageViewModel)siteContext.Items[CurrentCategoryViewModelKeyName];

            return viewModel;
        }

        /// <summary>
        /// This method returns child products for this category
        /// </summary>
        /// <param name="searchOptions">The options to perform the search with</param>
        /// <param name="categoryItem">The category item whose children to retrieve</param>
        /// <returns>A list of child products</returns>        
        protected ProductSearchResults GetChildProducts(CommerceSearchOptions searchOptions, Item categoryItem)
        {
            IEnumerable<CommerceQueryFacet> facets = null;
            var returnList = new List<Item>();
            var totalPageCount = 0;
            var totalProductCount = 0;

            if (Item != null)
            {
                SearchResponse searchResponse = null;
                if (CatalogUtility.IsItemDerivedFromCommerceTemplate(categoryItem, CommerceConstants.KnownTemplateIds.CommerceDynamicCategoryTemplate))
                {
                    var defaultBucketQuery = categoryItem[CommerceConstants.KnownSitecoreFieldNames.DefaultBucketQuery];
                    var persistendBucketFilter = categoryItem[CommerceConstants.KnownSitecoreFieldNames.PersistentBucketFilter];
                    persistendBucketFilter = CleanLanguageFromFilter(persistendBucketFilter);
                    searchResponse = SearchNavigation.SearchCatalogItems(defaultBucketQuery, persistendBucketFilter, searchOptions);
                }
                else
                {
                    searchResponse = SearchNavigation.GetCategoryProducts(categoryItem.ID, searchOptions);
                }

                if (searchResponse != null)
                {
                    returnList.AddRange(searchResponse.ResponseItems);

                    totalProductCount = searchResponse.TotalItemCount;
                    totalPageCount = searchResponse.TotalPageCount;
                    facets = searchResponse.Facets;
                }
            }

            var results = new ProductSearchResults(returnList, totalProductCount, totalPageCount, searchOptions.StartPageIndex, facets);
            return results;
        }

        /// <summary>
        /// Takes a collection of a facets and a querystring of facet values and adds the values to the collection
        /// </summary>
        /// <param name="facets">The facet collection</param>
        /// <param name="valueQueryString">The values to add to the collection in querystring format</param>
        /// <param name="productSearchOptions">The options to update with facets</param>
        protected void UpdateOptionsWithFacets(IEnumerable<CommerceQueryFacet> facets, string valueQueryString, CommerceSearchOptions productSearchOptions)
        {
            if (facets != null && facets.Any())
            {
                if (!string.IsNullOrEmpty(valueQueryString))
                {
                    var facetValuesCombos = valueQueryString.Split(new char[] { '&' });

                    foreach (var facetValuesCombo in facetValuesCombos)
                    {
                        var facetValues = facetValuesCombo.Split(new char[] { '=' });

                        var name = facetValues[0];

                        var existingFacet = facets.FirstOrDefault(item => item.Name.Equals(name, System.StringComparison.OrdinalIgnoreCase));

                        if (existingFacet != null)
                        {
                            var values = facetValues[1].Split(new char[] { Constants.QueryStrings.FacetsSeparator });

                            foreach (var value in values)
                            {
                                existingFacet.Values.Add(value);
                            }
                        }
                    }
                }

                productSearchOptions.FacetFields = facets;
            }
        }

        /// <summary>
        /// Adds and required sorting to the options class
        /// </summary>
        /// <param name="sortField">The field to sort on</param>
        /// <param name="sortDirection">The direction to perform the sorting</param>
        /// <param name="productSearchOptions">The options class to add the sorting to.</param>
        protected void UpdateOptionsWithSorting(string sortField, CommerceConstants.SortDirection? sortDirection, CommerceSearchOptions productSearchOptions)
        {
            if (!string.IsNullOrEmpty(sortField))
            {
                productSearchOptions.SortField = sortField;

                if (sortDirection.HasValue)
                {
                    productSearchOptions.SortDirection = sortDirection.Value;
                }

                ViewBag.SortField = sortField;
                ViewBag.SortDirection = sortDirection;
            }
        }

        /// <summary>
        /// Cleans the language from filter.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <returns>The updated filter.</returns>
        protected string CleanLanguageFromFilter(string filter)
        {
            if (filter.IndexOf("language:", System.StringComparison.OrdinalIgnoreCase) < 0)
            {
                return filter;
            }

            var newFilter = new StringBuilder();

            var statementList = filter.Split(';');
            foreach (var statement in statementList)
            {
                if (statement.IndexOf("language", System.StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    continue;
                }

                if (newFilter.Length > 0)
                {
                    newFilter.Append(';');
                }

                newFilter.Append(statement);
            }

            return newFilter.ToString();
        }

        /// <summary>
        /// Returns a Promotion View based on the datasource of the control
        /// </summary>
        /// <returns></returns>
        public ActionResult Promotion()
        {
            var datasource = CurrentRendering.DataSource;
            var datasourceItem = Context.Database.GetItem(ID.Parse(datasource));
            var commercePromotion = new CommercePromotion(datasourceItem);

            return View(CurrentRenderingView, commercePromotion);
        }
        #endregion
    }
}