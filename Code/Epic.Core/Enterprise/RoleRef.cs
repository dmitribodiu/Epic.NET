//  
//  RoleRef.cs
//  
//  Author:
//       Giacomo Tesio <giacomo@tesio.it>
// 
//  Copyright (c) 2010-2011 Giacomo Tesio
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

namespace Epic.Enterprise
{
	[Serializable]
	internal class RoleRef : IDisposable
	{
		public readonly RoleBase Role;
		private int _refs;
		public RoleRef (RoleBase role)
		{
			if(null == role)
				throw new ArgumentNullException("role");
			Role = role;
			_refs = 0;
		}
		
		public int Increase()
		{
			++_refs; // TODO: evaluate whether use CAS for thread safety
			return _refs;
		}
		
		public int Decrease()
		{
			if(_refs == 0)
			{
				string message = string.Format("No reference to {0} should be out.", Role.GetType().FullName);
				throw new InvalidOperationException(message);
			}
			--_refs; // TODO: evaluate whether use CAS for thread safety
			return _refs;
		}

		#region IDisposable implementation
		public void Dispose ()
		{
			Role.Dispose();
		}
		#endregion
	}
	
	
}
