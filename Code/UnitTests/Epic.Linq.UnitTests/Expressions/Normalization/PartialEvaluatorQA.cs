﻿using System;
using System.Linq.Expressions;
using Rhino.Mocks;
using NUnit.Framework;
using Epic.Linq.Fakes;

namespace Epic.Linq.Expressions.Normalization
{
    [TestFixture]
    public class PartialEvaluatorQA : RhinoMocksFixtureBase
    {
        [Test]
        public void Initialize_withoutComposition_throwsArgumentNullException()
        {
            // assert:
            Assert.Throws<ArgumentNullException>(delegate {
                new PartialEvaluator(null);
            });
        }
        
        [Test]
        public void Visit_aClosedInstanceField_returnTheConstantValue()
        {
            // arrange:
            int constantValue = 10;
            ClassWithFieldAndProperty closure = new ClassWithFieldAndProperty();
            closure.Field = constantValue;
            Expression<Func<int>> expressionToVisit = () => closure.Field;
            FakeNormalizer normalizer = new FakeNormalizer();
            new PartialEvaluator(normalizer);

            // act:
            Expression result = normalizer.Visit(expressionToVisit, VisitContext.New);

            // assert:
            Verify.That(result).IsA<Expression<Func<int>>>()
                .WithA(e => e.Body, body => Verify.That(body).IsA<ConstantExpression>().WithA(c => c.Value, valueThat => Is.EqualTo(constantValue)));
        }

        [Test]
        public void Visit_aClosedInstanceProperty_returnTheConstantValue()
        {
            // arrange:
            string constantValue = "constantValue";
            ClassWithFieldAndProperty closure = new ClassWithFieldAndProperty();
            closure.Property = constantValue;
            Expression<Func<string>> expressionToVisit = () => closure.Property;
            FakeNormalizer normalizer = new FakeNormalizer();
            new PartialEvaluator(normalizer);

            // act:
            Expression result = normalizer.Visit(expressionToVisit, VisitContext.New);

            // assert:
            Verify.That(result).IsA<Expression<Func<string>>>()
                .WithA(e => e.Body, body => Verify.That(body).IsA<ConstantExpression>().WithA(c => c.Value, valueThat => Is.EqualTo(constantValue)));
        }

        [Test]
        public void Visit_aClosedInstancePropertysPropertyAccess_returnTheConstantValue()
        {
            // arrange:
            string constantValue = "constantValue";
            ClassWithFieldAndProperty closure = new ClassWithFieldAndProperty();
            closure.Property = constantValue;
            Expression<Func<int>> expressionToVisit = () => closure.Property.Length;
            FakeNormalizer normalizer = new FakeNormalizer();
            new PartialEvaluator(normalizer);

            // act:
            Expression result = normalizer.Visit(expressionToVisit, VisitContext.New);

            // assert:
            Verify.That(result).IsA<Expression<Func<int>>>()
                .WithA(e => e.Body, body => Verify.That(body).IsA<ConstantExpression>().WithA(c => c.Value, valueThat => Is.EqualTo(constantValue.Length)));
        }

        [Test]
        public void Visit_aClosedInstancePropertyMethodCall_returnTheConstantValue()
        {
            // arrange:
            string constantValue = "constantValue";
            ClassWithFieldAndProperty closure = new ClassWithFieldAndProperty();
            closure.Property = constantValue;
            Expression<Func<string>> expressionToVisit = () => closure.Property.ToLower();
            FakeNormalizer normalizer = new FakeNormalizer();
            new PartialEvaluator(normalizer);

            // act:
            Expression result = normalizer.Visit(expressionToVisit, VisitContext.New);

            // assert:
            Verify.That(result).IsA<Expression<Func<string>>>()
                .WithA(e => e.Body, body => Verify.That(body).IsA<ConstantExpression>().WithA(c => c.Value, valueThat => Is.EqualTo(constantValue.ToLower())));
        }

        [Test]
        public void Visit_aClosedInstancePropertyMethodCallWithValues_returnTheConstantValue()
        {
            // arrange:
            string constantValue = "constantValue";
            ClassWithFieldAndProperty closure = new ClassWithFieldAndProperty();
            closure.Property = constantValue;
            Expression<Func<string>> expressionToVisit = () => closure.Property.Substring(8);
            FakeNormalizer normalizer = new FakeNormalizer();
            new PartialEvaluator(normalizer);

            // act:
            Expression result = normalizer.Visit(expressionToVisit, VisitContext.New);

            // assert:
            Verify.That(result).IsA<Expression<Func<string>>>()
                .WithA(e => e.Body, body => Verify.That(body).IsA<ConstantExpression>().WithA(c => c.Value, valueThat => Is.EqualTo(constantValue.Substring(8))));
        }

