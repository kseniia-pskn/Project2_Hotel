using System;
using System.Threading;

namespace Project2_Hotel
{
    class Program
    {
        static void Main(string[] args)
        {
            MultiCellBuffer buffer = new MultiCellBuffer();
            Hotel hotel = new Hotel(buffer);
            TravelAgent agent1 = new TravelAgent("Agent1", buffer);
            TravelAgent agent2 = new TravelAgent("Agent2", buffer);

            Console.WriteLine("Starting hotel and agent threads...");

            Thread hotelThread = new Thread(hotel.Start);
            Thread agentThread1 = new Thread(agent1.Start);
            Thread agentThread2 = new Thread(agent2.Start);

            hotelThread.Start();
            agentThread1.Start();
            agentThread2.Start();

            hotelThread.Join();
            agentThread1.Join();
            agentThread2.Join();

            Console.WriteLine("All threads have finished.");
        }
    }
}
