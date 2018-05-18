using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
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
            
            using (var connection = ConnectionMultiplexer.Connect($"127.0.0.1:6379")){
                    var redisDb = connection.GetDatabase();
                    RedisValue[] values = {"BTCUSD","1","10"};
                    string  result = (string)redisDb.Execute("queryfirstbidask",values);
                    chatRoomHandler.SendMessageToAllAsync(result);

            }
           
        }
        

    }
}