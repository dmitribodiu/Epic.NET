//  
//  VisitorComposition.cs
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
using System.Linq.Expressions;
using System.Collections.Generic;

namespace Epic.Linq.Expressions.Visit
{
    public sealed class VisitorsComposition : ICompositeVisitor
    {
        private readonly List<VisitorBase> _chain;
        public VisitorsComposition ()
        {
            _chain = new List<VisitorBase>();
            new UnvisitableExpressionsVisitor(this);
        }

        #region ICompositeVisitor implementation
        public ICompositeVisitor<TExpression> GetVisitor<TExpression> (TExpression target) where TExpression : System.Linq.Expressions.Expression
        {
            return GetVisitor<TExpression>(target, _chain.Count);
        }
        #endregion
        
        private void Register(VisitorBase visitor, out int compositionSize)
        {
            compositionSize = _chain.Count;
            _chain.Add(visitor);
        }
        
        private static Expression IdentityVisit<TExpression>(TExpression expression, IVisitState state) where TExpression : Expression
        {
            return expression;
        }
        
        private ICompositeVisitor<TExpression> GetVisitor<TExpression>(TExpression target, int startingPosition) where TExpression : Expression
        {
            ICompositeVisitor<TExpression> foundVisitor = null;
            
            while(startingPosition > 0)
            {
                --startingPosition;
                VisitorBase visitor = _chain[startingPosition];
                foundVisitor = visitor.AsVisitor<TExpression>(target);
                if(null != foundVisitor)
                    return foundVisitor;
            }
            
            return new VisitorWrapper<TExpression>(this, IdentityVisit<TExpression>); // TODO : should we throw ???
        }
        
        public abstract class VisitorBase : ICompositeVisitor
        {
            private readonly int _nextVisitor;
            private readonly VisitorsComposition _composition;
            
            protected ICompositeVisitor<TExpression> GetNextVisitor<TExpression>(TExpression target) where TExpression : Expression
            {
                return _composition.GetVisitor<TExpression>(target, _nextVisitor);
            }
            
            protected VisitorBase(VisitorsComposition composition)
            {
                if(null == composition)
                    throw new ArgumentNullException("composition");
                _composition = composition;
                _composition.Register(this, out _nextVisitor);
            }
            
            protected internal virtual ICompositeVisitor<TExpression> AsVisitor<TExpression>(TExpression target) where TExpression : Expression
            {
                return this as ICompositeVisitor<TExpression>;
            }
            
            #region ICompositeVisitor implementation
            public ICompositeVisitor<TExpression> GetVisitor<TExpression> (TExpression target) where TExpression : System.Linq.Expressions.Expression
            {
                return _composition.GetVisitor<TExpression>(target);
            }
            #endregion
        }
    }
}

