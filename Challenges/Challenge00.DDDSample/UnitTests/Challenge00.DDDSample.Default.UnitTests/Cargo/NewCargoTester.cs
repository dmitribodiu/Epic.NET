//  
//  NewCargoTester.cs
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
using Challenge00.DDDSample.Cargo;
using Rhino.Mocks;
using Challenge00.DDDSample.Location;
using Challenge00.DDDSample.Voyage;
namespace DefaultImplementation.Cargo
{
	[TestFixture]
	public class NewCargoTester
	{
		[Test]
		public void Test_Ctor_01()
		{
			// arrange:
			TrackingId id = new TrackingId("CRG01");
			IRouteSpecification specification = MockRepository.GenerateStrictMock<IRouteSpecification>();
		
			// act:
			NewCargo state = new NewCargo(id, specification);
		
			// assert:
			Assert.IsTrue(state.Equals(state));
			Assert.IsFalse(state.Equals(null));
			Assert.AreSame(id, state.Identifier);
			Assert.AreSame(specification, state.RouteSpecification);
			Assert.IsNull(state.LastKnownLocation);
			Assert.IsNull(state.Itinerary);
			Assert.IsNull(state.CurrentVoyage);
			Assert.IsNull(state.EstimatedTimeOfArrival);
			Assert.AreEqual(RoutingStatus.NotRouted, state.RoutingStatus);
			Assert.AreEqual(TransportStatus.NotReceived, state.TransportStatus);
			Assert.IsFalse(state.IsUnloadedAtDestination);
			specification.VerifyAllExpectations();
		}
		
		[Test()]
		[ExpectedException(typeof(ArgumentNullException))]
		public void Test_Ctor_02 ()
		{
			// arrange:
			TrackingId number = new TrackingId("VYGTEST01");
		
			// act:
			new NewCargo(number, null);
		}
		
		[Test()]
		[ExpectedException(typeof(ArgumentNullException))]
		public void Test_Ctor_03 ()
		{
			// arrange:
			IRouteSpecification specification = MockRepository.GenerateStrictMock<IRouteSpecification>();
		
			// act:
			new NewCargo(null, specification);
		}
		
		[Test]
		public void Test_AssignToRoute_01()
		{
			// arrange:
			TrackingId id = new TrackingId("CRG01");
			IItinerary itinerary = MockRepository.GenerateStrictMock<IItinerary>();
			itinerary.Expect(i => i.Equals(null)).Return(false);
			IRouteSpecification specification = MockRepository.GenerateStrictMock<IRouteSpecification>();
			specification.Expect(s => s.IsSatisfiedBy(itinerary)).Return(true).Repeat.Once();
			NewCargo state = new NewCargo(id, specification);
		
			// act:
			CargoState newState = state.AssignToRoute(itinerary);
		
			// assert:
			Assert.IsNotNull(newState);
			Assert.IsTrue(state.CalculationDate <= newState.CalculationDate);
			Assert.AreEqual(RoutingStatus.Routed, newState.RoutingStatus);
			Assert.AreSame(itinerary, newState.Itinerary);
			Assert.AreNotSame(state, newState);
			Assert.IsFalse(state.Equals(newState));
			Assert.IsFalse(newState.Equals(state));
			itinerary.VerifyAllExpectations();
			specification.VerifyAllExpectations();
		}
		
		[Test]
		public void Test_AssignToRoute_02()
		{
			// arrange:
			TrackingId id = new TrackingId("CRG01");
			IItinerary itinerary = MockRepository.GenerateStrictMock<IItinerary>();
			itinerary.Expect(i => i.Equals(null)).Return(false);
			IRouteSpecification specification = MockRepository.GenerateStrictMock<IRouteSpecification>();
			specification.Expect(s => s.IsSatisfiedBy(itinerary)).Return(false).Repeat.Once();
			NewCargo state = new NewCargo(id, specification);
		
			// act:
			Assert.Throws<ArgumentException>(delegate { state.AssignToRoute(itinerary); } );
		
			// assert:
			itinerary.VerifyAllExpectations();
			specification.VerifyAllExpectations();
		}
		
		[Test]
		public void Test_SpecifyNewRoute_01()
		{
			// arrange:
			TrackingId id = new TrackingId("CRG01");
			IRouteSpecification specification = MockRepository.GenerateStrictMock<IRouteSpecification>();
			NewCargo state = new NewCargo(id, specification);
			IRouteSpecification specification2 = MockRepository.GenerateStrictMock<IRouteSpecification>();
			specification.Expect(s => s.Equals(specification2)).Return(false).Repeat.Any();
			specification2.Expect(s => s.Equals(specification)).Return(false).Repeat.Any();
		
			// act:
			CargoState newState = state.SpecifyNewRoute(specification2);
		
			// assert:
			Assert.IsNotNull(newState);
			Assert.AreNotSame(state, newState);
			Assert.AreEqual(RoutingStatus.NotRouted, newState.RoutingStatus);
			Assert.AreSame(specification2, newState.RouteSpecification);
			Assert.IsTrue(state.CalculationDate <= newState.CalculationDate);
			specification.VerifyAllExpectations();
			specification2.VerifyAllExpectations();
		}
		
