// <copyright file="ChatServer.cs" company="UofU-CS3500">
// Copyright (c) 2024 UofU-CS3500. All rights reserved.
// Authors: Kailey Nothamn, Callum Dingley
// Version: 3/25/2025
// </copyright>

using CS3500.Networking;

namespace CS3500.Chatting;

/// <summary>
///   A simple ChatServer that handles clients separately and allows each client to interact with the others (on the 
///   same server).
/// </summary>
public partial class ChatServer
{
	/// <summary>
	/// List of clients that are currently connected to the server.
	/// </summary>
	private static List<NetworkConnection> clients = [];

	/// <summary>
	///   The main program.
	/// </summary>
	/// <param name="args"> ignored. </param>
	/// <returns> A Task. Not really used. </returns>
	private static void Main( string[] args )
    {
        Server.StartServer( HandleConnect, 11_000 );
        Console.Read(); // don't stop the program.
    }

    /// <summary>
    ///   <pre>
    ///     When a new connection is established, enter a loop that receives from and
    ///     replies to a client.
    ///   </pre>
    /// </summary>
    private static void HandleConnect( NetworkConnection connection )
    {
        try
        {
			lock (clients)
			{
				clients.Add(connection);
			}

			connection.Name = connection.ReadLine();
			connection.Send("Your name has been set as: " + connection.Name);

			while ( true )
            {
				var message = connection.ReadLine();

				lock (clients)
				{
					foreach (NetworkConnection conn in clients)
					{
						conn.Send(connection.Name + " said: " + message);
					}
				}
            }
        }
        catch ( Exception e)
        {
			lock (clients)
			{
				clients.Remove(connection);
			}

			foreach (NetworkConnection conn in clients)
			{
				conn.Send(e.Message.Trim());
			}
		}
    }
}