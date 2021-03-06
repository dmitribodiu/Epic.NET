//
//  VariantQA.cs
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
using NUnit.Framework;
using System;
using Rhino.Mocks;

namespace Epic.Specifications
{
    [TestFixture()]
    public class VariantQA : RhinoMocksFixtureBase
    {
        public static readonly ISpecification<Fakes.FakeCandidate1> q = new Fakes.NamedSpecification<Fakes.FakeCandidate1>("q");
        public static readonly ISpecification<Fakes.FakeCandidate1> r = new Fakes.NamedSpecification<Fakes.FakeCandidate1>("r");
        static object[] ToStringSource =
        {
            new object[] {
                q.OfType<Fakes.FakeCandidate1Abstraction>(), 
                "q⇗FakeCandidate1Abstraction"
            },
            new object[] {
                q.OfType<Fakes.FakeCandidate1Specialization>(), 
                "q⇘FakeCandidate1Specialization"
            },
            new object[] {
                q.And(r).OfType<Fakes.FakeCandidate1Abstraction>(), 
                "(q ∧ r)⇗FakeCandidate1Abstraction"
            },
            new object[] {
                q.Or(r).OfType<Fakes.FakeCandidate1Specialization>(), 
                "(q ∨ r)⇘FakeCandidate1Specialization"
            },
            new object[] {
                q.Negate().OfType<Fakes.FakeCandidate1Abstraction>(), 
                "(¬q)⇗FakeCandidate1Abstraction"
            }
        };
        
        [Test, TestCaseSource("ToStringSource")]
        public void ToString_OfAVariant_works(ISpecification toTest, string expression)
        {
            // act:
            string result = toTest.ToString();
            
            // assert:
            Assert.AreEqual(expression, result);
        }
        [Test]
        public void Initialize_withASpecification_works ()
        {
            // arrange:
            ISpecification<Fakes.FakeCandidate1> inner1 = GenerateStrictMock<ISpecification<Fakes.FakeCandidate1>>();
            inner1.Expect(s => s.CandidateType).Return(typeof(Fakes.FakeCandidate1)).Repeat.Once();
            ISpecification<Fakes.FakeCandidate1Abstraction> inner2 = GenerateStrictMock<ISpecification<Fakes.FakeCandidate1Abstraction>>();

            // act:
            var toTestUpcasting = new Variant<Fakes.FakeCandidate1, Fakes.FakeCandidate1Abstraction>(inner1);
            var toTestDowncasting = new Variant<Fakes.FakeCandidate1Abstraction, Fakes.FakeCandidate1>(inner2);

            // assert:
            Assert.IsNotNull(toTestUpcasting);
            Assert.AreSame(inner1, toTestUpcasting.InnerSpecification);
            Assert.AreSame(inner1, (toTestUpcasting as IMonadicSpecificationComposition<Fakes.FakeCandidate1Abstraction>).Operand);
            Assert.AreEqual(typeof(Fakes.FakeCandidate1), (toTestUpcasting as ISpecification<Fakes.FakeCandidate1Abstraction>).CandidateType);
            Assert.IsNotNull(toTestDowncasting);
            Assert.AreSame(inner2, toTestDowncasting.InnerSpecification);
            Assert.AreSame(inner2, (toTestDowncasting as IMonadicSpecificationComposition<Fakes.FakeCandidate1Abstraction>).Operand);
            Assert.AreEqual(typeof(Fakes.FakeCandidate1), (toTestDowncasting as ISpecification<Fakes.FakeCandidate1>).CandidateType);
        }

        [Test]
        public void Initialize_withoutASpecification_throwsArgumentNullException ()
        {
            // assert:
            Assert.Throws<ArgumentNullException>(delegate() {
                new Variant<Fakes.FakeCandidate1Abstraction, Fakes.FakeCandidate1>(null);
            });
        }


        [Test]
        public void Initialize_withTwoEqualsCandidateTypes_throwTypeInitializationException ()
        {
            // arrange:
            ISpecification<Fakes.FakeCandidate1> inner = GenerateStrictMock<ISpecification<Fakes.FakeCandidate1>>();
            
            // act:
            Assert.Throws<TypeInitializationException>(delegate() {
                new Variant<Fakes.FakeCandidate1, Fakes.FakeCandidate1>(inner);
            });
        }
        
        [Test]
        public void Equals_withAnotherWithEqualsInner_isTrue ()
        {
            // arrange:
            ISpecification<Fakes.FakeCandidate1> inner = GenerateStrictMock<ISpecification<Fakes.FakeCandidate1>>();
            inner.Expect(s => s.Equals(inner)).Return(true).Repeat.AtLeastOnce();
            var toTest1 = new Variant<Fakes.FakeCandidate1, Fakes.FakeCandidate1Abstraction>(inner);
            var other = new Variant<Fakes.FakeCandidate1, Fakes.FakeCandidate1Abstraction>(inner);

            // act:
            bool result = toTest1.Equals(other);

            // assert:
            Assert.IsTrue(result);
        }

