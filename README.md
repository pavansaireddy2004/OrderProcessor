OrderProcessor – BiztelAI Internship Assignment

This repository contains two main components required for the BiztelAI Software Engineering Internship assignment:
a console-based Order Generator
a background Order Processor Service that runs continuously and processes incoming order files.Both components work together to simulate a real-time backend order ingestion pipeline.

1) OrderGenerator (Console Tool)

The OrderGenerator is a simple console application that produces JSON order files. These files are placed inside the IncomingOrders folder of the OrderProcessorService.
The generator can create valid order.

To run the generator, navigate into the OrderGenerator folder and use:

dotnet run -- <count> <mode>

For example: dotnet run -- 5 valid

This command generates five valid order files. Modes available are:

valid – correctly structured orders

invalid – missing fields or logic failures

corrupt – unreadable JSON

These files are automatically picked up by the backend service.

2) OrderProcessorService (Worker Background Service) :

This is a .NET 6 Worker Service that continuously watches the IncomingOrders folder.
Whenever a new .json file appears, the service immediately attempts to read and process it.

The processor performs the following tasks:

Reads and deserializes the JSON content

Validates required fields (such as CustomerName and TotalAmount)

Applies business logic (e.g., marking orders above a threshold as high-value)

Saves valid orders into a SQLite database

Saves invalid or corrupted orders into a separate table with the reason for failure

Prevents duplicate processing

Handles file locking, bursts of many files, and malformed data gracefully

To start the service, navigate into the OrderProcessorService folder and run:  dotnet run
When running, the console will display log messages showing file detection, processing, and database saving activity. The service continues running until manually stopped with Ctrl + C.

3) Database (SQLite)

A SQLite database named orders.db is automatically created if it does not exist.
Two tables are used:

ValidOrders – contains properly processed orders along with a flag indicating if the order is high-value

InvalidOrders – contains raw JSON and the reason for validation failure

This setup allows easy testing, debugging, and future extension of the system.

4) Project Structure Overview

The repository contains two sibling folders:

OrderGenerator – console application used to generate test data

OrderProcessorService – worker service that processes the generated files

Both folders contain their respective .cs files, configuration files, and local database (for the processor).

5) How Everything Works Together

You start the OrderProcessorService — it begins monitoring the folder.

You run the OrderGenerator — it creates JSON files in the monitored folder.

The worker service instantly detects the files, processes them, validates them, and stores the results into SQLite.

Logs appear in the console showing exactly what happened to each file.

This simulates the behavior of a backend service that receives and processes orders from an external system in real time.



 


