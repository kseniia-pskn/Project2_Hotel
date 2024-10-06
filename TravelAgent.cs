using System;
using System.Threading;

namespace Project2_Hotel
{
    public delegate void OrderCreationEvent(Order order);

    public class TravelAgent
    {
        public static event OrderCreationEvent? OrderCreation; // Event for creating an order
        private string agentId;
        private MultiCellBuffer buffer;
        private bool keepRunning = true;

        public TravelAgent(string agentId, MultiCellBuffer buffer)
        {
            this.agentId = agentId;
            this.buffer = buffer;
        }

        // Method to stop the agent
        public void Stop()
        {
            keepRunning = false;
        }

        // Method called when a price cut event occurs
        public void OnPriceCut(double roomPrice, string hotelId)
        {
            Console.WriteLine($"{agentId} received price cut notification from {hotelId}. New price: {roomPrice}");
            CreateOrder(roomPrice);
        }

        // Create an order and add it to the buffer
        private void CreateOrder(double roomPrice)
        {
            Random rnd = new Random();
            long cardNo = rnd.Next(5000, 8000);
            int quantity = rnd.Next(1, 5);
            Order order = new Order(agentId, cardNo, roomPrice, quantity);
            OrderCreation?.Invoke(order);
            buffer.SetOneCell(order);
            Console.WriteLine($"{agentId} created an order for {quantity} rooms at ${roomPrice} each.");
        }

        // Method to start the travel agent thread
        public void Start()
        {
            Random rnd = new Random();
            while (keepRunning)
            {
                Thread.Sleep(rnd.Next(1000, 5000)); // Random wait to simulate activity
                CreateOrder(rnd.Next(80, 160)); 
            }
        }

        // Callback method to confirm processed orders
        public void OrderProcessConfirm(Order order, double orderAmount)
        {
            Console.WriteLine($"{agentId} received confirmation for order. Total amount: ${orderAmount}");
        }
    }
}