		[Test]
		public void Test_SpecifyNewRoute_02()
		{
			// arrange:
			TrackingId id = new TrackingId("CRG01");
			IRouteSpecification specification = MockRepository.GenerateStrictMock<IRouteSpecification>();
			IRouteSpecification specification2 = MockRepository.GenerateStrictMock<IRouteSpecification>();
			specification.Expect(s => s.Equals(specification2)).Return(true).Repeat.Any();
			specification2.Expect(s => s.Equals(specification)).Return(true).Repeat.Any();
			NewCargo state = new NewCargo(id, specification);
		
			// act:
			CargoState newState = state.SpecifyNewRoute(specification2);
		
			// assert:
			Assert.IsNotNull(newState);
			Assert.AreSame(state, newState);
			specification.VerifyAllExpectations();
			specification2.VerifyAllExpectations();
		}
		
		[Test]
		public void Test_SpecifyNewRoute_03()
		{
			// arrange:
			TrackingId id = new TrackingId("CRG01");
			IRouteSpecification specification = MockRepository.GenerateStrictMock<IRouteSpecification>();
			IRouteSpecification specification2 = MockRepository.GenerateStrictMock<IRouteSpecification>();
			specification.Expect(s => s.Equals(specification2)).Return(false).Repeat.Any();
			specification2.Expect(s => s.Equals(specification)).Return(false).Repeat.Any();
			IItinerary itinerary = MockRepository.GenerateStrictMock<IItinerary>();
			itinerary.Expect(i => i.Equals(null)).Return(false);
			itinerary.Expect(i => i.FinalArrivalDate).Return(DateTime.UtcNow + TimeSpan.FromDays(1));
			specification.Expect(s => s.IsSatisfiedBy(itinerary)).Return(true).Repeat.Once();
			specification2.Expect(s => s.IsSatisfiedBy(itinerary)).Return(true).Repeat.Once();
			CargoState state = new NewCargo(id, specification);
			state = state.AssignToRoute(itinerary);
		
			// act:
			CargoState newState = state.SpecifyNewRoute(specification2);
		
			// assert:
			Assert.IsNotNull(newState);
			Assert.AreNotSame(state, newState);
			Assert.AreEqual(RoutingStatus.Routed, newState.RoutingStatus);
			Assert.AreSame(specification2, newState.RouteSpecification);
			Assert.IsTrue(state.CalculationDate <= newState.CalculationDate);
			specification.VerifyAllExpectations();
			specification2.VerifyAllExpectations();
			itinerary.VerifyAllExpectations();
		}
		
		[Test]
		public void Test_SpecifyNewRoute_04()
		{
			// arrange:
			TrackingId id = new TrackingId("CRG01");
			IRouteSpecification specification = MockRepository.GenerateStrictMock<IRouteSpecification>();
			IRouteSpecification specification2 = MockRepository.GenerateStrictMock<IRouteSpecification>();
			specification.Expect(s => s.Equals(specification2)).Return(false).Repeat.Any();
			specification2.Expect(s => s.Equals(specification)).Return(false).Repeat.Any();
			IItinerary itinerary = MockRepository.GenerateStrictMock<IItinerary>();
			itinerary.Expect(i => i.Equals(null)).Return(false);
			specification.Expect(s => s.IsSatisfiedBy(itinerary)).Return(true).Repeat.Once();
			specification2.Expect(s => s.IsSatisfiedBy(itinerary)).Return(false).Repeat.Once();
			CargoState state = new NewCargo(id, specification);
			state = state.AssignToRoute(itinerary);
		
			// act:
			CargoState newState = state.SpecifyNewRoute(specification2);
		
			// assert:
			Assert.IsNotNull(newState);
			Assert.AreNotSame(state, newState);
			Assert.IsFalse(state.Equals(newState));
			Assert.IsFalse(newState.Equals(state));
			Assert.AreEqual(RoutingStatus.Misrouted, newState.RoutingStatus);
			Assert.AreSame(specification2, newState.RouteSpecification);
			Assert.IsTrue(state.CalculationDate <= newState.CalculationDate);
			specification.VerifyAllExpectations();
			specification2.VerifyAllExpectations();
			itinerary.VerifyAllExpectations();
		}
		
		[Test]
		public void Test_ClearCustoms_01()
		{
			// arrange:
			TrackingId id = new TrackingId("CRG01");
			IRouteSpecification specification = MockRepository.GenerateStrictMock<IRouteSpecification>();
			ILocation location = MockRepository.GenerateMock<ILocation>();
			CargoState state = new NewCargo(id, specification);
		
			// assert:
			Assert.Throws<InvalidOperationException>(delegate { state.ClearCustoms(location, DateTime.Now); });
		}
		
