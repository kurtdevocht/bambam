﻿using Microsoft.Owin.Hosting;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BamBam.ScratchHelper
{
	class Program
	{
		static void Main( string[] args )
		{
			Console.WriteLine( "BamBam Scratch Helper" );
			Console.WriteLine();
			Console.WriteLine();

			if ( !FindArduino() )
			{
				Console.WriteLine( "ERROR -- No Arduino found an any COM port:" );
				Console.WriteLine( "    - Verify all connections" );
				Console.WriteLine( "    - Make sure no other program is using the COM port" );
				Console.WriteLine( "    - Make sure the right sketch is loaded on your Arduino" );
				return;
			}

			Console.WriteLine();
			Console.WriteLine();
			string baseAddress = "http://localhost:15000/";

			try
			{
				// Start OWIN host 
				using ( WebApp.Start<Startup>( url: baseAddress ) )
				{
					Console.WriteLine( "Listening on " + baseAddress.ToString() + " for Scratch requests." );
					Console.WriteLine();
					Console.WriteLine( "Press enter to stop..." );
					Console.ReadLine();
				}
			}
			catch( Exception ex )
			{
				Console.WriteLine( "ERROR -- Cannot listen on " + baseAddress + " for Scratch requests: " + ex.Message);
				Console.WriteLine();
				Console.WriteLine( "Press enter to stop..." );
				Console.ReadLine();
			}
		}

		private static bool FindArduino()
		{
			Console.WriteLine( "Looking for Arduino's on all COM ports..." );
			var arduinos = SerialPort.GetPortNames().Select( p => new Arduino( p ) ).ToList();
			Task.WaitAll( arduinos.Select( a => a.OpenAsync() ).ToArray() );

			Arduino arduino = null;
			foreach ( var a in arduinos.OrderBy( a => a.ComPort ) )
			{
				Console.Write( "    " + a.ComPort + ": " );
				if ( a.IsOpen )
				{
					if ( arduino == null )
					{
						Console.WriteLine( "Good Arduino! Will take that one..." );
						arduino = a;
					}
					else
					{
						Console.WriteLine( "Good Arduino too, but already took " + arduino.ComPort );
						a.Dispose();
					}
				}
				else
				{
					Console.WriteLine( a.Error );
				}
			}

			if ( arduino == null )
			{
				return false;
			}

			Globals.Arduino = arduino;
			return true;
		}
	}
}