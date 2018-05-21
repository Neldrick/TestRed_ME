namespace TestRed_ME.Modules{
    public class Deal{
        public double price {get;set;}
        public double amount{get;set;}
    }
    public class LimitedOrder{
        public double price {get;set;}
        public double amount{get;set;}
    }
    public class MarketOrder{
        public double amount{get;set;}
        public bool buy{get;set;}
    }
    public class CancelOrder{
        public string orderId{get;set;}
    }
}