        [Test]
        public void Visit_aParameterField_returnTheSameExpression()
        {
            // arrange:
            Expression<Func<ClassWithFieldAndProperty, string>> expressionToVisit = c => c.Property;
            FakeNormalizer normalizer = new FakeNormalizer();
            new PartialEvaluator(normalizer);

            // act:
            Expression result = normalizer.Visit(expressionToVisit, VisitContext.New);

            // assert:
            Assert.AreSame(expressionToVisit, result);
        }


        [Test]
        public void Visit_aParametersMethodCall_returnTheSameExpression()
        {
            // arrange:
            Expression<Func<string, string>> expressionToVisit = c => c.ToLower();
            FakeNormalizer normalizer = new FakeNormalizer();
            new PartialEvaluator(normalizer);

            // act:
            Expression result = normalizer.Visit(expressionToVisit, VisitContext.New);

            // assert:
            Assert.AreSame(expressionToVisit, result);
        }

        [Test]
        public void Visit_aParameterFieldWhichExpressionIsReplacedFromAnotherVisitor_returnANewExpressionButDontEvaluateAnything()
        {
            // arrange:
            IVisitContext context = VisitContext.New;
            ParameterExpression newParameter = Expression.Parameter(typeof(ClassWithFieldAndProperty), "p");
            Expression<Func<ClassWithFieldAndProperty, string>> dummy = c => c.Property;
            MemberExpression expressionToVisit = (MemberExpression)dummy.Body;
            FakeNormalizer normalizer = new FakeNormalizer();
            CompositeVisitor<Expression>.VisitorBase mockable = GeneratePartialMock<CompositeVisitor<Expression>.VisitorBase, IDerivedExpressionsVisitor>(normalizer);
            IDerivedExpressionsVisitor mockableInterceptor = mockable as IDerivedExpressionsVisitor;
            mockableInterceptor.Expect(v => v.Visit(dummy.Parameters[0], context)).Return(newParameter).Repeat.Once();
            new PartialEvaluator(normalizer);

            // act:
            Expression result = normalizer.Visit(expressionToVisit, context);

            // assert:
            Assert.AreNotSame(expressionToVisit, result);
            Verify.That(result).IsA<MemberExpression>()
                .WithA(e => e.Expression, that => Is.SameAs(newParameter));
        }

        [Test]
        public void Visit_aCallToAMethodWithArgumentsThatCanNotBeCompiled_returnsANewExpressionButDontEvaluateArguments()
        {
            IVisitContext context = VisitContext.New;
            string constantValue = "constantValue";
            ClassWithFieldAndProperty closure = new ClassWithFieldAndProperty();
            closure.Property = constantValue;
            Expression<Func<int, string>> expressionToVisit = startIndex => closure.Property.Substring(startIndex);
            FakeNormalizer normalizer = new FakeNormalizer();
            new PartialEvaluator(normalizer);

            // act:
            Expression result = normalizer.Visit(expressionToVisit, context);

            // assert:
            Assert.AreNotSame(expressionToVisit, result);
            Verify.That(result).IsA<Expression<Func<int, string>>>()
                .WithA(e => e.Parameters[0], that => Is.SameAs(expressionToVisit.Parameters[0]))
                .WithA(e => e.Body, body => Verify.That(body).IsA<MethodCallExpression>()
                                                .WithA(call => call.Object, obj => Verify.That(obj).IsA<ConstantExpression>()
                                                                                        .WithA(o => o.Value, that => Is.EqualTo(constantValue))));
        }

        [Test]
        public void Visit_aReplacedParameterMethod_returnANewExpressionButDontEvaluateAnything()
        {
            // arrange:
            IVisitContext context = VisitContext.New;
            ParameterExpression newParameter = Expression.Parameter(typeof(string), "p");
            Expression<Func<string, string>> dummy = s => s.ToLower();
            MethodCallExpression expressionToVisit = (MethodCallExpression)dummy.Body;
            FakeNormalizer normalizer = new FakeNormalizer();
            CompositeVisitor<Expression>.VisitorBase mockable = GeneratePartialMock<CompositeVisitor<Expression>.VisitorBase, IDerivedExpressionsVisitor>(normalizer);
            IDerivedExpressionsVisitor mockableInterceptor = mockable as IDerivedExpressionsVisitor;
            mockableInterceptor.Expect(v => v.Visit(dummy.Parameters[0], context)).Return(newParameter).Repeat.Once();
            new PartialEvaluator(normalizer);

            // act:
            Expression result = normalizer.Visit(expressionToVisit, context);

            // assert:
            Assert.AreNotSame(expressionToVisit, result);
            Verify.That(result).IsA<MethodCallExpression>()
                .WithA(e => e.Object, that => Is.SameAs(newParameter))
                .WithA(e => e.Method, that => Is.SameAs(expressionToVisit.Method));
        }
    }
}
