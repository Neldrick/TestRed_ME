using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using StackExchange.Redis;
using TestRed_ME.Modules;
using WSModules;

namespace TestRed_ME.Controllers{
    [Route("api/[controller]")]
    public class BidAskController:Controller{
        private ChatRoomHandler chatRoomHandler{get;set;}
        public BidAskController (ChatRoomHandler handler){
            chatRoomHandler = handler;
        }
        [HttpGet]
        public async Task Get(){
            OrderResult  oResult= new OrderResult();
            using (var connection = ConnectionMultiplexer.Connect($"127.0.0.1:6379")){
                    var redisDb = connection.GetDatabase();
                    
                    oResult.bidask = (string)redisDb.Execute(
                        "queryfirstbidask",new object[] {"BTCUSD","0","5"});
                    

            }
            chatRoomHandler.SendMessageToAllAsync(JsonConvert.SerializeObject(oResult));
       
           
        }
        

    }
}