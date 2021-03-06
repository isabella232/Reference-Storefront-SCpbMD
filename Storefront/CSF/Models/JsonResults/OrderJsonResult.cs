﻿//-----------------------------------------------------------------------
// <copyright file="OrderJsonResult.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2015
// </copyright>
// <summary>Defines the OrderJsonResult class.</summary>
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

namespace Sitecore.Commerce.Storefront.Models.JsonResults
{
    using Sitecore.Commerce.Connect.CommerceServer.Orders.Models;
    using Sitecore.Commerce.Entities.Carts;
    using Sitecore.Commerce.Services;
    using Sitecore.Commerce.Services.Carts;
    using Sitecore.Diagnostics;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Web;

    /// <summary>
    /// Emits the Json result of a Order request.
    /// </summary>
    public class OrderJsonResult : CartJsonResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OrderJsonResult"/> class.
        /// </summary>
        public OrderJsonResult()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderJsonResult"/> class.
        /// </summary>
        /// <param name="result">The result.</param>
        public OrderJsonResult(ServiceProviderResult result)
            : base(result)
        {
        }

        /// <summary>
        /// Initializes this object based on the data contained in the provided cart.
        /// </summary>
        /// <param name="cart">The cart used to initialize this object.</param>
        public override void Initialize(Cart cart)
        {
            base.Initialize(cart);

            var order = cart as Sitecore.Commerce.Entities.Orders.Order;
            if (order != null)
            {
                this.OrderId = order.OrderID;
            }
        }

        /// <summary>
        /// Gets or sets the order ID.
        /// </summary>
        public string OrderId { get; set; }
    }
}