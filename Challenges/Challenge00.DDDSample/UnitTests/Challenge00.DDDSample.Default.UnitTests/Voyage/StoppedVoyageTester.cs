//  
//  JustCreatedVoyageTester.cs
//  
//  Author:
//       Giacomo Tesio <giacomo@tesio.it>
// 
//  Copyright (c) 2010 Giacomo Tesio
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
using Challenge00.DDDSample.Voyage;
using Rhino.Mocks;
using Challenge00.DDDSample.Location;
namespace DefaultImplementation.Voyage
{
	[TestFixture()]
	public class StoppedVoyageTester
	{
		[TestCase(0)]
		[TestCase(1)]
		[TestCase(2)]
		public void Test_Ctor_01(int index)
		{
			// arrange:
			ISchedule schedule = MockRepository.GenerateStrictMock<ISchedule>();
			schedule.Expect(s => s.MovementsCount).Return(3).Repeat.Any();
		
			// act:
			StoppedVoyage state = new StoppedVoyage(schedule, index);
		
			// assert:
			Assert.AreSame(schedule, state.Schedule);
			Assert.IsFalse(state.IsMoving);
		}
		
		[Test()]
		[ExpectedException(typeof(ArgumentNullException))]
		public void Test_Ctor_02 ()
		{
			// arrange:
			
		
			// act:
			new StoppedVoyage(null, 0);
		}
		
		[TestCase(-1)]
		[TestCase(3)]
		[TestCase(4)]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void Test_Ctor_03(int index)
		{
			// arrange:
			ISchedule schedule = MockRepository.GenerateStrictMock<ISchedule>();
			schedule.Expect(s => s.MovementsCount).Return(3).Repeat.Any();
		
			// act:
			new StoppedVoyage(schedule, index);
		}
		
		[TestCase(0)]
		[TestCase(1)]
		[TestCase(2)]
		public void Test_LastKnownLocation_01(int index)
		{
			// arrange:
			ISchedule schedule = MockRepository.GenerateStrictMock<ISchedule>();
			schedule.Expect(s => s.MovementsCount).Return(3).Repeat.Any();
			UnLocode initialLocation = new UnLocode("DPLOC");
			ICarrierMovement movement = MockRepository.GenerateStrictMock<ICarrierMovement>();
			movement.Expect(m => m.DepartureLocation).Return(initialLocation).Repeat.Once();
			schedule.Expect(s => s[index]).Return(movement).Repeat.Once();
		
			// act:
			StoppedVoyage state = new StoppedVoyage(schedule, index);
		
			// assert:
			Assert.AreSame(initialLocation, state.LastKnownLocation);
			movement.VerifyAllExpectations();
			schedule.VerifyAllExpectations();
		}
		
	
		[TestCase(0)]
		[TestCase(1)]
		[TestCase(2)]
		public void Test_NextExpectedLocation_01(int index)
		{
			// arrange:
			ISchedule schedule = MockRepository.GenerateStrictMock<ISchedule>();
			schedule.Expect(s => s.MovementsCount).Return(3).Repeat.Any();
			UnLocode initialLocation = new UnLocode("ARLOC");
			ICarrierMovement movement = MockRepository.GenerateStrictMock<ICarrierMovement>();
			movement.Expect(m => m.ArrivalLocation).Return(initialLocation).Repeat.Once();
			schedule.Expect(s => s[index]).Return(movement).Repeat.Once();
		
			// act:
			StoppedVoyage state = new StoppedVoyage(schedule, index);
		
			// assert:
			Assert.AreSame(initialLocation, state.NextExpectedLocation);
			movement.VerifyAllExpectations();
			schedule.VerifyAllExpectations();
		}

