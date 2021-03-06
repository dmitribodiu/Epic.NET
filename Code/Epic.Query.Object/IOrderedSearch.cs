//  
//  IOrderedSearch.cs
//  
//  Author:
//       Giacomo Tesio <giacomo@tesio.it>
// 
//  Copyright (c) 2010-2013 Giacomo Tesio
// 
//  This file is part of Epic.NET.
// 
//  Epic.NET is free software: you can redistribute it and/or modify
//  it under the terms of the GNU Affero General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
// 
//  Epic.NET is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU Affero General Public License for more details.
// 
//  You should have received a copy of the GNU Affero General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.
//  
using System;
using System.Collections.Generic;
using Epic.Query.Object.Expressions;

namespace Epic.Query.Object
{
    /// <summary>
    /// Reppresent a search for a <typeparamref name="TEntity"/> that sorts the results
    /// by an <seealso cref="OrderCriteria{TEntity}"/>.
    /// </summary>
    /// <typeparam name="TEntity">Type of the entity of interest.</typeparam>
    /// <typeparam name="TIdentity">Type of the identity of <typeparamref name="TEntity"/>.</typeparam>
    public interface IOrderedSearch<TEntity, TIdentity> : ISearch<TEntity, TIdentity>
        where TEntity : class
        where TIdentity : IEquatable<TIdentity>
    {
        /// <summary>
        /// Deferred expression, sorting the results. 
        /// </summary>
        new Order<TEntity> Expression { get; }
    }
}

