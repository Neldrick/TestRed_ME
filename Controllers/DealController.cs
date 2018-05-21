using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using WSModules;
using TestRed_ME.Modules;
using Newtonsoft.Json;

namespace TestRed_ME.Controllers{
    [Route("api/[controller]")]
    public class DealController:Controller{
        private ChatRoomHandler chatRoomHandler{get;set;}
        public DealController (ChatRoomHandler handler){
            chatRoomHandler = handler;
        }
        [HttpPost]
        public async Task Post(string json){
             OrderResult  oResult= new OrderResult();
                         using (var connection = ConnectionMultiplexer.Connect($"127.0.0.1:6379")){
                        var redisDb = connection.GetDatabase();
                        
            oResult.executedPrice = json;
            oResult.bidask = (string)redisDb.Execute(
                                            "queryfirstbidask",new object[] {"BTCUSD","0","5"});

                         }
             chatRoomHandler.SendMessageToAllAsync(JsonConvert.SerializeObject(oResult));
        }
        

    }
}