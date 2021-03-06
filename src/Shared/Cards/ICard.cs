﻿using System;

namespace OpenDokoBlazor.Shared.Cards
{
    public interface ICard
    {
        public Suit Suit { get; }

        /// <summary>
        /// The value of the card in points.
        /// </summary>
        public int Value { get; }

        public int SortValue { get; }
        
        public Guid Id { get; }
    }
}