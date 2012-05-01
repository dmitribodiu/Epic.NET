//  
//  Query.cs
//  
//  Author:
//       Giacomo Tesio <giacomo@tesio.it>
// 
//  Copyright (c) 2010-2012 Giacomo Tesio
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
using System.Runtime.Serialization;
using System.Security.Permissions;


namespace Epic.Query.Object.Expressions
{
    [Serializable]
    public abstract class Expression<TValue> : VisitableBase, ISerializable
    {
        protected Expression ()
            : base()
        {
        }

        #region ISerializable implementation

        protected Expression (SerializationInfo info, StreamingContext context)
            : this()
        {
            if(null == info)
                throw new ArgumentNullException("info");
        }

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        protected abstract void GetObjectData (SerializationInfo info, StreamingContext context);

        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        void ISerializable.GetObjectData (SerializationInfo info, StreamingContext context)
        {
            if(null == info)
                throw new ArgumentNullException("info");
            GetObjectData(info, context);
        }

        #endregion ISerializable implementation
    }
}

