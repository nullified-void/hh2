using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Npgsql;

namespace WebApplication4.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class ValuesController : ControllerBase
    {

        //
        // POST api/values/get
        [HttpPost("get")]
        public ActionResult<string> get()
        {
            string connectionstring = System.IO.File.ReadAllLines("settings.pbl")[0];
            NpgsqlConnection npgSqlConnection = new NpgsqlConnection(connectionstring);
            List<user> newuser = new List<user>();
            string Json = "";
            npgSqlConnection.Open();
            NpgsqlCommand npgSqlCommand = new NpgsqlCommand("select * from workers", npgSqlConnection);
            NpgsqlDataReader r1 = npgSqlCommand.ExecuteReader();
            while (r1.Read())
            {
                user olduser = new user();
                olduser.id = r1[0].ToString();
                olduser.FirstName = ((string[])r1[1])[0];
                olduser.SecondName = ((string[])r1[2])[0];
                olduser.LastName = ((string[])r1[3])[0];
                olduser.DateOfB = ((DateTime)r1[4]).ToString() ;
                newuser.Add(olduser);
            }
            Json = JsonConvert.SerializeObject(newuser);
            r1.Close();
            npgSqlConnection.Close();
            return Json;

        }
        // POST api/values/add
        [HttpPost("add")]
        public void add()
        {
            string connectionstring = System.IO.File.ReadAllLines("settings.pbl")[0];
            NpgsqlConnection npgSqlConnection = new NpgsqlConnection(connectionstring);
            npgSqlConnection.Open();
            NpgsqlCommand npgSqlCommand = new NpgsqlCommand("insert into workers(\"FirstName\", \"SecondName\", \"LastName\", \"DateOfB\") values (\'{-}\',\'{-}\',\'{-}\',\'2000-01-01\')", npgSqlConnection);
            npgSqlCommand.ExecuteNonQuery();
            npgSqlConnection.Close();
        }
        // POST api/values/delete
        [HttpPost("delete")]
        public void delete([FromBody] user newuser)
        {
            string connectionstring = System.IO.File.ReadAllLines("settings.pbl")[0];
            NpgsqlConnection npgSqlConnection = new NpgsqlConnection(connectionstring);
            npgSqlConnection.Open();
            NpgsqlCommand npgSqlCommand = new NpgsqlCommand(String.Format("delete from workers where id = {0}", newuser.id), npgSqlConnection);
            npgSqlCommand.ExecuteNonQuery();
            npgSqlConnection.Close();
        }
        // POST api/values/edit
        [HttpPost("edit")]
        public void edit([FromBody] user newuser)
        {

            string connectionstring = System.IO.File.ReadAllLines("settings.pbl")[0];
            NpgsqlConnection npgSqlConnection = new NpgsqlConnection(connectionstring);
            npgSqlConnection.Open();
            NpgsqlCommand npgSqlCommand = new NpgsqlCommand(String.Format("update workers set " +
                "\"FirstName\" = \'{{{0}}}\'," +
                " \"SecondName\" = \'{{{1}}}\', " +
                "\"LastName\" = \'{{{2}}}\', " +
                "\"DateOfB\" = \'{3}\' " +
                "where \"id\" = {4}", 
                newuser.FirstName,
                newuser.SecondName,
                newuser.LastName,
                newuser.DateOfB,
                newuser.id), npgSqlConnection);
            npgSqlCommand.ExecuteNonQuery();
            npgSqlConnection.Close();
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
