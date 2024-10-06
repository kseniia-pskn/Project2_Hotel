namespace Project2_Hotel
{
    public class Order
    {
        public string SenderId { get; private set; }
        public long CardNo { get; private set; }
        public double UnitPrice { get; private set; }
        public int Quantity { get; private set; }

        public Order(string senderId, long cardNo, double unitPrice, int quantity)
        {
            SenderId = senderId;
            CardNo = cardNo;
            UnitPrice = unitPrice;
            Quantity = quantity;
        }
    }
}
