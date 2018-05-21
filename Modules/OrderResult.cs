namespace TestRed_ME.Modules{
    public class OrderResult{
         public string orderId{get;set;}
         public string bidask{get;set;}
         public string executedPrice{get;set;}
    }
    public class CancelOrderResult{
        public string Status{get;set;}
        public ServerReturnOrder Order{get;set;}
        public double CurrentBid{get;set;}
        public double CurrentAsk{get;set;}
    }
    
}