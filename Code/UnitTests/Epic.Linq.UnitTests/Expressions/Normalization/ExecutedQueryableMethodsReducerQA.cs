//  
//  ExecutedQueryableMethodsReducerQA.cs
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
using System.Linq;
using System.Linq.Expressions;
using NUnit.Framework;
using Epic.Linq.Fakes;

namespace Epic.Linq.Expressions.Normalization
{
    [TestFixture]
    public class ExecutedQueryableMethodsReducerQA
    {
        [Test]
        public void Initialize_withoutComposition_throwsArgumentNullException()
        {
            // assert:
            Assert.Throws<ArgumentNullException>(delegate {
                new ExecutedQueryableMethodsReducer(null);
            });
        }

        [Test]
        public void AsVisitor_forAMethodThatDontBelongToQueryable_returnsNull()
        {
            // arrange:
            Expression<Func<int, string>> dummy = i => i.ToString();
            MethodCallExpression expressionToVisit = (MethodCallExpression)dummy.Body;
            FakeNormalizer composition = new FakeNormalizer();
            ExecutedQueryableMethodsReducer reducer = new ExecutedQueryableMethodsReducer(composition);

            // act:
            IVisitor<Expression, MethodCallExpression> result = reducer.AsVisitor(expressionToVisit);

            // assert:
            Assert.IsNull(result);
        }

        [Test]
        public void AsVisitor_forAMethodThatBelongToQueryable_returnsItself()
        {
            // arrange:
            IQueryable<int> queryable = Enumerable.Empty<int>().AsQueryable();
            Expression<Func<int, bool>> dummy = i => queryable.Contains(i);
            MethodCallExpression expressionToVisit = (MethodCallExpression)dummy.Body;
            FakeNormalizer composition = new FakeNormalizer();
            ExecutedQueryableMethodsReducer reducer = new ExecutedQueryableMethodsReducer(composition);

            // act:
            IVisitor<Expression, MethodCallExpression> result = reducer.AsVisitor(expressionToVisit);

            // assert:
            Assert.AreSame(reducer, result);
        }
    }
}

