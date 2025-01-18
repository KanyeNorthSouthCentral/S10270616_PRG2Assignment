//==========================================================
// Student Number : S10270619
// Student Name : Dhushyanth
// Partner Name : Hafiz
//==========================================================
﻿using System.Collections.Generic;
public abstract class Flight
{
    public string FlightNumber { get; set; }
    public string Origin { get; set; }
    public string Destination { get; set; }
    public DateTime ExpectedTime { get; set; }
    public string Status { get; set; }

    public Flight(string flightNumber, string origin, string destination, DateTime expectedTime, string status = "Scheduled")
    {
        FlightNumber = flightNumber;
        Origin = origin;
        Destination = destination;
        ExpectedTime = expectedTime;
        Status = status;
    }

    public abstract double CalculateFees();

    public override string ToString()
    {
        return $"{FlightNumber}: {Origin} -> {Destination}, Time: {ExpectedTime}, Status: {Status}";
    }
}
