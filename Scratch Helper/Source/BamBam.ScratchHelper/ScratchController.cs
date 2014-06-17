using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace BamBam.ScratchHelper
{
	public class ScratchController : ApiController
	{
		[Route( "poll" )]
		[HttpGet]
		public HttpResponseMessage Poll()
		{
			var resp = new HttpResponseMessage( HttpStatusCode.OK );
			resp.Content = new StringContent( string.Empty, Encoding.UTF8, "text/plain" );
			return resp;
		}

		[Route( "bam/{output}" )]
		[HttpGet]
		public HttpResponseMessage Bam( int output)
		{
			Globals.Arduino.PulseOutput( output );
			var resp = new HttpResponseMessage( HttpStatusCode.OK );
			resp.Content = new StringContent( string.Empty, Encoding.UTF8, "text/plain" );
			return resp;
		}
	}
}
