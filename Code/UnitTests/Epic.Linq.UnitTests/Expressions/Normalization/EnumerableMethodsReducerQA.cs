//  
//  EnumerableMethodsReducerQA.cs
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
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Epic.Linq.Fakes;
using System.Linq;
using Rhino.Mocks;

namespace Epic.Linq.Expressions.Normalization
{
    [TestFixture]
    public class EnumerableMethodsReducerQA : RhinoMocksFixtureBase
    {
        [Test]
        public void Initialize_withoutComposition_throwsArgumentNullException()
        {
            // assert:
            Assert.Throws<ArgumentNullException>(delegate {
                new EnumerableMethodsReducer(null);
            });
        }

        [Test]
        public void AsVisitor_forAMethodThatDontBelongToEnumerable_returnsNull()
        {
            // arrange:
            Expression<Func<int, string>> dummy = i => i.ToString();
            MethodCallExpression expressionToVisit = (MethodCallExpression)dummy.Body;
            FakeNormalizer composition = new FakeNormalizer();
            EnumerableMethodsReducer reducer = new EnumerableMethodsReducer(composition);

            // act:
            IVisitor<Expression, MethodCallExpression> result = reducer.AsVisitor(expressionToVisit);

            // assert:
            Assert.IsNull(result);
        }

        [Test]
        public void AsVisitor_forAMethodThatBelongToEnumerable_returnsItself()
        {
            // arrange:
            IEnumerable<int> queryable = Enumerable.Empty<int>();
            Expression<Func<int, bool>> dummy = i => queryable.Contains(i);
            MethodCallExpression expressionToVisit = (MethodCallExpression)dummy.Body;
            FakeNormalizer composition = new FakeNormalizer();
            EnumerableMethodsReducer reducer = new EnumerableMethodsReducer(composition);

            // act:
            IVisitor<Expression, MethodCallExpression> result = reducer.AsVisitor(expressionToVisit);

            // assert:
            Assert.AreSame(reducer, result);
        }

        private void RegisterEchoVisitorFor<TArgument>(CompositeVisitor<Expression> composition, IVisitContext context, MethodCallExpression callExression, int argumentIndex)
            where TArgument : Expression
        {
            TArgument argumentToReturn = (TArgument)callExression.Arguments[argumentIndex];
            FakeVisitor<Expression, TArgument> mockEchoVisitor = GeneratePartialMock<FakeVisitor<Expression, TArgument>>(composition);
            Func<TArgument, IVisitor<Expression, TArgument>> impl = a => object.ReferenceEquals(a, argumentToReturn) ? mockEchoVisitor : null;// .Return(mockEchoVisitor);
            mockEchoVisitor.Expect(v => v.CallAsVisitor<TArgument>(argumentToReturn)).IgnoreArguments().Do(impl).Repeat.Once();
            mockEchoVisitor.Expect(v => v.Visit(argumentToReturn, context)).Return(argumentToReturn).Repeat.Once();
        }

        [Test, TestCaseSource(typeof(Samples), "ReduceableEnumerableMethodCallExpressions")]
        public void Visit_anEnumerableMethodThatCanBeReduced_returnsAConstantExpressionContainingTheResultingEnumerable(MethodCallExpression expressionToVisit, IEnumerable<string> originalEnumerable, IEnumerable<string> finalEnumerable)
        {
            // arrange:
            IVisitContext context = GenerateStrictMock<IVisitContext>();
            FakeNormalizer composition = new FakeNormalizer();
            new EnumerableMethodsReducer(composition);
            FakeVisitor<Expression, MemberExpression> mockVisitor = GeneratePartialMock<FakeVisitor<Expression, MemberExpression>>(composition);
            mockVisitor.Expect(v => v.CallAsVisitor((MemberExpression)expressionToVisit.Arguments[0])).Return(mockVisitor).Repeat.Once();
            mockVisitor.Expect(v => v.Visit((MemberExpression)expressionToVisit.Arguments[0], context)).Return(Expression.Constant(originalEnumerable)).Repeat.Once();
            mockVisitor.Expect(v => v.CallAsVisitor<MethodCallExpression>(expressionToVisit)).Return(null).Repeat.Any();
            mockVisitor.Expect(v => v.CallAsVisitor<Expression>(null)).IgnoreArguments().Return(null).Repeat.Any();
            for (int i = 1; i < expressionToVisit.Arguments.Count; ++i)
            {
                switch(expressionToVisit.Arguments[i].NodeType)
                {
                    case ExpressionType.Lambda:
                        RegisterEchoVisitorFor<LambdaExpression>(composition, context, expressionToVisit, i);
                        break;
                    case ExpressionType.Constant:
                        RegisterEchoVisitorFor<ConstantExpression>(composition,context,expressionToVisit,i);
                    break;
                    case ExpressionType.MemberAccess:
                        RegisterEchoVisitorFor<MemberExpression>(composition, context, expressionToVisit, i);
                    break;
                    default:
                        Assert.Fail("TEST TO FIX: Unexpected expression of nodeType {0}, as the argument at {1} in the expression: {2}.", expressionToVisit.Arguments[i].NodeType, i, expressionToVisit.ToString());
                    break;
                }
            }

            // act:
            Expression result = composition.Visit(expressionToVisit, context);

            // assert:
            Verify.That(result).IsA<ConstantExpression>()
                  .WithA(e => e.Value, 
                         value => Verify.That(value).IsA<IEnumerable<string>>()
                                        .WithA(e => e.Count(), that => Is.EqualTo(finalEnumerable.Count()))
                                        .WithEach<string>(e => e, (c, i) => Assert.AreSame(finalEnumerable.ElementAt(i), c)));

        }

