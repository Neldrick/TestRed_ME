using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using WSModules;

namespace TestRed_ME.Controllers{
    [Route("api/[controller]")]
    public class DealController:Controller{
        private ChatRoomHandler chatRoomHandler{get;set;}
        public DealController (ChatRoomHandler handler){
            chatRoomHandler = handler;
        }
        [HttpGet]
        public async Task Post(string json){
            chatRoomHandler.SendMessageToAllAsync(json);
        }
        

    }
}