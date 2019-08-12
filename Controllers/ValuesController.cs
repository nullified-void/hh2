using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using MySql.Data.MySqlClient;

namespace WebApplication4.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class ValuesController : ControllerBase
    {
        public static MySqlConnection connection;

        // POST api/values/get
        [HttpPost("get")]
        public ActionResult<string> Post([FromBody] user newuser)
        {
            string Json = "";
            string connectionstring = System.IO.File.ReadAllLines("settings.pbl")[0];
            connection = new MySqlConnection(connectionstring);
            connection.Open();
            MySqlCommand com1 = new MySqlCommand("", connection);
            com1.CommandText = "select * from workers";
            MySqlDataReader r1 = com1.ExecuteReader();
            while (r1.Read())
            {
                newuser.id = r1[0].ToString();
                newuser.FirstName = r1[1].ToString();
                newuser.SecondName = r1[2].ToString();
                newuser.LastName = r1[3].ToString();
                newuser.DateOfB = r1[4].ToString();
                Json += JsonConvert.SerializeObject(newuser);
            }
            r1.Close();
            connection.Close();
            return Json;

        }
        // POST api/values
        [HttpPost("add")]
        public ActionResult<string> Post2([FromBody] user newuser)
        {
            return "";
        }

    }
    public struct user
    {
        public string id { get; set; }
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public string LastName { get; set; }
        public string DateOfB { get; set; }
    }
}