		[Test]
		public void Test_Claim_01()
		{
			// arrange:
			TrackingId id = new TrackingId("CRG01");
			IRouteSpecification specification = MockRepository.GenerateStrictMock<IRouteSpecification>();
			ILocation location = MockRepository.GenerateMock<ILocation>();
			CargoState state = new NewCargo(id, specification);
		
			// assert:
			Assert.Throws<InvalidOperationException>(delegate { state.Claim(location, DateTime.Now); });
		}
		
		[Test]
		public void Test_Recieve_01()
		{
			// arrange:
			TrackingId id = new TrackingId("CRG01");
			UnLocode code = new UnLocode("START");
			IRouteSpecification specification = MockRepository.GenerateStrictMock<IRouteSpecification>();
			ILocation location = MockRepository.GenerateMock<ILocation>();
			location.Expect(l => l.UnLocode).Return(code).Repeat.AtLeastOnce();
			IItinerary itinerary = MockRepository.GenerateStrictMock<IItinerary>();
			itinerary.Expect(i => i.InitialDepartureLocation).Return(code).Repeat.AtLeastOnce();
			itinerary.Expect(i => i.Equals(null)).Return(false).Repeat.Any();
			itinerary.Expect(i => i.FinalArrivalDate).Return(DateTime.UtcNow + TimeSpan.FromDays(1)).Repeat.Any();
			specification.Expect(s => s.IsSatisfiedBy(itinerary)).Return(true).Repeat.Once();
			CargoState state = new NewCargo(id, specification);
			state = state.AssignToRoute(itinerary);
		
			// act:
			CargoState newState = state.Recieve(location, DateTime.UtcNow);
		
			// assert:
			Assert.IsNotNull(newState);
			Assert.IsInstanceOf<InPortCargo>(newState);
			Assert.AreSame(code, newState.LastKnownLocation);
			Assert.IsTrue(newState.EstimatedTimeOfArrival.HasValue);
			Assert.IsTrue(TransportStatus.InPort == newState.TransportStatus);
			location.VerifyAllExpectations();
			itinerary.VerifyAllExpectations();
			specification.VerifyAllExpectations();
		}
		
		[Test]
		public void Test_Recieve_02()
		{
			// arrange:
			TrackingId id = new TrackingId("CRG01");
			UnLocode start = new UnLocode("START");
			UnLocode other = new UnLocode("OTHER");
			IRouteSpecification specification = MockRepository.GenerateStrictMock<IRouteSpecification>();
			ILocation location = MockRepository.GenerateMock<ILocation>();
			location.Expect(l => l.UnLocode).Return(other).Repeat.AtLeastOnce();
			IItinerary itinerary = MockRepository.GenerateStrictMock<IItinerary>();
			itinerary.Expect(i => i.InitialDepartureLocation).Return(start).Repeat.AtLeastOnce();
			itinerary.Expect(i => i.Equals(null)).Return(false).Repeat.Any();
			itinerary.Expect(i => i.FinalArrivalDate).Return(DateTime.UtcNow + TimeSpan.FromDays(1)).Repeat.Any();
			specification.Expect(s => s.IsSatisfiedBy(itinerary)).Return(true).Repeat.Once();
			CargoState state = new NewCargo(id, specification);
			state = state.AssignToRoute(itinerary);
		
			// assert:
			Assert.Throws<ArgumentException>(delegate {state.Recieve(location, DateTime.UtcNow);});
			location.VerifyAllExpectations();
			itinerary.VerifyAllExpectations();
			specification.VerifyAllExpectations();
		}
		
		[Test]
		public void Test_Recieve_03()
		{
			// arrange:
			TrackingId id = new TrackingId("CRG01");
			IRouteSpecification specification = MockRepository.GenerateStrictMock<IRouteSpecification>();
			ILocation location = MockRepository.GenerateMock<ILocation>();
			CargoState state = new NewCargo(id, specification);
		
			// assert:
			Assert.Throws<InvalidOperationException>(delegate {state.Recieve(location, DateTime.UtcNow);});
			location.VerifyAllExpectations();
			specification.VerifyAllExpectations();
		}
		
		[Test]
		public void Test_LoadOn_01()
		{
			// arrange:
			TrackingId id = new TrackingId("CRG01");
			IRouteSpecification specification = MockRepository.GenerateStrictMock<IRouteSpecification>();
			IVoyage voyage = MockRepository.GenerateMock<IVoyage>();
			CargoState state = new NewCargo(id, specification);
		
			// assert:
			Assert.Throws<InvalidOperationException>(delegate { state.LoadOn(voyage, DateTime.Now); });
		}
		
		[Test]
		public void Test_Unload_01()
		{
			// arrange:
			TrackingId id = new TrackingId("CRG01");
			IRouteSpecification specification = MockRepository.GenerateStrictMock<IRouteSpecification>();
			IVoyage voyage = MockRepository.GenerateMock<IVoyage>();
			CargoState state = new NewCargo(id, specification);
		
			// assert:
			Assert.Throws<InvalidOperationException>(delegate { state.Unload(voyage, DateTime.Now); });
		}
	}
}