		[TestCase(0)]
		[TestCase(1)]
		[TestCase(2)]
		public void Test_StopOverAt_01(int index)
		{
			// arrange:
			ISchedule schedule = MockRepository.GenerateStrictMock<ISchedule>();
			schedule.Expect(s => s.MovementsCount).Return(3).Repeat.Any();
			UnLocode initialLocation = new UnLocode("DPLOC");
			ICarrierMovement movement = MockRepository.GenerateStrictMock<ICarrierMovement>();
			movement.Expect(m => m.DepartureLocation).Return(initialLocation).Repeat.Once();
			schedule.Expect(s => s[index]).Return(movement).Repeat.Once();
			ILocation location = MockRepository.GenerateStrictMock<ILocation>();
			location.Expect(l => l.UnLocode).Return(initialLocation).Repeat.Any();

			
			// act:
			StoppedVoyage state = new StoppedVoyage(schedule, index);
			VoyageState arrived = state.StopOverAt(location);
		
			// assert:
			Assert.AreSame(state, arrived);
			schedule.VerifyAllExpectations();
			movement.VerifyAllExpectations();
			location.VerifyAllExpectations();
		}
		
		[TestCase(0)]
		[TestCase(1)]
		[TestCase(2)]
		public void Test_StopOverAt_02(int index)
		{
			// arrange:
			ISchedule schedule = MockRepository.GenerateStrictMock<ISchedule>();
			schedule.Expect(s => s.MovementsCount).Return(3).Repeat.Any();
			UnLocode initialLocation = new UnLocode("DPLOC");
			ICarrierMovement movement = MockRepository.GenerateStrictMock<ICarrierMovement>();
			movement.Expect(m => m.DepartureLocation).Return(initialLocation).Repeat.Twice();
			schedule.Expect(s => s[index]).Return(movement).Repeat.Twice();
			ILocation location = MockRepository.GenerateStrictMock<ILocation>();
			location.Expect(l => l.UnLocode).Return(new UnLocode("ANTHR")).Repeat.Any();
			
			// act:
			StoppedVoyage state = new StoppedVoyage(schedule, index);

			// assert:
			Assert.Throws<ArgumentException>(delegate {state.StopOverAt(location);});
			schedule.VerifyAllExpectations();
			movement.VerifyAllExpectations();
			location.VerifyAllExpectations();
		}
		
		[TestCase(0)]
		[TestCase(1)]
		[TestCase(2)]
		public void Test_DepartFrom_01(int index)
		{
			// arrange:
			ISchedule schedule = MockRepository.GenerateStrictMock<ISchedule>();
			schedule.Expect(s => s.MovementsCount).Return(3).Repeat.Any();
			UnLocode initialLocation = new UnLocode("DPLOC");
			UnLocode destinationLocation = new UnLocode("ARLOC");
			ICarrierMovement movement = MockRepository.GenerateStrictMock<ICarrierMovement>();
			movement.Expect(m => m.DepartureLocation).Return(initialLocation).Repeat.Any();
			movement.Expect(m => m.ArrivalLocation).Return(destinationLocation).Repeat.Any();
			schedule.Expect(s => s[index]).Return(movement).Repeat.Any();
			ILocation location = MockRepository.GenerateStrictMock<ILocation>();
			location.Expect(l => l.UnLocode).Return(initialLocation).Repeat.Any();
			
			// act:
			StoppedVoyage state = new StoppedVoyage(schedule, index);
			VoyageState moving = state.DepartFrom(location);
		
			// assert:
			Assert.IsInstanceOf<MovingVoyage>(moving);
			Assert.AreSame(state.LastKnownLocation, moving.LastKnownLocation);
			Assert.AreSame(state.NextExpectedLocation, moving.NextExpectedLocation);
			schedule.VerifyAllExpectations();
			movement.VerifyAllExpectations();
			location.VerifyAllExpectations();
		} 
		
		[TestCase(0)]
		[TestCase(1)]
		[TestCase(2)]
		public void Test_DepartFrom_02(int index)
		{
			// arrange:
			ISchedule schedule = MockRepository.GenerateStrictMock<ISchedule>();
			schedule.Expect(s => s.MovementsCount).Return(3).Repeat.Any();
			UnLocode initialLocation = new UnLocode("DPLOC");
			ICarrierMovement movement = MockRepository.GenerateStrictMock<ICarrierMovement>();
			movement.Expect(m => m.DepartureLocation).Return(initialLocation).Repeat.Any();
			schedule.Expect(s => s[index]).Return(movement).Repeat.Any();
			ILocation location = MockRepository.GenerateStrictMock<ILocation>();
			location.Expect(l => l.UnLocode).Return(new UnLocode("ANTHR")).Repeat.Any();
			
			// act:
			StoppedVoyage state = new StoppedVoyage(schedule, index);

			// assert:
			Assert.Throws<ArgumentException>(delegate {state.DepartFrom(location);});
			schedule.VerifyAllExpectations();
			movement.VerifyAllExpectations();
			location.VerifyAllExpectations();
		}
		
