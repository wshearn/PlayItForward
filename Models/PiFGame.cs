// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PiFGame.cs" project="PiF" assembly="PiF" solution="PiF" company="Seven Software">
//   Copyright (c) Seven Software. All rights reserved.
// </copyright>
// <author username="Robert Baker">sevenalive</author>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3">
//  This file is part of PiF.
//   PiF is free software: you can redistribute it and/or modify it under the terms of the GNU General Public
//    License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any
//    later version. PiF is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without
//    even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public
//    License for more details. You should have received a copy of the GNU General Public License
//    along with PiF.  If not, see http://www.gnu.org/licenses/.
// </license>
// --------------------------------------------------------------------------------------------------------------------
namespace PiF.Models
{
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Runtime.Serialization;

    [KnownType(typeof(PiFGame))]
    public class PiFGame
    {
        public PiFGame()
        {
            if (this.Count == 0)
            {
                this.Count = 1;
            }
        }

        #region Public Properties

        /// <summary>
        /// Gets or sets the number of copies being given.
        /// </summary>
        [Required]
        [DataType("Integer")]
        [DisplayName("Quanity")]
        [DefaultValue(1)]
        public int Count { get; set; }

        /// <summary>
        /// Gets or sets the number of copies being given.
        /// </summary>
        [Required]
        [DataType("Integer")]
        [DisplayName("Points")]
        [DefaultValue(1)]
        [ReadOnly(true)]
        public int PointWorth { get; set; }

        /// <summary>
        /// Gets or sets the name of the game.
        /// </summary>
        [DisplayName("Game")]
        
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the ID of the game.
        /// </summary>
        [DisplayName("Game")]
        [Required]
        [UIHint("GameList")]
        public int ID { get; set; }

        /// <summary>
        /// Gets or sets the steam ID of the game.
        /// </summary>
        [ReadOnly(true)]
        [DisplayName("SteamID")]
        public int SteamAppID { get; set; }

        #endregion
    }
}