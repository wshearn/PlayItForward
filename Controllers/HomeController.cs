// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HomeController.cs" project="PiF" assembly="PiF" solution="PiF" company="Seven Software">
//   Copyright (c) Seven Software. All rights reserved.
// </copyright>
// // <author username="Robert Baker">sevenalive</author>
// // <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3">
// //  This file is part of PiF.
// //   PiF is free software: you can redistribute it and/or modify it under the terms of the GNU General Public
// //    License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any
// //    later version. PiF is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without
// //    even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public
// //    License for more details. You should have received a copy of the GNU General Public License
// //    along with PiF.  If not, see http://www.gnu.org/licenses/.
// // </license>
// --------------------------------------------------------------------------------------------------------------------

namespace PiF.Controllers
{
    using System.Web.Mvc;

    [HandleError]
    public class HomeController : Controller
    {
        #region Public Methods

        public ActionResult About()
        {
            return this.View();
        }

        public ActionResult Index()
        {
            this.ViewData["Message"] = "Welcome to the Play It Forward App!";

            return this.View();
        }

        #endregion
    }
}