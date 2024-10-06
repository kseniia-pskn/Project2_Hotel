using System;
using System.Threading;

public class Hotel
{
    public static event PriceCutEvent PriceCut; // Event to notify travel agents of a price drop
    private double currentRoomPrice = 100; // Initial room price
    private MultiCellBuffer buffer;
    private int priceCutCounter = 0; // Limit the number of price cuts to 10

    public Hotel(MultiCellBuffer buffer)
    {
        this.buffer = buffer;
    }

    // Method to simulate price updates and notify agents on price drop
    public void Start()
    {
        Random rnd = new Random();
        while (priceCutCounter < 10) // Stop after 10 price cuts
        {
            Thread.Sleep(rnd.Next(1000, 3000)); // Simulate time taken for price updates
            double newPrice = pricingModel();

            if (newPrice < currentRoomPrice) // Emit event if price drops
            {
                priceCutCounter++;
                PriceCut?.Invoke(newPrice, "Hotel1");
                Console.WriteLine($"Price cut! New price is {newPrice}");
            }

            currentRoomPrice = newPrice;
            ProcessOrders();
        }

        Console.WriteLine("Hotel has stopped price cuts after 10 reductions.");
    }

    // Pricing model to generate new prices
    public double pricingModel()
    {
        Random rnd = new Random();
        return rnd.Next(80, 160); // Generate a price between 80 and 160
    }

    // Process orders from the buffer
    private void ProcessOrders()
    {
        Order order = buffer.GetOneCell();
        if (order != null)
        {
            Thread orderProcessingThread = new Thread(() => OrderProcessing.ProcessOrder(order));
            orderProcessingThread.Start();
        }
    }
}
