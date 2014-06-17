using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BamBam.ScratchHelper
{
	class Arduino : IDisposable
	{
		private const string c_expectedFirmware = "BAMBAM_V01"; // The Arduino should return this string when a '?' is sent
		private string m_error; // If OpenAsync fails, this variable will contain a description of what went wrong
		private string m_comPort;
		private SerialPort m_serialPort;
		private object m_lock = new object(); // Used to synchronize access to the serial port
		private bool m_isOpen;

		private ManualResetEvent m_responseReceived = new ManualResetEvent( false );
		private string m_response;

		public Arduino( string comPort )
		{
			m_comPort = comPort;
		}

		public string ComPort
		{
			get
			{
				return m_comPort;
			}
		}

		public bool IsOpen
		{
			get
			{
				return m_isOpen;
			}
		}

		public Task<bool> OpenAsync()
		{
			return Task<bool>.Factory.StartNew( () =>
				{
					// Step 1 - Try to open COM port
					try
					{
						m_serialPort = new SerialPort( m_comPort, 115200 );
						m_serialPort.DtrEnable = true;
						m_serialPort.RtsEnable = true;
						m_serialPort.Open();
					}
					catch( Exception ex )
					{
						this.Error = "Unable to open port " + m_comPort + ": " + ex.Message;
						return m_isOpen;
					}

					// Step 2 - Send a '?' and check if the right firmware string is returned
					try
					{
						var buff = new byte[ 256 ];

						m_serialPort.DiscardInBuffer();
						m_serialPort.DataReceived += m_serialPort_DataReceived;
						m_serialPort.Write( new byte[] { (byte)'?' }, 0, 1 );
						if ( !m_responseReceived.WaitOne( TimeSpan.FromMilliseconds( 500 ) ) )
						{
							m_serialPort.Close();
							this.Error = "No response received from " + m_comPort;
							return m_isOpen;
						}

						if ( string.IsNullOrEmpty( m_response ) )
						{
							this.Error = "No valid Arduino on " + m_comPort + " (empty firmware response)";
							return m_isOpen;
						}

						if ( !m_response.StartsWith( c_expectedFirmware ) )
						{
							this.Error = "No valid Arduino on " + m_comPort + " (expected firmware " + c_expectedFirmware + ", but Arduino returned" + m_response + ")";
							return m_isOpen;
						}

						// Valid response received in time => All OK
						m_isOpen = true;
						return m_isOpen;
					}
					catch( Exception ex )
					{
						this.Error = "Unable to send data to " + m_comPort + ": " + ex.Message;
						return m_isOpen;
					}
				});
		}

		public string Error
		{
			get
			{
				return m_error;
			}
			private set
			{
				m_error = value;
			}
		}

		void m_serialPort_DataReceived( object sender, SerialDataReceivedEventArgs e )
		{
			var buffer = new byte[ 256 ];
			try
			{
				int bytesRead = m_serialPort.Read( buffer, 0, buffer.Length );
				m_response = Encoding.ASCII.GetString( buffer, 0, bytesRead );
				m_responseReceived.Set();
			}
			catch
			{
				// Can't do anything here...
			}
		}

		public void PulseOutput( int output )
		{
			var c = new byte[]{ (byte)('1' + output - 1) };

			lock ( m_lock )
			{
				m_serialPort.Write( c, 0, 1 );
			}
		}

		public void Dispose()
		{
			m_serialPort.Dispose();
			m_responseReceived.Dispose();
		}
	}
}
