﻿using System;
using System.Collections.Generic;
using WebApplication1.EfStuff.Model.Airport;
using WebApplication1.Models.Airport;

namespace WebApplication1.Presentation.Airport
{
	public interface IAirportPresentation
	{
		void AdmitPassengers();
		void BookTicket(long id);
		void ConvertFlights(List<Flight> flights);
		void DepartPassengers();
		bool FlightIsValid(long id);
		List<Flight> GetAvailableFlights();
		List<IncomingFlightInfoViewModel> GetIndexViewModel();
		bool IsValidAddmissionTime(DateTime flightDate);
		bool IsValidDepartureTime(DateTime flightDate);
	}
}