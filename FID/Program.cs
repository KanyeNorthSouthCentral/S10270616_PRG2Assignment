﻿using FID;
using System;
using System.Collections.Generic;
using System.IO;

class Program
{
    static Dictionary<string, Airline> airlines = new Dictionary<string, Airline>();
    static Dictionary<string, BoardingGate> boardingGates = new Dictionary<string, BoardingGate>();
    static Dictionary<string, Flight> flights = new Dictionary<string, Flight>();

    static void Main(string[] args)
    {
        LoadAirlines();
        LoadBoardingGates();
        LoadFlights();

        while (true)
        {
            ShowMainMenu();
            int option = GetUserInput();

            switch (option)
            {
                case 1:
                    //Hafiz Feature 3 //
                    ListAllFlights();
                    break;
                case 2:
                    //Dhush Feature 4 //
                    ListBoardingGates();
                    break;
                case 3:
                    //Hafiz Feature 5 //
                    AssignBoardingGateToFlight();
                    break;
                case 4:
                    //Hafiz Feature 6 //
                    CreateFlight();
                    break;
                case 5:
                    //Dhush Feature 7 //
                    DisplayFullFlightDetailsFromAirline();
                    break;
                case 6:
                    //Dhush Feature 8 //
                    ModifyFlightDetails();
                    break;
                case 7:
                    //Hafiz Feature 9 //
                    DisplayScheduledFlights();
                    break;
                case 8:
                    //Dhush Advanced Feature//
                    ProcessUnassignedFlights();
                    break;
                case 9:
                    //Hafiz Advanced Feature//
                    DisplayTotalFeePerAirline();
                    break;
                case 10:
                    //Dhush Additional Feature//
                    SearchAndFilterFlights();
                    break;
                case 0:
                    Console.WriteLine("Exiting program. Goodbye!");
                    return;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }
    }

    //COllab//
    static int GetUserInput()
    {
        if (int.TryParse(Console.ReadLine(), out int option))
        {
            return option;
        }
        return -1;
    }

    static void ShowMainMenu()
    {
        Console.WriteLine("\n=============================================");
        Console.WriteLine("Welcome to Changi Airport Terminal 5");
        Console.WriteLine("=============================================");
        Console.WriteLine("1. List All Flights");
        Console.WriteLine("2. List Boarding Gates");
        Console.WriteLine("3. Assign a Boarding Gate to a Flight");
        Console.WriteLine("4. Create Flight");
        Console.WriteLine("5. Display Airline Flights");
        Console.WriteLine("6. Modify Flight Details");
        Console.WriteLine("7. Display Flight Schedule");
        Console.WriteLine("8. Process Unassigned Flights");
        Console.WriteLine("9. Display Total Fee Per Airline");
        Console.WriteLine("10.Search And Filter Flights");
        Console.WriteLine("0. Exit");
        Console.Write("\nPlease select your option: ");
    }

    //Dhush Feature 1//
    static void LoadAirlines()
    {
        Console.WriteLine("Loading Airlines...");
        // Counter to track how many airlines loaded
        int count = 0;
        // Reading of the csv
        using (StreamReader sr = new StreamReader("airlines.csv"))
        {
            sr.ReadLine(); // Skip header
            // Variable to hold the line of the file as it is read
            string line;
            // Loop to read file till then end of the line 
            while ((line = sr.ReadLine()) != null)
            {
                var data = line.Split(',');
                // Extracting the airline name and code
                string name = data[0].Trim();  // Trim to remove any extra spaces
                string code = data[1].Trim();  // Trim to remove any extra spaces
                // Add the name and code into the dictionary
                airlines[code] = new Airline(name, code);
                // increase the counter 
                count++;
            }
        }

        //  Total number of airlines that were loaded from the file
        Console.WriteLine($"{count} Airlines Loaded!");
    }

    //Hafiz Feature 2 //
    static void LoadFlights()
    {
        Console.WriteLine("Loading Flights...");
        int count = 0;
        try
        {
            using (StreamReader sr = new StreamReader("flights.csv"))
            {
                sr.ReadLine(); string line;
                while ((line = sr.ReadLine()) != null)
                {
                    var data = line.Split(',');
                    if (data.Length >= 5 && DateTime.TryParse(data[3].Trim(), out DateTime expectedTime))
                    {
                        string flightNumber = data[0].Trim();
                        string origin = data[1].Trim();
                        string destination = data[2].Trim();
                        string requestType = data[4].Trim(); // Special Request Code 

                        // Default status for flights 
                        string status = "Scheduled";

                        Flight flight = requestType switch
                        {
                            "CFFT" => new CFFTFlight(flightNumber, origin, destination, expectedTime, status, 150),
                            "DDJB" => new DDJBFlight(flightNumber, origin, destination, expectedTime, status, 300),
                            "LWTT" => new LWTTFlight(flightNumber, origin, destination, expectedTime, status, 500),
                            _ => new NORMFlight(flightNumber, origin, destination, expectedTime, status)
                        };

                        flights[flightNumber] = flight;

                        string airlineCode = flightNumber.Substring(0, 2);
                        if (airlines.ContainsKey(airlineCode))
                        {
                            airlines[airlineCode].AddFlight(flight);
                        }

                        count++;
                    }
                    else
                    {
                        Console.WriteLine($"Invalid row skipped: {line}");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading flights: {ex.Message}");
        }
        Console.WriteLine($"{count} Flights Loaded!");
    }

    //Hafiz Feature 3 //
    static void ListAllFlights()
    {
        Console.WriteLine("=============================================");
        Console.WriteLine("List of Flights for Changi Airport Terminal 5");
        Console.WriteLine("=============================================");
        Console.WriteLine($"{"Flight Number",-15} {"Airline Name",-25} {"Origin",-20} {"Destination",-20} {"Expected Departure/Arrival Time",-30}");

        List<Flight> flightList = new List<Flight>(flights.Values);
        flightList.Sort();

        foreach (var flight in flights.Values)
        {
            string airlineCode = flight.FlightNumber.Substring(0, 2).Trim();
            string airlineName = "Unknown Airline";

            if (airlines.ContainsKey(airlineCode))
            {
                airlineName = airlines[airlineCode].Name;
            }

            Console.WriteLine($"{flight.FlightNumber,-15} {airlineName,-25} {flight.Origin,-20} {flight.Destination,-20} {flight.ExpectedTime.ToString("dd/M/yyyy HH:mm"),-30}");
        }
    }
    
    //Dhush Feature 4//
    static void LoadBoardingGates()
    {
        Console.WriteLine("Loading Boarding Gates...");
        // Counter to track how many airlines loaded
        int count = 0;
        // Reading of the csv
        using (StreamReader sr = new StreamReader("boardinggates.csv"))
        {
            sr.ReadLine(); // Skip header
            // Variable to hold the line of the file as it is read
            string line;
            // Loop to read file till then end of the line 
            while ((line = sr.ReadLine()) != null)
            {
                var data = line.Split(',');
                {
                    // Extracting the Boarding Gate name and special request
                    string gateName = data[0].Trim();
                    bool supportsDDJB = bool.Parse(data[1].Trim()); // Parse as boolean (true/false)
                    bool supportsCFFT = bool.Parse(data[2].Trim()); // Parse as boolean (true/false)
                    bool supportsLWTT = bool.Parse(data[3].Trim()); // Parse as boolean (true/false)
                    // Add the Boarding Gate name and special request into the dictionary
                    boardingGates[gateName] = new BoardingGate(gateName, supportsCFFT, supportsDDJB, supportsLWTT);
                    count++;
                }
            }
        }
        Console.WriteLine($"{count} Boarding Gates Loaded!");
    }

    //Dhush Feature 4 //
    static void ListBoardingGates()
    {
        Console.WriteLine("=============================================");
        Console.WriteLine("List of Boarding Gates for Changi Airport Terminal 5");
        Console.WriteLine("=============================================");
        Console.WriteLine($"{"Gate Name",-19} {"DDJB",-24} {"CFFT",-19} {"LWTT",-20} ");


        foreach (var gate in boardingGates.Values)
        {
            Console.WriteLine($"{gate.GateName,-20}{gate.SupportsDDJB,-25}{gate.SupportsCFFT,-20}{gate.SupportsLWTT,-20}");
        }
    }

    //Hafiz Feature 5 //
    static void AssignBoardingGateToFlight()
    {
        Console.WriteLine("=============================================");
        Console.WriteLine("Assign a Boarding Gate to a Flight");
        Console.WriteLine("=============================================");

        // Prompt the user for the Flight Number
        Console.Write("Enter Flight Number: ");
        string flightNumber = Console.ReadLine().Trim().ToUpper();

        // Check if the flight exists
        if (!flights.ContainsKey(flightNumber))
        {
            Console.WriteLine("Invalid Flight Number. Please try again.");
            return;
        }

        Flight flight = flights[flightNumber];

        // Prompt the user for the Boarding Gate Name
        Console.Write("Enter Boarding Gate Name: ");
        string gateName = Console.ReadLine().Trim().ToUpper();

        // Check if the boarding gate exists
        if (!boardingGates.ContainsKey(gateName))
        {
            Console.WriteLine("Invalid Boarding Gate Name. Please try again.");
            return;
        }

        BoardingGate gate = boardingGates[gateName];

        // Check if the boarding gate is already assigned to another flight
        if (gate.Flight != null)
        {
            Console.WriteLine($"Boarding Gate {gateName} is already assigned to another flight. Please choose a different gate.");
            return;
        }

        // Display the flight and gate details
        Console.WriteLine($"Flight Number: {flight.FlightNumber}");
        Console.WriteLine($"Origin: {flight.Origin}");
        Console.WriteLine($"Destination: {flight.Destination}");
        Console.WriteLine($"Expected Time: {flight.ExpectedTime:dd/M/yyyy h:mm:ss tt}");
        Console.WriteLine($"Special Request Code: None");
        Console.WriteLine($"Boarding Gate Name: {gate.GateName}");
        Console.WriteLine($"Supports DDJB: {gate.SupportsDDJB}");
        Console.WriteLine($"Supports CFFT: {gate.SupportsCFFT}");
        Console.WriteLine($"Supports LWTT: {gate.SupportsLWTT}");

        // Prompt the user if they would like to update the status of the flight
        Console.Write("Would you like to update the status of the flight? (Y/N): ");
        string updateStatus = Console.ReadLine().Trim().ToUpper();

        if (updateStatus == "Y")
        {
            Console.WriteLine("1. Delayed");
            Console.WriteLine("2. Boarding");
            Console.WriteLine("3. On Time");
            Console.Write("Please select the new status of the flight: ");
            int statusOption = int.Parse(Console.ReadLine().Trim());

            if (statusOption == 1)
            {
                flight.Status = "Delayed";
            }
            else if (statusOption == 2)
            {
                flight.Status = "Boarding";
            }
            else if (statusOption == 3)
            {
                flight.Status = "On Time";
            }
            else
            {
                Console.WriteLine("Invalid option. Status not updated.");
            }
        }
        
        // Assign the boarding gate to the flight
        gate.Flight = flight;
        // Display a success message
        Console.WriteLine($"Flight {flight.FlightNumber} has been assigned to Boarding Gate {gate.GateName}!");
    }

    //Hafiz Feature 6 //
    static void CreateFlight()
{
        Console.Write("Enter Flight Number: ");
        string flightNumber = Console.ReadLine().Trim().ToUpper();

        Console.Write("Enter Origin: ");
        string origin = Console.ReadLine().Trim();

        Console.Write("Enter Destination: ");
        string destination = Console.ReadLine().Trim();

        //DateTime expectedTime;
        DateTime expectedTime = DateTime.MinValue;
        bool validDate = false;

        while (!validDate)
        {
            Console.Write("Enter Expected Departure/Arrival Time (dd/MM/yyyy HH:mm): ");
            string input = Console.ReadLine().Trim();

            try
            {
                expectedTime = DateTime.Parse(input);
                validDate = true;
            }
            catch
            {
                Console.WriteLine("Invalid format. Please enter again in the correct format.");
            }
        }

        Console.Write("Enter Special Request Code (CFFT/DDJB/LWTT/None): ");
        string requestType = Console.ReadLine().Trim().ToUpper();
        if (requestType != "CFFT" && requestType != "DDJB" && requestType != "LWTT" && requestType != "NONE")
        {
            requestType = "None";
        }

        string status = "Scheduled";
        Flight newFlight = requestType switch
        {
            "CFFT" => new CFFTFlight(flightNumber, origin, destination, expectedTime, status, 150),
            "DDJB" => new DDJBFlight(flightNumber, origin, destination, expectedTime, status, 300),
            "LWTT" => new LWTTFlight(flightNumber, origin, destination, expectedTime, status, 500),
            _ => new NORMFlight(flightNumber, origin, destination, expectedTime, status)
        };

        flights[flightNumber] = newFlight;

        using (StreamWriter sw = new StreamWriter("flights.csv", true))
        {
            sw.WriteLine($"{flightNumber},{origin},{destination},{expectedTime},{requestType}");
        }

        Console.WriteLine($"Flight {flightNumber} has been added!");

        Console.Write("Would you like to add another flight? (Y/N): ");
        string response = Console.ReadLine().Trim().ToUpper();
        if (response != "Y")
        {
            Console.WriteLine("Flight(s) have been successfully added!");
        }
    }

    //Dhush Feature 7 //
    static void DisplayFullFlightDetailsFromAirline()
{
    Console.WriteLine("=============================================");
    Console.WriteLine("List of Airlines for Changi Airport Terminal 5");
    Console.WriteLine("=============================================");
    Console.WriteLine($"{"Airline Code",-25} {"Airline Name"}");

    foreach (var airlineEntry in airlines.Values)
    {
        Console.WriteLine($"{airlineEntry.Code,-25} {airlineEntry.Name}");
    }

    Console.Write("Enter Airline Code: ");
    string airlineCode = Console.ReadLine().Trim().ToUpper();

    if (!airlines.ContainsKey(airlineCode))
    {
        Console.WriteLine("Invalid Airline Code. Please enter a valid code from the list.");
        return;
    }

    var selectedAirline = airlines[airlineCode];

    Console.WriteLine("=============================================");
    Console.WriteLine($"List of Flights for {selectedAirline.Name}");
    Console.WriteLine("=============================================");
    Console.WriteLine($"{"Flight Number",-15} {"Airline Name",-25} {"Origin",-25} {"Destination",-25} {"Expected Departure/Arrival Time"}");

    foreach (var flight in selectedAirline.Flights.Values)
    {
        Console.WriteLine($"{flight.FlightNumber,-15} {selectedAirline.Name,-25} {flight.Origin,-25} {flight.Destination,-25} {flight.ExpectedTime:dd/M/yyyy h:mm:ss tt}");
    }
}

    //Dhush Feature 8 //
    static void ModifyFlightDetails()
{
    Console.WriteLine("\n=============================================");
    Console.WriteLine("List of Airlines for Changi Airport Terminal 5");
    Console.WriteLine("=============================================");

    Console.WriteLine("\nAirline Code\tAirline Name");

    foreach (var airlineEntry in airlines.Values)
    {
        Console.WriteLine($"{airlineEntry.Code,-15}{airlineEntry.Name}");
    }

    Console.Write("\nEnter Airline Code: ");
    string airlineCode = Console.ReadLine()?.Trim().ToUpper();

    if (string.IsNullOrEmpty(airlineCode) || !airlines.TryGetValue(airlineCode, out var airline))
    {
        Console.WriteLine("Invalid Airline Code. Please try again.");
        return;
    }

    Console.WriteLine($"\nList of Flights for {airline.Name}");
    Console.WriteLine("\nFlight Number   Airline Name           Origin                 Destination            Expected Departure/Arrival Time");

    if (airline.Flights.Count == 0)
    {
        Console.WriteLine("No flights available for this airline.");
        return;
    }

    foreach (var flight in airline.Flights.Values)
    {
        Console.WriteLine($"{flight.FlightNumber,-15}{airline.Name,-22}{flight.Origin,-25}{flight.Destination,-25}{flight.ExpectedTime:dd/M/yyyy h:mm:ss tt}");
    }

    Console.Write("\nChoose an existing Flight to modify or delete: ");
    string flightNumber = Console.ReadLine()?.Trim().ToUpper();

    if (string.IsNullOrEmpty(flightNumber) || !airline.Flights.TryGetValue(flightNumber, out var selectedFlight))
    {
        Console.WriteLine("Invalid Flight Number. Please try again.");
        return;
    }

    Console.WriteLine("\n1. Modify Flight");
    Console.WriteLine("2. Delete Flight");
    Console.Write("Choose an option: ");

    if (!int.TryParse(Console.ReadLine()?.Trim(), out int choice) || (choice != 1 && choice != 2))
    {
        Console.WriteLine("Invalid choice. Please enter 1 or 2.");
        return;
    }

    if (choice == 1)
    {
        Console.WriteLine("\n1. Modify Basic Information");
        Console.WriteLine("2. Modify Status");
        Console.WriteLine("3. Modify Special Request Code");
        Console.WriteLine("4. Modify Boarding Gate");
        Console.Write("Choose an option: ");

        if (!int.TryParse(Console.ReadLine()?.Trim(), out int detailChoice) || detailChoice < 1 || detailChoice > 4)
        {
            Console.WriteLine("Invalid choice.");
            return;
        }

        switch (detailChoice)
        {
            case 1:
                Console.Write("Enter new Origin: ");
                string newOrigin = Console.ReadLine()?.Trim();
                Console.Write("Enter new Destination: ");
                string newDestination = Console.ReadLine()?.Trim();
                Console.Write("Enter new Expected Departure/Arrival Time (dd/MM/yyyy HH:mm): ");
                if (DateTime.TryParseExact(Console.ReadLine()?.Trim(), "d/M/yyyy H:mm", null, System.Globalization.DateTimeStyles.None, out var newTime))
                {
                    selectedFlight.Origin = newOrigin;
                    selectedFlight.Destination = newDestination;
                    selectedFlight.ExpectedTime = newTime;
                    Console.WriteLine("\nFlight updated!");
                }
                else
                {
                    Console.WriteLine("Invalid date format. Expected Time not updated.");
                    return;
                }
                break;
            case 2:
                Console.Write("Enter new Status (Delayed, Boarding, On Time): ");
                string newStatus = Console.ReadLine()?.Trim();
                selectedFlight.Status = newStatus;
                Console.WriteLine("\nStatus updated successfully.");
                break;
            case 3:
                Console.Write("Enter new Special Request Code (CFFT, DDJB, LWTT, None): ");
                string specialRequest = Console.ReadLine()?.Trim();
                Console.WriteLine("\nSpecial Request Code updated successfully.");
                break;
            case 4:
                Console.Write("Enter new Boarding Gate: ");
                string newGate = Console.ReadLine()?.Trim();
                Console.WriteLine("\nBoarding Gate updated successfully.");
                break;
        }

        Console.WriteLine("\nFlight Number: " + selectedFlight.FlightNumber);
        Console.WriteLine("Airline Name: " + airline.Name);
        Console.WriteLine("Origin: " + selectedFlight.Origin);
        Console.WriteLine("Destination: " + selectedFlight.Destination);
        Console.WriteLine($"Expected Departure/Arrival Time: {selectedFlight.ExpectedTime:dd/M/yyyy h:mm:ss tt}");
        Console.WriteLine("Status: " + selectedFlight.Status);
        Console.WriteLine("Special Request Code: CFFT");
        Console.WriteLine("Boarding Gate: Unassigned");
    }
    else if (choice == 2)
    {
        Console.Write("\nAre you sure you want to delete this flight? [Y/N]: ");
        if (Console.ReadLine()?.Trim().ToUpper() == "Y")
        {
            airline.Flights.Remove(flightNumber);
            flights.Remove(flightNumber);
            Console.WriteLine("\nFlight deleted successfully.");
        }
        else
        {
            Console.WriteLine("\nFlight deletion cancelled.");
        }
    }
}

    //Hafiz Feature 9 //
    static void DisplayScheduledFlights()
{

        Console.WriteLine("=============================================");
        Console.WriteLine("Flight Schedule for Changi Airport Terminal 5");
        Console.WriteLine("=============================================");
        Console.WriteLine($"{"Flight Number",-15} {"Airline Name",-25} {"Origin",-25} {"Destination",-25} {"Expected Departure/Arrival Time",-30}   {"Status",-18} {"Boarding Gate",-15}");

        List<Flight> flightList = new List<Flight>(flights.Values);
        flightList.Sort();

        foreach (var flight in flightList)
        {
            string airlineCode = flight.FlightNumber.Substring(0, 2).Trim();
            string airlineName = airlines.ContainsKey(airlineCode) ? airlines[airlineCode].Name : "Unknown Airline";
            string boardingGate = "Unassigned";

            foreach (var gate in boardingGates.Values)
            {
                if (gate.Flight != null && gate.Flight.FlightNumber == flight.FlightNumber)
                {
                    boardingGate = gate.GateName;
                    break;
                }
            }
            string expectedTimeFormatted = flight.ExpectedTime.ToString("dd/M/yyyy h:mm:ss tt").PadRight(30);
            Console.WriteLine($"{flight.FlightNumber,-15} {airlineName,-25} {flight.Origin,-25} {flight.Destination,-25} {expectedTimeFormatted}    {flight.Status,-18} {boardingGate,-16}");
        }
    }

    //Dhush Advanced Feature (a) //
    static void ProcessUnassignedFlights()
    {
        Console.WriteLine("=============================================");
        Console.WriteLine("Processing Unassigned Flights");
        Console.WriteLine("=============================================");

        Queue<Flight> unassignedFlights = new Queue<Flight>();
        int unassignedFlightCount = 0;
        int unassignedGateCount = 0;

        // Check for unassigned flights
        foreach (var flight in flights.Values)
        {
            bool isAssigned = false;
            foreach (var gate in boardingGates.Values)
            {
                if (gate.Flight != null && gate.Flight.FlightNumber == flight.FlightNumber)
                {
                    isAssigned = true;
                    break;
                }
            }
            if (!isAssigned)
            {
                unassignedFlights.Enqueue(flight);
                unassignedFlightCount++;
            }
        }

        // Check for unassigned boarding gates
        foreach (var gate in boardingGates.Values)
        {
            if (gate.Flight == null)
            {
                unassignedGateCount++;
            }
        }

        Console.WriteLine($"Total Unassigned Flights: {unassignedFlightCount}");
        Console.WriteLine($"Total Unassigned Boarding Gates: {unassignedGateCount}");

        // Process unassigned flights
        while (unassignedFlights.Count > 0)
        {
            Flight flight = unassignedFlights.Dequeue();
            bool assigned = false;

            foreach (var gate in boardingGates.Values)
            {
                if (gate.Flight == null)
                {
                    if (flight is CFFTFlight && gate.SupportsCFFT)
                    {
                        gate.Flight = flight;
                        assigned = true;
                        break;
                    }
                    else if (flight is DDJBFlight && gate.SupportsDDJB)
                    {
                        gate.Flight = flight;
                        assigned = true;
                        break;
                    }
                    else if (flight is LWTTFlight && gate.SupportsLWTT)
                    {
                        gate.Flight = flight;
                        assigned = true;
                        break;
                    }
                    else if (flight is NORMFlight && !gate.SupportsCFFT && !gate.SupportsDDJB && !gate.SupportsLWTT)
                    {
                        gate.Flight = flight;
                        assigned = true;
                        break;
                    }
                }
            }

            if (assigned)
            {
                Console.WriteLine($"Flight {flight.FlightNumber} assigned to a boarding gate.");
            }
            else
            {
                Console.WriteLine($"No available gate for Flight {flight.FlightNumber}.");
            }
        }

        Console.WriteLine("Processing complete.");
    }
    
    //Hafiz Advanced Feature (b) //
    static void DisplayTotalFeePerAirline()
    {

    }

    //DHUSH Aditional Feature//
    static void SearchAndFilterFlights()
    {
        Console.WriteLine("=============================================");
        Console.WriteLine("Search and Filter Flights");
        Console.WriteLine("=============================================");

        // Prompt the user for search criteria
        Console.WriteLine("Search and filter flights by:");
        Console.WriteLine("1. Origin");
        Console.WriteLine("2. Destination");
        Console.WriteLine("3. Airline");
        Console.WriteLine("4. Status");
        Console.WriteLine("5. Boarding Gate");
        Console.Write("Enter your choice (1-5): ");
        int choice = GetUserInput();

        string filterValue = "";
        switch (choice)
        {
            case 1:
                Console.Write("Enter Origin (e.g., SIN): ");
                filterValue = Console.ReadLine().Trim().ToUpper();
                break;
            case 2:
                Console.Write("Enter Destination (e.g., SIN): ");
                filterValue = Console.ReadLine().Trim().ToUpper();
                break;
            case 3:
                Console.Write("Enter Airline Code (e.g., SQ): ");
                filterValue = Console.ReadLine().Trim().ToUpper();
                break;
            case 4:
                Console.Write("Enter Status (e.g., Scheduled, Boarding, Delayed): ");
                filterValue = Console.ReadLine().Trim();
                break;
            case 5:
                Console.Write("Enter Boarding Gate (e.g., A1): ");
                filterValue = Console.ReadLine().Trim().ToUpper();
                break;
            default:
                Console.WriteLine("Invalid choice. Returning to main menu.");
                return;
        }

        // Filter flights based on the selected criteria
        var filteredFlights = flights.Values.Where(flight =>
        {
            switch (choice)
            {
                case 1:
                    return flight.Origin.Equals(filterValue, StringComparison.OrdinalIgnoreCase);
                case 2:
                    return flight.Destination.Equals(filterValue, StringComparison.OrdinalIgnoreCase);
                case 3:
                    string airlineCode = flight.FlightNumber.Substring(0, 2);
                    return airlineCode.Equals(filterValue, StringComparison.OrdinalIgnoreCase);
                case 4:
                    return flight.Status.Equals(filterValue, StringComparison.OrdinalIgnoreCase);
                case 5:
                    foreach (var gate in boardingGates.Values)
                    {
                        if (gate.Flight != null && gate.Flight.FlightNumber == flight.FlightNumber && gate.GateName.Equals(filterValue, StringComparison.OrdinalIgnoreCase))
                        {
                            return true;
                        }
                    }
                    return false;
                default:
                    return false;
            }
        }).ToList();

        // Display the filtered flights
        if (filteredFlights.Any())
        {
            Console.WriteLine("=============================================");
            Console.WriteLine("Filtered Flights");
            Console.WriteLine("=============================================");
            Console.WriteLine($"{"Flight Number",-15} {"Airline Name",-25} {"Origin",-20} {"Destination",-20} {"Expected Time",-30} {"Status",-15} {"Boarding Gate",-15}");

            foreach (var flight in filteredFlights)
            {
                string airlineCode = flight.FlightNumber.Substring(0, 2);
                string airlineName = airlines.ContainsKey(airlineCode) ? airlines[airlineCode].Name : "Unknown Airline";
                string boardingGate = "Unassigned";

                foreach (var gate in boardingGates.Values)
                {
                    if (gate.Flight != null && gate.Flight.FlightNumber == flight.FlightNumber)
                    {
                        boardingGate = gate.GateName;
                        break;
                    }
                }

                Console.WriteLine($"{flight.FlightNumber,-15} {airlineName,-25} {flight.Origin,-20} {flight.Destination,-20} {flight.ExpectedTime.ToString("dd/M/yyyy HH:mm"),-30} {flight.Status,-15} {boardingGate,-15}");
            }
        }
        else
        {
            Console.WriteLine("No flights found matching the specified criteria.");
        }
    }

}



