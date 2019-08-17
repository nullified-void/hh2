using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
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
        public void add([FromBody] user newuser)
        {
            string connectionstring = System.IO.File.ReadAllLines("settings.pbl")[0];
            NpgsqlConnection npgSqlConnection = new NpgsqlConnection(connectionstring);
            npgSqlConnection.Open();
            NpgsqlCommand npgSqlCommand = new NpgsqlCommand(
                string.Format("insert into workers(\"FirstName\", \"SecondName\", \"LastName\", \"DateOfB\") " +
                "values (\'{{{0}}}\',\'{{{1}}}\',\'{{{2}}}\',\'{{{3}}}\')", 
                newuser.FirstName, 
                newuser.SecondName, 
                newuser.LastName, 
                newuser.DateOfB), 
                npgSqlConnection);
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
        // POST api/values/auth
        [HttpPost("auth")]
        public string auth([FromBody] log info)
        {
            string connectionstring = System.IO.File.ReadAllLines("settings.pbl")[0];
            NpgsqlConnection npgSqlConnection = new NpgsqlConnection(connectionstring);
            string storedpass = "";
            string access = "";
            npgSqlConnection.Open();
            NpgsqlCommand npgSqlCommand = new NpgsqlCommand(string.Format("select password, access from public.\"Logins\" where login = \'{{{0}}}\'",info.username), npgSqlConnection);
            NpgsqlDataReader r1 = npgSqlCommand.ExecuteReader();
            while (r1.Read())
            {
                access = ((string[])r1[1])[0];
                storedpass = ((string[])r1[0])[0];
            }
            r1.Close();
            npgSqlConnection.Close();
            if (verify(info.password, storedpass) == "true")
            {

                return access;
            }
            return "wrong";

        }
        public string hashing(string password)
        {
            byte[] salt;
            new RNGCryptoServiceProvider().GetBytes(salt = new byte[16]);
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000);
            byte[] hash = pbkdf2.GetBytes(20);
            byte[] hashBytes = new byte[36];
            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 20);
            string savedPasswordHash = Convert.ToBase64String(hashBytes);
            return savedPasswordHash;
        }
        public string verify(string password, string savedPasswordHash)
        {
            byte[] hashBytes = Convert.FromBase64String(savedPasswordHash);
            byte[] salt = new byte[16];
            Array.Copy(hashBytes, 0, salt, 0, 16);
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000);
            byte[] hash = pbkdf2.GetBytes(20);
            for (int i = 0; i < 20; i++)
                if (hashBytes[i + 16] != hash[i])
                    return "wrong";
            return "true";
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
    public struct log
    {
        public string username { get; set; }
        public string password { get; set; }
        public string access { get; set; }
    }

}
