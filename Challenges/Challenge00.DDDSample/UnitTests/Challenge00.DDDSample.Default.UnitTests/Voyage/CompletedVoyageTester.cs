//  
//  CompletedVoyageTester.cs
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
using System;
using NUnit.Framework;
using Challenge00.DDDSample.Voyage;
using Rhino.Mocks;
using Challenge00.DDDSample.Location;
namespace DefaultImplementation.Voyage
{
	[TestFixture()]
	public class CompletedVoyageTester
	{
		[Test]
		public void Ctor_01()
		{
			// arrange:
			VoyageNumber number = new VoyageNumber("VYGTEST01");
			ISchedule schedule = MockRepository.GenerateStrictMock<ISchedule>();
			schedule.Expect(s => s.MovementsCount).Return(3).Repeat.Any();
		
			// act:
			CompletedVoyage state = new CompletedVoyage(number, schedule);
		
			// assert:
			Assert.AreSame(schedule, state.Schedule);
			Assert.IsFalse(state.IsMoving);
		}
		
		[Test()]
		[ExpectedException(typeof(ArgumentNullException))]
		public void Ctor_02 ()
		{
			// arrange:
			VoyageNumber number = new VoyageNumber("VYGTEST01");
		
			// act:
			new CompletedVoyage(number, null);
		}
		
		[Test()]
		[ExpectedException(typeof(ArgumentNullException))]
		public void Ctor_03 ()
		{
			// arrange:
			ISchedule schedule = MockRepository.GenerateStrictMock<ISchedule>();
			schedule.Expect(s => s.MovementsCount).Return(3).Repeat.Any();
		
			// act:
			new CompletedVoyage(null, schedule);
		}
		
		[Test]
		public void LastKnownLocation_01()
		{
			// arrange:
			VoyageNumber number = new VoyageNumber("VYGTEST01");
			ISchedule schedule = MockRepository.GenerateStrictMock<ISchedule>();
			schedule.Expect(s => s.MovementsCount).Return(3).Repeat.Any();
			UnLocode arrivalLocation = new UnLocode("ATEND");
			ICarrierMovement movement = MockRepository.GenerateStrictMock<ICarrierMovement>();
			movement.Expect(m => m.ArrivalLocation).Return(arrivalLocation).Repeat.Once();
			schedule.Expect(s => s[2]).Return(movement).Repeat.Once();
		
			// act:
			CompletedVoyage state = new CompletedVoyage(number, schedule);
		
			// assert:
			Assert.AreSame(arrivalLocation, state.LastKnownLocation);
			movement.VerifyAllExpectations();
			schedule.VerifyAllExpectations();
		}
		
	
		[Test]
		public void NextExpectedLocation_01()
		{
			// arrange:
			VoyageNumber number = new VoyageNumber("VYGTEST01");
			ISchedule schedule = MockRepository.GenerateStrictMock<ISchedule>();
			schedule.Expect(s => s.MovementsCount).Return(3).Repeat.Any();
			UnLocode arrivalLocation = new UnLocode("ATEND");
			ICarrierMovement movement = MockRepository.GenerateStrictMock<ICarrierMovement>();
			movement.Expect(m => m.ArrivalLocation).Return(arrivalLocation).Repeat.Once();
			schedule.Expect(s => s[2]).Return(movement).Repeat.Once();
		
			// act:
			CompletedVoyage state = new CompletedVoyage(number, schedule);
		
			// assert:
			Assert.AreSame(arrivalLocation, state.NextExpectedLocation);
			movement.VerifyAllExpectations();
			schedule.VerifyAllExpectations();
		}

		[Test]
		public void StopOverAt_01()
		{
			// arrange:
			VoyageNumber number = new VoyageNumber("VYGTEST01");
			ISchedule schedule = MockRepository.GenerateStrictMock<ISchedule>();
			
			// act:
			CompletedVoyage state = new CompletedVoyage(number, schedule);
			
			// assert:
			Assert.Throws<InvalidOperationException>(delegate {state.StopOverAt(MockRepository.GenerateStrictMock<ILocation>());});
			schedule.VerifyAllExpectations();
		}
		
		[Test]
		public void DepartFrom_01()
		{
			// arrange:
			VoyageNumber number = new VoyageNumber("VYGTEST01");
			ISchedule schedule = MockRepository.GenerateStrictMock<ISchedule>();
			
			// act:
			CompletedVoyage state = new CompletedVoyage(number, schedule);
			
			// assert:
			Assert.Throws<InvalidOperationException>(delegate {state.DepartFrom(MockRepository.GenerateStrictMock<ILocation>());});
			schedule.VerifyAllExpectations();

		} 
		