		[TestCase(0)]
		[TestCase(1)]
		[TestCase(2)]
		public void Test_Equals_01(int index)
		{
			// arrange:
			ISchedule schedule = MockRepository.GenerateStrictMock<ISchedule>();
			schedule.Expect(s => s.MovementsCount).Return(3).Repeat.Any();
			schedule.Expect(s => s.Equals(schedule)).Return(true).Repeat.Any();

			// act:
			StoppedVoyage state1 = new StoppedVoyage(schedule, index);
			StoppedVoyage state2 = new StoppedVoyage(schedule, index);
			
			// assert:
			Assert.IsFalse(state1.Equals(null));
			Assert.IsTrue(state1.Equals(state1));
			Assert.IsTrue(state1.Equals(state2));
		}
		
		[TestCase(0)]
		[TestCase(1)]
		[TestCase(2)]
		public void Test_Equals_02(int index)
		{
			// arrange:
			ISchedule schedule = MockRepository.GenerateStrictMock<ISchedule>();
			schedule.Expect(s => s.MovementsCount).Return(4).Repeat.Any();
			schedule.Expect(s => s.Equals(schedule)).Return(true).Repeat.Any();

			// act:
			StoppedVoyage state1 = new StoppedVoyage(schedule, index);
			StoppedVoyage state2 = new StoppedVoyage(schedule, index + 1);
			
			// assert:
			Assert.IsFalse(state1.Equals(state2));
			Assert.IsFalse(state2.Equals(state1));
			schedule.VerifyAllExpectations();
		}

		[TestCase(0)]
		[TestCase(1)]
		[TestCase(2)]
		public void Test_Equals_03(int index)
		{
			// arrange:
			ISchedule schedule1 = MockRepository.GenerateStrictMock<ISchedule>();
			ISchedule schedule2 = MockRepository.GenerateStrictMock<ISchedule>();
			schedule1.Expect(s => s.MovementsCount).Return(3).Repeat.Any();
			schedule1.Expect(s => s.Equals(schedule2)).Return(false).Repeat.Any();
			schedule2.Expect(s => s.MovementsCount).Return(3).Repeat.Any();
			schedule2.Expect(s => s.Equals(schedule1)).Return(false).Repeat.Any();

			// act:
			StoppedVoyage state1 = new StoppedVoyage(schedule1, index);
			StoppedVoyage state2 = new StoppedVoyage(schedule2, index);
			
			// assert:
			Assert.IsFalse(state1.Equals(state2));
			Assert.IsFalse(state2.Equals(state1));
			schedule1.VerifyAllExpectations();
			schedule2.VerifyAllExpectations();
		}
		
		[TestCase(0)]
		[TestCase(1)]
		[TestCase(2)]
		public void Test_Equals_04(int index)
		{
			// arrange:
			ISchedule schedule = MockRepository.GenerateStrictMock<ISchedule>();
			schedule.Expect(s => s.MovementsCount).Return(3).Repeat.Any();
			schedule.Expect(s => s.Equals(schedule)).Return(true).Repeat.Any();

			// act:
			StoppedVoyage state1 = new StoppedVoyage(schedule, index);
			VoyageState state2 = new MovingVoyage(schedule, index);
			
			// assert:
			Assert.IsFalse(state1.Equals(state2));
			schedule.VerifyAllExpectations();
		}
		
		[TestCase(0)]
		[TestCase(1)]
		[TestCase(2)]
		public void Test_Equals_05(int index)
		{
			// arrange:
			ISchedule schedule = MockRepository.GenerateStrictMock<ISchedule>();
			schedule.Expect(s => s.MovementsCount).Return(3).Repeat.Any();

			// act:
			StoppedVoyage state1 = new StoppedVoyage(schedule, index);
			VoyageState state2 = MockRepository.GeneratePartialMock<VoyageState>(schedule);
			
			// assert:
			Assert.IsFalse(state1.Equals(state2));
			schedule.VerifyAllExpectations();
			state2.VerifyAllExpectations();
		}
	}
}

