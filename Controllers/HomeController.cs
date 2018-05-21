using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using StackExchange.Redis;
using TestRed_ME.Models;
using TestRed_ME.Modules;
using WSModules;

namespace TestRed_ME.Controllers
{
    
    public class HomeController : Controller
    {
        private  ChatRoomHandler chatHandler {get;set;}
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }
        public string  getFirstBidAsk(){
            string result = "";
             using (var connection = ConnectionMultiplexer.Connect($"127.0.0.1:6379")){
                    var redisDb = connection.GetDatabase();
                    
                     result = (string)redisDb.Execute(
                        "queryfirstbidask",new object[] {"BTCUSD","0","5"});
                    
            }
            return result;
        }
        public JsonResult buyLimitedOrder([FromBody] LimitedOrder order){
            string []result; 
            
            OrderResult  oResult= new OrderResult();
            if(order!=null && order.price!=0 &&order.amount!=0){
             using (var connection = ConnectionMultiplexer.Connect($"127.0.0.1:6379")){
                    var redisDb = connection.GetDatabase();
                    
                     result = (string[])redisDb.Execute(
                        "buylimitedOrder",new object[] {"BTCUSD","666666",order.price.ToString(),order.amount.ToString(),"2","0"});
                     double bid,ask;
                     oResult.orderId=(result[1].Replace("\"ServerOrderId\":\"","")).Replace("\"","");
                     oResult.executedPrice= result[result.Length-1];
                     if(oResult.executedPrice.Contains("{}")){
                             bid = double.Parse(result[3].Replace("\"CurrentBid\":","")); //\"CurrentBid\":\"
                             ask =  double.Parse(result[4].Replace("\"CurrentAsk\":",""));
                             if(order.price>(bid-10) ||order.price>(ask-10)){
                                  oResult.bidask = (string)redisDb.Execute(
                                    "queryfirstbidask",new object[] {"BTCUSD","0","5"});
                             }
                             oResult.executedPrice="{}";
                     }
                     else{
                         oResult.executedPrice= oResult.executedPrice.Replace("\"PriceExecuted\":","");

                         oResult.bidask = (string)redisDb.Execute(
                        "queryfirstbidask",new object[] {"BTCUSD","0","5"});
                     }
            }
            }
            
               return Json(oResult);
        }
         public JsonResult sellLimitedOrder([FromBody] LimitedOrder order){
            string []result; 
            
            OrderResult  oResult= new OrderResult();
            if(order!=null && order.price!=0 &&order.amount!=0){
             using (var connection = ConnectionMultiplexer.Connect($"127.0.0.1:6379")){
                    var redisDb = connection.GetDatabase();
                    
                     result = (string[])redisDb.Execute(
                        "selllimitedOrder",new object[] {"BTCUSD","666666",order.price.ToString(),order.amount.ToString(),"2","0"});
                     double bid,ask;
                     oResult.orderId=(result[1].Replace("\"ServerOrderId\":\"","")).Replace("\"","");
                     oResult.executedPrice= result[result.Length-1];
                     if(oResult.executedPrice.Contains("{}")){
                             bid = double.Parse(result[3].Replace("\"CurrentBid\":","")); //\"CurrentBid\":\"
                             ask =  double.Parse(result[4].Replace("\"CurrentAsk\":",""));
                             if(order.price<(bid+10) ||order.price<(ask+10)){
                                  oResult.bidask = (string)redisDb.Execute(
                                    "queryfirstbidask",new object[] {"BTCUSD","0","5"});
                             }
                             oResult.executedPrice="{}";
                     }
                     else{
                         oResult.executedPrice= oResult.executedPrice.Replace("\"PriceExecuted\":","");

                         oResult.bidask = (string)redisDb.Execute(
                        "queryfirstbidask",new object[] {"BTCUSD","0","5"});
                     }
            }
            }
            
               return Json(oResult);
        }
        public JsonResult marketOrder([FromBody] MarketOrder order){
            string []result; 
            
            OrderResult  oResult= new OrderResult();
            if(order!=null &&order.amount!=0){
             using (var connection = ConnectionMultiplexer.Connect($"127.0.0.1:6379")){
                    var redisDb = connection.GetDatabase();
                    string command="";
                    if(order.buy){
                        command="buymarketorder";

                    }
                    else{
                        command = "sellmarketorder";
                    }
                     result = (string[])redisDb.Execute(
                        command,new object[] {"BTCUSD",order.amount.ToString(),"2","0"});
                     double bid,ask;
                     oResult.orderId=(result[1].Replace("\"ServerOrderId\":\"","")).Replace("\"","");
                     oResult.executedPrice= result[result.Length-1];
                     if(!oResult.executedPrice.Contains("{}")){
                            
                         oResult.executedPrice= oResult.executedPrice.Replace("\"PriceExecuted\":","");

                         oResult.bidask = (string)redisDb.Execute(
                        "queryfirstbidask",new object[] {"BTCUSD","0","5"});
                     }
            }
            }
            
               return Json(oResult);
        }
         public JsonResult cancelOrder([FromBody] CancelOrder order){
             OrderResult  oResult= new OrderResult();
            if(order.orderId!=null && order.orderId !=""){
                using (var connection = ConnectionMultiplexer.Connect($"127.0.0.1:6379")){
                        var redisDb = connection.GetDatabase();
                        
                        string result = (string)redisDb.Execute("cancelorder",new object[] {order.orderId,"2","0"});
                        if(result.Contains("Error")){
                            return Json(oResult);
                        }
                        else{
                            
                                CancelOrderResult cResult= JsonConvert.DeserializeObject<CancelOrderResult>(result);
                                double price = double.Parse(cResult.Order.Price);
                                if(cResult.Order.BuySell=="0"){
if(price>(cResult.CurrentBid-10) ||price>(cResult.CurrentAsk-10)){
                                        oResult.bidask = (string)redisDb.Execute(
                                            "queryfirstbidask",new object[] {"BTCUSD","0","5"});
                                    }
                                    else{
                                        oResult.bidask="{}";
                                    }
                                }
                                else{
                                    if(price<(cResult.CurrentBid-10) ||price<(cResult.CurrentAsk-10)){
                                        oResult.bidask = (string)redisDb.Execute(
                                            "queryfirstbidask",new object[] {"BTCUSD","0","5"});
                                    }
                                    else{
                                        oResult.bidask="{}";
                                    }
                                }
                                
                                oResult.executedPrice= "{}";

                        }
                
                }
            }
            
               return Json(oResult);
         }
        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
