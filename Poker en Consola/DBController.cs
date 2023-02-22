using Npgsql;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Poker_en_Consola
{
    internal class DBController
    {
        private string connectionString = "Host=localhost;Username=postgres;Password=ultradatabase;Database=casino";
    
    
        public List<PokerPlayer> GetAllPlayers()
        {
            List<PokerPlayer> players= new List<PokerPlayer>();
            string query = "select * from player";
            string connString = "Host=localhost;Port=5433;Username=postgres;Password=ultradatabase;Database=postgres";

            using (NpgsqlConnection conn = new NpgsqlConnection(connString))
            {
                NpgsqlCommand command = new NpgsqlCommand(query, conn);
                conn.Open();

                if (conn.State == System.Data.ConnectionState.Open)
                {
                    Console.WriteLine("Conectado");
                }

                NpgsqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    String nickname = reader.GetString(1);
                    PokerPlayer player = new PokerPlayer(nickname, 0, Actions.None, true);
                    players.Add(player);
                }

                // Escribir consultas, etc. 
                conn.Close();
            }

            return players;
        }

        public void updatePlayerPlayMoney(PokerPlayer player)
        {
            List<PokerPlayer> players = new List<PokerPlayer>();
            string idQuery = "select * from player where nickname = '" + player.nickname + "'";
            string connString = "Host=localhost;Port=5433;Username=postgres;Password=ultradatabase;Database=postgres";
            int playerId = -1; //because id value on DB will never be -1
            decimal playerPlayMoney = 0;

            using (NpgsqlConnection conn = new NpgsqlConnection(connString))
            {
                NpgsqlCommand command = new NpgsqlCommand(idQuery, conn);
                conn.Open();

                NpgsqlDataReader reader = command.ExecuteReader();


                while (reader.Read())
                {
                    playerId = reader.GetInt32(0);
                    playerPlayMoney = reader.GetDecimal(6);
                    Console.WriteLine("ID: {0}", playerId);
                }
                // Escribir consultas, etc.

                reader.Close();
                if (playerId != -1) //UPDATING PLAYMONEY ON DATABASE
                {
                    decimal newValue = playerPlayMoney + player.stack;
                    string moneyQuery = "UPDATE player SET playmoney = " + newValue + " WHERE id = " + playerId;
                    command.CommandText = moneyQuery;
                    command.ExecuteNonQuery();
                }

                conn.Close();
            }
        }

        public void updatePlayerRealMoney(PokerPlayer player)
        {
            List<PokerPlayer> players = new List<PokerPlayer>();
            string idQuery = "select * from player where nickname = '" + player.nickname + "'";
            string connString = "Host=localhost;Port=5433;Username=postgres;Password=ultradatabase;Database=postgres";
            int playerId = -1; //because id value on DB will never be -1
            decimal playerRealMoney = 0;

            using (NpgsqlConnection conn = new NpgsqlConnection(connString))
            {
                NpgsqlCommand command = new NpgsqlCommand(idQuery, conn);
                conn.Open();

                NpgsqlDataReader reader = command.ExecuteReader();


                while (reader.Read())
                {
                    playerId = reader.GetInt32(0);
                    playerRealMoney = reader.GetDecimal(5);
                    Console.WriteLine("ID: {0}", playerId);
                }
                // Escribir consultas, etc.

                reader.Close();
                if (playerId != -1) //UPDATING player RealMoney ON DATABASE
                {
                    decimal newValue = playerRealMoney + player.stack;
                    string moneyQuery = "UPDATE player SET realmoney = " + newValue + " WHERE id = " + playerId;
                    command.CommandText = moneyQuery;
                    command.ExecuteNonQuery();
                }

                conn.Close();
            }
        }

    }

}