        [Test]
        public void IsSatisfiedBy_Candidates_thatSatisfyTheInnerSpecification ()
        {
            // arrange:
            Fakes.FakeCandidate1 candidate1 = new Epic.Fakes.FakeCandidate1();
            Fakes.FakeCandidate1 candidate2 = new Epic.Fakes.FakeCandidate1();
            Fakes.FakeCandidate1Abstraction wrongTypeAbstraction = GeneratePartialMock<Fakes.FakeCandidate1Abstraction>();
            ISpecification<Fakes.FakeCandidate1> inner = GenerateStrictMock<ISpecification<Fakes.FakeCandidate1>>();
            inner.Expect(s => s.IsSatisfiedBy(candidate1)).Return(true).Repeat.Once();
            inner.Expect(s => s.IsSatisfiedBy(candidate2)).Return(false).Repeat.Once();
            inner.Expect(s => s.IsSatisfiedBy(null)).Return(false).Repeat.Once();
            ISpecification<Fakes.FakeCandidate1Abstraction> toTest = new Variant<Fakes.FakeCandidate1, Fakes.FakeCandidate1Abstraction>(inner);

            // act:
            bool candidate1Result = toTest.IsSatisfiedBy(candidate1);
            bool candidate2Result = toTest.IsSatisfiedBy(candidate2);
            bool wrongTypeResult = toTest.IsSatisfiedBy(wrongTypeAbstraction);

            // assert:
            Assert.IsTrue(candidate1Result);
            Assert.IsFalse(candidate2Result);
            Assert.IsFalse(wrongTypeResult);
        }

        [Test]
        public void OfType_withANotImplementedCandidate_returnsANoSpecification ()
        {
            // arrange:
            ISpecification<Fakes.FakeCandidate1> inner = GenerateStrictMock<ISpecification<Fakes.FakeCandidate1>>();
            ISpecification<Fakes.FakeCandidate1Abstraction> toTest = new Variant<Fakes.FakeCandidate1, Fakes.FakeCandidate1Abstraction>(inner);
            
            
            // act:
            var result = toTest.OfType<Fakes.FakeCandidate2>();
            
            // assert:
            Assert.AreSame(No<Fakes.FakeCandidate2>.Specification, result);
        }

        [Test]
        public void OfType_withAMoreDerivedCandidateOnAnUpcastingVariantSpecification_asksToTheInnerSpecification ()
        {
            // arrange:
            ISpecification<Fakes.FakeCandidate1Specialization> expectedResult = GenerateStrictMock<ISpecification<Fakes.FakeCandidate1Specialization>>();
            ISpecification<Fakes.FakeCandidate1> inner = GenerateStrictMock<ISpecification<Fakes.FakeCandidate1>>();
            inner.Expect(s => s.OfType<Fakes.FakeCandidate1Specialization>()).Return(expectedResult).Repeat.Once();
            ISpecification<Fakes.FakeCandidate1Abstraction> toTest = new Variant<Fakes.FakeCandidate1, Fakes.FakeCandidate1Abstraction>(inner);


            // act:
            var result = toTest.OfType<Fakes.FakeCandidate1Specialization>();

            // assert:
            Assert.AreSame(expectedResult, result);
        }

        [Test]
        public void OfType_withAMoreDerivedCandidateOnAnDowncastingVariantSpecification_asksToTheInnerSpecification ()
        {
            // arrange:
            ISpecification<Fakes.FakeCandidate1Specialization> expectedResult = GenerateStrictMock<ISpecification<Fakes.FakeCandidate1Specialization>>();
            ISpecification<Fakes.FakeCandidate1Abstraction> inner = GenerateStrictMock<ISpecification<Fakes.FakeCandidate1Abstraction>>();
            inner.Expect(s => s.OfType<Fakes.FakeCandidate1Specialization>()).Return(expectedResult).Repeat.Once();
            ISpecification<Fakes.FakeCandidate1> toTest = new Variant<Fakes.FakeCandidate1Abstraction, Fakes.FakeCandidate1>(inner);
            
            
            // act:
            var result = toTest.OfType<Fakes.FakeCandidate1Specialization>();
            
            // assert:
            Assert.AreSame(expectedResult, result);
        }

        [Test]
        public void OfType_withAnIntermediateCandidateOnAnDowncastingVariantSpecification_returnsANewVariantWithTheCurrentVariantAsInner ()
        {
            // arrange:
            ISpecification<Fakes.FakeCandidate1Abstraction> inner = GenerateStrictMock<ISpecification<Fakes.FakeCandidate1Abstraction>>();
            ISpecification<Fakes.FakeCandidate1Specialization> toTest = new Variant<Fakes.FakeCandidate1Abstraction, Fakes.FakeCandidate1Specialization>(inner);
            
            
            // act:
            var result = toTest.OfType<Fakes.FakeCandidate1>();
            
            // assert:
            Assert.IsInstanceOf<Variant<Fakes.FakeCandidate1Specialization, Fakes.FakeCandidate1>>(result);
            Assert.AreSame(toTest, (result as Variant<Fakes.FakeCandidate1Specialization, Fakes.FakeCandidate1>).InnerSpecification);
        }
    }
}