		[Test]
		public void Equals_01()
		{
			// arrange:
			VoyageNumber number = new VoyageNumber("VYGTEST01");
			ISchedule schedule = MockRepository.GenerateStrictMock<ISchedule>();
			schedule.Expect(s => s.MovementsCount).Return(3).Repeat.Any();
			schedule.Expect(s => s.Equals(schedule)).Return(true).Repeat.Any();

			// act:
			CompletedVoyage state1 = new CompletedVoyage(number, schedule);
			CompletedVoyage state2 = new CompletedVoyage(number, schedule);
			
			// assert:
			Assert.IsFalse(state1.Equals(null));
			Assert.IsTrue(state1.Equals(state1));
			Assert.IsTrue(state1.Equals(state2));
			Assert.IsTrue(state2.Equals(state1));
			Assert.IsTrue(state1.Equals((object)state1));
			Assert.IsTrue(state1.Equals((object)state2));
			Assert.IsTrue(state2.Equals((object)state1));
			schedule.VerifyAllExpectations();
		}
		
		[Test]
		public void Equals_02()
		{
			// arrange:
			VoyageNumber number = new VoyageNumber("VYGTEST01");
			ISchedule schedule = MockRepository.GenerateStrictMock<ISchedule>();
			schedule.Expect(s => s.MovementsCount).Return(4).Repeat.Any();
			schedule.Expect(s => s.Equals(schedule)).Return(true).Repeat.Any();

			// act:
			CompletedVoyage state1 = new CompletedVoyage(number, schedule);
			CompletedVoyage state2 = new CompletedVoyage(number, schedule);
			
			// assert:
			Assert.IsTrue(state1.Equals(state2));
			Assert.IsTrue(state2.Equals(state1));
			schedule.VerifyAllExpectations();
		}

		[Test]
		public void Equals_03()
		{
			// arrange:
			VoyageNumber number = new VoyageNumber("VYGTEST01");
			ISchedule schedule1 = MockRepository.GenerateStrictMock<ISchedule>();
			ISchedule schedule2 = MockRepository.GenerateStrictMock<ISchedule>();
			schedule1.Expect(s => s.MovementsCount).Return(3).Repeat.Any();
			schedule1.Expect(s => s.Equals(schedule2)).Return(false).Repeat.Any();
			schedule2.Expect(s => s.MovementsCount).Return(3).Repeat.Any();
			schedule2.Expect(s => s.Equals(schedule1)).Return(false).Repeat.Any();

			// act:
			CompletedVoyage state1 = new CompletedVoyage(number, schedule1);
			CompletedVoyage state2 = new CompletedVoyage(number, schedule2);
			
			// assert:
			Assert.IsFalse(state1.Equals(state2));
			Assert.IsFalse(state2.Equals(state1));
			schedule1.VerifyAllExpectations();
			schedule2.VerifyAllExpectations();
		}
		
		[Test]
		public void Equals_04()
		{
			// arrange:
			VoyageNumber number = new VoyageNumber("VYGTEST01");
			ISchedule schedule = MockRepository.GenerateStrictMock<ISchedule>();
			schedule.Expect(s => s.MovementsCount).Return(3).Repeat.Any();
			schedule.Expect(s => s.Equals(schedule)).Return(true).Repeat.Any();

			// act:
			CompletedVoyage state1 = new CompletedVoyage(number, schedule);
			VoyageState state2 = new MovingVoyage(number, schedule, 2);
			
			// assert:
			Assert.IsFalse(state1.Equals(state2));
			schedule.VerifyAllExpectations();
		}
		
		[Test]
		public void Equals_05()
		{
			// arrange:
			VoyageNumber number = new VoyageNumber("VYGTEST01");
			ISchedule schedule = MockRepository.GenerateStrictMock<ISchedule>();
			schedule.Expect(s => s.MovementsCount).Return(3).Repeat.Any();
			schedule.Expect(s => s.Equals(schedule)).Return(true).Repeat.Any();

			// act:
			CompletedVoyage state1 = new CompletedVoyage(number, schedule);
			VoyageState state2 = new StoppedVoyage(number, schedule, 2);
			
			// assert:
			Assert.IsFalse(state1.Equals(state2));
			schedule.VerifyAllExpectations();
		}
		
		[Test]
		public void Equals_06()
		{
			// arrange:
			VoyageNumber number = new VoyageNumber("VYGTEST01");
			ISchedule schedule = MockRepository.GenerateStrictMock<ISchedule>();
			schedule.Expect(s => s.MovementsCount).Return(3).Repeat.Any();

			// act:
			CompletedVoyage state1 = new CompletedVoyage(number, schedule);
			VoyageState state2 = MockRepository.GeneratePartialMock<VoyageState>(number, schedule);
			
			// assert:
			Assert.IsFalse(state1.Equals(state2));
			schedule.VerifyAllExpectations();
			state2.VerifyAllExpectations();
		}
		
		[Test]
		public void WillStopOverAt_01()
		{
			// arrange:
			VoyageNumber number = new VoyageNumber("VYGTEST01");
			ISchedule schedule = MockRepository.GenerateStrictMock<ISchedule>();
			schedule.Expect(s => s.MovementsCount).Return(3).Repeat.Any();
			ILocation location = MockRepository.GenerateStrictMock<ILocation>();
	
			// act:
			CompletedVoyage state = new CompletedVoyage(number, schedule);
			bool willStopOverAtLocation = state.WillStopOverAt(location);
		
			// assert:
			Assert.IsFalse(willStopOverAtLocation);
			location.VerifyAllExpectations();
			schedule.VerifyAllExpectations();
		}
	}
}

