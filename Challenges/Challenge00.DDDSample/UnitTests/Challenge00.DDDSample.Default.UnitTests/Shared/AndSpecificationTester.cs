using System;
using NUnit.Framework;
using Challenge00.DDDSample.Default.Shared;
using Challenge00.DDDSample.Shared;
using Rhino.Mocks;
namespace Challenge00.DDDSample.Default.UnitTests
{
	[TestFixture]
	public class AndSpecificationTester : AbstractSpecificationTester<AndSpecification<object>, object>
	{
		#region implemented abstract members of Challenge00.DDDSample.Default.UnitTests.AbstractSpecificationTester[AndSpecification[System.Object],System.Object]
		protected override void CreateEqualsSpecification (out AndSpecification<object> spec1, out AndSpecification<object> spec2)
		{
			ISpecification<object> left = MockRepository.GenerateMock<ISpecification<object>>();
			left.Stub(s => s.Equals(left)).Return(true).Repeat.Any();
			ISpecification<object> right = MockRepository.GenerateMock<ISpecification<object>>();
			right.Stub(s => s.Equals(right)).Return(true).Repeat.Any();
			
			right.Stub(s => s.Equals(left)).Return(false).Repeat.Any();
			left.Stub(s => s.Equals(right)).Return(false).Repeat.Any();
			
			spec1 = new AndSpecification<object>(left, right);
			spec2 = new AndSpecification<object>(left, right);
		}
		
		
		protected override void CreateDifferentSpecification (out AndSpecification<object> spec1, out AndSpecification<object> spec2)
		{
			ISpecification<object> left = MockRepository.GenerateMock<ISpecification<object>>();
			left.Stub(s => s.Equals(left)).Return(true).Repeat.Any();
			ISpecification<object> right = MockRepository.GenerateMock<ISpecification<object>>();
			right.Stub(s => s.Equals(right)).Return(true).Repeat.Any();
			

			spec1 = new AndSpecification<object>(right, right);
			spec2 = new AndSpecification<object>(left, left);
		}
		
		
		protected override AndSpecification<object> CreateNewSpecification ()
		{
			ISpecification<object> left = MockRepository.GenerateMock<ISpecification<object>>();
			left.Stub(s => s.Equals(left)).Return(true).Repeat.Any();
			ISpecification<object> right = MockRepository.GenerateMock<ISpecification<object>>();
			right.Stub(s => s.Equals(right)).Return(true).Repeat.Any();
			
			right.Stub(s => s.Equals(left)).Return(false).Repeat.Any();
			left.Stub(s => s.Equals(right)).Return(false).Repeat.Any();
			
			return new AndSpecification<object>(left, right);
		}
		
		#endregion
		
		[Test]
		public void Test_Ctor_01()
		{
			// arrange:
			ISpecification<object> left = MockRepository.GenerateMock<ISpecification<object>>();
			ISpecification<object> right = MockRepository.GenerateMock<ISpecification<object>>();	
			
			// act:
			AndSpecification<object> spec = new AndSpecification<object>(left,right);
		
			// assert:
			Assert.IsNotNull(spec);
			Assert.AreSame(left, spec.First);
			Assert.AreSame(right, spec.Second);
		}
		
		[Test()]
		[ExpectedException(typeof(ArgumentNullException))]
		public void Test_Ctor_02 ()
		{
			// arrange:
			ISpecification<object> right = MockRepository.GenerateMock<ISpecification<object>>();	
			
			// act:
			AndSpecification<object> spec = new AndSpecification<object>(null,right);
		}
		
		[Test()]
		[ExpectedException(typeof(ArgumentNullException))]
		public void Test_Ctor_03 ()
		{
			// arrange:
			ISpecification<object> left = MockRepository.GenerateMock<ISpecification<object>>();	
			
			// act:
			AndSpecification<object> spec = new AndSpecification<object>(left,null);
		}
		
		[Test]
		public void Test_IsSatisfiedBy_01()
		{
			// arrange:
			object candidate = new object();
			ISpecification<object> left = MockRepository.GenerateMock<ISpecification<object>>();
			left.Expect(s => s.IsSatisfiedBy(candidate)).Return(true);
			ISpecification<object> right = MockRepository.GenerateMock<ISpecification<object>>();	
			right.Expect(s => s.IsSatisfiedBy(candidate)).Return(true);
			
		
			// act:
			ISpecification<object> target = new AndSpecification<object>(left, right);
			bool satisfied = target.IsSatisfiedBy(candidate);
		
			// assert:
			Assert.IsTrue(satisfied);
		 	left.VerifyAllExpectations();
			right.VerifyAllExpectations();
		}
		
		[Test]
		public void Test_IsSatisfiedBy_02()
		{
			// arrange:
			object candidate = new object();
			ISpecification<object> left = MockRepository.GenerateMock<ISpecification<object>>();
			left.Expect(s => s.IsSatisfiedBy(candidate)).Return(true);
			ISpecification<object> right = MockRepository.GenerateMock<ISpecification<object>>();	
			right.Expect(s => s.IsSatisfiedBy(candidate)).Return(false);
			
		
			// act:
			ISpecification<object> target = new AndSpecification<object>(left, right);
			bool satisfied = target.IsSatisfiedBy(candidate);
		
			// assert:
			Assert.IsFalse(satisfied);
		 	left.VerifyAllExpectations();
			right.VerifyAllExpectations();
		}
		
		[Test]
		public void Test_IsSatisfiedBy_03()
		{
			// arrange:
			object candidate = new object();
			ISpecification<object> left = MockRepository.GenerateMock<ISpecification<object>>();
			left.Expect(s => s.IsSatisfiedBy(candidate)).Return(false);
			ISpecification<object> right = MockRepository.GenerateMock<ISpecification<object>>();	
			
		
			// act:
			ISpecification<object> target = new AndSpecification<object>(left, right);
			bool satisfied = target.IsSatisfiedBy(candidate);
		
			// assert:
			Assert.IsFalse(satisfied);
		 	left.VerifyAllExpectations();
			right.AssertWasNotCalled(s => s.IsSatisfiedBy(candidate));
		}
		
		[Test]
		public void Test_IsSatisfiedBy_04()
		{
			// arrange:
			object candidate = new object();
			ISpecification<object> left = MockRepository.GenerateMock<ISpecification<object>>();
			left.Expect(s => s.IsSatisfiedBy(candidate)).Return(false);
			ISpecification<object> right = MockRepository.GenerateMock<ISpecification<object>>();	
			right.Expect(s => s.IsSatisfiedBy(candidate)).Return(false);
			
		
			// act:
			ISpecification<object> target = new AndSpecification<object>(left, right);
			bool satisfied = target.IsSatisfiedBy(candidate);
		
			// assert:
			Assert.IsFalse(satisfied);
		 	left.VerifyAllExpectations();
            right.AssertWasNotCalled(s => s.IsSatisfiedBy(candidate));
        }
	}
}

