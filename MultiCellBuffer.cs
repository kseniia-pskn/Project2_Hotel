using System;
using System.Threading;

namespace Project2_Hotel
{
    public class MultiCellBuffer
    {
        private const int bufferSize = 3;
        private Order[] multiCells = new Order[bufferSize];
        private SemaphoreSlim setSemaph = new SemaphoreSlim(bufferSize, bufferSize);
        private SemaphoreSlim getSemaph = new SemaphoreSlim(0, bufferSize);

        public void SetOneCell(Order order)
        {
            setSemaph.Wait();
            lock (multiCells)
            {
                for (int i = 0; i < bufferSize; i++)
                {
                    if (multiCells[i] == null)
                    {
                        multiCells[i] = order;
                        break;
                    }
                }
            }
            getSemaph.Release();
        }

        public Order GetOneCell()
        {
            getSemaph.Wait();
            lock (multiCells)
            {
                for (int i = 0; i < bufferSize; i++)
                {
                    if (multiCells[i] != null)
                    {
                        Order order = multiCells[i];
                        multiCells[i] = null;
                        setSemaph.Release();
                        return order;
                    }
                }
            }
            return null;
        }
    }
}
