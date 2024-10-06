using System;
using System.Threading;

namespace Project2_Hotel
{
    public delegate void PriceCutEvent(double newPrice, string hotelName);

    public class Hotel
    {
        public static event PriceCutEvent? PriceCut; // Event to notify travel agents of a price drop
        private double currentRoomPrice = 100; // Initial room price
        private MultiCellBuffer buffer;
        private int priceCutCounter = 0; // Limit the number of price cuts to 10
        private bool keepRunning = true;

        public Hotel(MultiCellBuffer buffer)
        {
            this.buffer = buffer;
        }

        // Method to stop the hotel thread
        public void Stop()
        {
            keepRunning = false;
        }

        // Method to simulate price updates and notify agents on price drop
        public void Start()
        {
            Random rnd = new Random();
            while (priceCutCounter < 10 && keepRunning) // Stop after 10 price cuts or when stopped
            {
                Thread.Sleep(rnd.Next(1000, 3000)); // Simulate time taken for price updates
                double newPrice = pricingModel();

                if (newPrice < currentRoomPrice) // Emit event if price drops
                {
                    priceCutCounter++;
                    PriceCut?.Invoke(newPrice, "Hotel1");
                    Console.WriteLine($"Price cut! New price is {newPrice}");
                }
                else
                {
                    Console.WriteLine($"No price cut. Current price remains {currentRoomPrice}");
                }

                currentRoomPrice = newPrice;
                ProcessOrders();
            }

            Console.WriteLine("Hotel has stopped price cuts after 10 reductions.");
            Stop(); // Stop running agents once hotel is done
        }

        // Pricing model to generate new prices
        public double pricingModel()
        {
            Random rnd = new Random();
            // Generate a price between 70 and 150 to ensure fluctuations
            return rnd.Next(70, 150); 
        }

        // Process orders from the buffer
        private void ProcessOrders()
        {
            Order? order = buffer.GetOneCell();
            if (order != null)
            {
                Console.WriteLine("Processing an order...");
                Thread orderProcessingThread = new Thread(() => OrderProcessing.ProcessOrder(order));
                orderProcessingThread.Start();
            }
            else
            {
                Console.WriteLine("No orders to process.");
            }
        }
    }
}