        [Test, TestCaseSource(typeof(Samples), "ReduceableEnumerableMethodCallExpressions")]
        public void Visit_anEnumerableMethodThatCanNotBeReduced_returnsAMethodCallToTheSameMethod(MethodCallExpression expressionToVisit, IEnumerable<string> originalEnumerable, IEnumerable<string> finalEnumerable)
        {
            // arrange:
            IVisitContext context = GenerateStrictMock<IVisitContext>();
            FakeNormalizer composition = new FakeNormalizer();
            new EnumerableMethodsReducer(composition);
            for (int i = 0; i < expressionToVisit.Arguments.Count; ++i)
            {
                switch (expressionToVisit.Arguments[i].NodeType)
                {
                    case ExpressionType.Lambda:
                        RegisterEchoVisitorFor<LambdaExpression>(composition, context, expressionToVisit, i);
                        break;
                    case ExpressionType.Constant:
                        RegisterEchoVisitorFor<ConstantExpression>(composition, context, expressionToVisit, i);
                        break;
                    case ExpressionType.MemberAccess:
                        RegisterEchoVisitorFor<MemberExpression>(composition, context, expressionToVisit, i);
                        break;
                    case ExpressionType.Call:
                        RegisterEchoVisitorFor<MethodCallExpression>(composition, context, expressionToVisit, i);
                        break;
                    default:
                        Assert.Fail("TEST TO FIX: Unexpected expression of nodeType {0}, as the argument at {1} in the expression: {2}.", expressionToVisit.Arguments[i].NodeType, i, expressionToVisit.ToString());
                        break;
                }
            }

            // act:
            Expression result = composition.Visit(expressionToVisit, context);

            // assert:
            Verify.That(result).IsA<MethodCallExpression>()
                  .WithEach(e => e.Arguments, (argument, atIndex) => Assert.AreSame(expressionToVisit.Arguments[atIndex], argument));

        }

        [Test, TestCaseSource(typeof(Samples), "ReduceableEnumerableMethodCallExpressions")]
        public void Visit_anEnumerableMethodThatCanBeReduced_returnsAConstantExpressionContainingTheResultingEnumerable(MethodCallExpression expressionToVisit)
        {
            // arrange:
            IVisitContext context = GenerateStrictMock<IVisitContext>();
            FakeNormalizer composition = new FakeNormalizer();
            new EnumerableMethodsReducer(composition);
            FakeVisitor<Expression, MemberExpression> mockVisitor = GeneratePartialMock<FakeVisitor<Expression, MemberExpression>>(composition);
            mockVisitor.Expect(v => v.CallAsVisitor((MemberExpression)expressionToVisit.Arguments[0])).Return(mockVisitor).Repeat.Once();
            mockVisitor.Expect(v => v.Visit((MemberExpression)expressionToVisit.Arguments[0], context)).Return(Expression.Constant(Enumerable.Empty<string>())).Repeat.Once();
            mockVisitor.Expect(v => v.CallAsVisitor<MethodCallExpression>(expressionToVisit)).Return(null).Repeat.Any();
            mockVisitor.Expect(v => v.CallAsVisitor<Expression>(null)).IgnoreArguments().Return(null).Repeat.Any();
            for (int i = 1; i < expressionToVisit.Arguments.Count; ++i)
            {
                switch (expressionToVisit.Arguments[i].NodeType)
                {
                    case ExpressionType.Lambda:
                        RegisterEchoVisitorFor<LambdaExpression>(composition, context, expressionToVisit, i);
                        break;
                    case ExpressionType.Constant:
                        RegisterEchoVisitorFor<ConstantExpression>(composition, context, expressionToVisit, i);
                        break;
                    case ExpressionType.MemberAccess:
                        RegisterEchoVisitorFor<MemberExpression>(composition, context, expressionToVisit, i);
                        break;
                    default:
                        Assert.Fail("TEST TO FIX: Unexpected expression of nodeType {0}, as the argument at {1} in the expression: {2}.", expressionToVisit.Arguments[i].NodeType, i, expressionToVisit.ToString());
                        break;
                }
            }

            // act:
            Expression result = composition.Visit(expressionToVisit, context);

            // assert:
            Verify.That(result).IsA<MethodCallExpression>()
                  .WithA(e => e.Method, that => Is.SameAs(expressionToVisit.Method))
                  .WithEach(e => e.Arguments, (that, atIndex) => Assert.AreSame(expressionToVisit.Arguments[atIndex], that));
                         

        }
    }
}
