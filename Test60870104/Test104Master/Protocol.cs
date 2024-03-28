using lib60870.CS101;
using lib60870.CS104;
using lib60870;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Test104Master.ViewModel;

namespace Test104Master
{
	internal class Protocol : IDisposable
	{
		private Connection _conn104 = null;
		private ConnectionEvent _connectionStatus = ConnectionEvent.CLOSED;
		private bool disposedValue;

		internal delegate void ConnectionStatusHandler(string status);
		public event ConnectionStatusHandler ConnectionStatusChanged;

		internal void ConnectionHandler(object parameter, ConnectionEvent connectionEvent)
		{
			_connectionStatus = connectionEvent;
			string message = "";

			switch (connectionEvent)
			{
				case ConnectionEvent.OPENED:
					message = $"{DateTime.Now.ToLongTimeString()} - Connected";
					break;
				case ConnectionEvent.CLOSED:
					message = $"{DateTime.Now.ToLongTimeString()} - Connection closed";
					break;
				case ConnectionEvent.CONNECT_FAILED:
					message = $"{DateTime.Now.ToLongTimeString()} - Connection Failed";
					break;
				case ConnectionEvent.STARTDT_CON_RECEIVED:
					message = $"{DateTime.Now.ToLongTimeString()} - STARTDT CON received";
					break;
				case ConnectionEvent.STOPDT_CON_RECEIVED:
					message = $"{DateTime.Now.ToLongTimeString()} - STOPDT CON received";
					break;
			}

			ConnectionStatusChanged?.Invoke(message);
		}

		internal bool AsduReceivedHandler(object parameter, ASDU asdu)
		{
			Console.WriteLine(asdu.ToString());

			if (asdu.TypeId == TypeID.M_SP_NA_1)
			{				

				for (int i = 0; i < asdu.NumberOfElements; i++)
				{

					var val = (SinglePointInformation)asdu.GetElement(i);

					Console.WriteLine("  IOA: " + val.ObjectAddress + " SP value: " + val.Value);
					Console.WriteLine("   " + val.Quality.ToString());
				}
			}
			else if (asdu.TypeId == TypeID.M_ME_TE_1)
			{

				for (int i = 0; i < asdu.NumberOfElements; i++)
				{

					var msv = (MeasuredValueScaledWithCP56Time2a)asdu.GetElement(i);

					Console.WriteLine("  IOA: " + msv.ObjectAddress + " scaled value: " + msv.ScaledValue);
					Console.WriteLine("   " + msv.Quality.ToString());
					Console.WriteLine("   " + msv.Timestamp.ToString());
				}

			}
			else if (asdu.TypeId == TypeID.M_ME_TF_1)
			{

				for (int i = 0; i < asdu.NumberOfElements; i++)
				{
					var mfv = (MeasuredValueShortWithCP56Time2a)asdu.GetElement(i);

					Console.WriteLine("  IOA: " + mfv.ObjectAddress + " float value: " + mfv.Value);
					Console.WriteLine("   " + mfv.Quality.ToString());
					Console.WriteLine("   " + mfv.Timestamp.ToString());
					Console.WriteLine("   " + mfv.Timestamp.GetDateTime().ToString());
				}
			}
			else if (asdu.TypeId == TypeID.M_SP_TB_1)
			{

				for (int i = 0; i < asdu.NumberOfElements; i++)
				{

					var val = (SinglePointWithCP56Time2a)asdu.GetElement(i);

					Console.WriteLine("  IOA: " + val.ObjectAddress + " SP value: " + val.Value);
					Console.WriteLine("   " + val.Quality.ToString());
					Console.WriteLine("   " + val.Timestamp.ToString());
				}
			}
			else if (asdu.TypeId == TypeID.M_ME_NC_1)
			{

				for (int i = 0; i < asdu.NumberOfElements; i++)
				{
					var mfv = (MeasuredValueShort)asdu.GetElement(i);

					Console.WriteLine("  IOA: " + mfv.ObjectAddress + " float value: " + mfv.Value);
					Console.WriteLine("   " + mfv.Quality.ToString());
				}
			}
			else if (asdu.TypeId == TypeID.M_ME_NB_1)
			{

				for (int i = 0; i < asdu.NumberOfElements; i++)
				{

					var msv = (MeasuredValueScaled)asdu.GetElement(i);

					Console.WriteLine("  IOA: " + msv.ObjectAddress + " scaled value: " + msv.ScaledValue);
					Console.WriteLine("   " + msv.Quality.ToString());
					
				}

			}
			else if (asdu.TypeId == TypeID.M_ME_ND_1)
			{

				for (int i = 0; i < asdu.NumberOfElements; i++)
				{

					var msv = (MeasuredValueNormalizedWithoutQuality)asdu.GetElement(i);

					Console.WriteLine("  IOA: " + msv.ObjectAddress + " scaled value: " + msv.NormalizedValue);
				}

			}
			else if (asdu.TypeId == TypeID.C_IC_NA_1)
			{
				if (asdu.Cot == CauseOfTransmission.ACTIVATION_CON)
					Console.WriteLine((asdu.IsNegative ? "Negative" : "Positive") + "confirmation for interrogation command");
				else if (asdu.Cot == CauseOfTransmission.ACTIVATION_TERMINATION)
					Console.WriteLine("Interrogation command terminated");
			}
			else
			{
				Console.WriteLine("Unknown message type!");
			}

			return true;
		}

		internal void OpenConnection() 
		{
			try
			{


				_conn104 = new Connection("127.0.0.1");

				_conn104.DebugOutput = true;

				_conn104.SetASDUReceivedHandler(AsduReceivedHandler, null);
				_conn104.SetConnectionHandler(ConnectionHandler, null);
				_conn104.Connect();

				
			}
			catch (Exception ex)
			{
				Console.WriteLine("Exception: " + ex.Message);
			}
		}

		internal void CloseConnection() 
		{
			try
			{
				_conn104?.Close();
			}
			catch (Exception ex)
			{
				Console.WriteLine("Exception: " + ex.Message);
			}
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					// TODO: dispose managed state (managed objects)
				}

				// TODO: free unmanaged resources (unmanaged objects) and override finalizer
				// TODO: set large fields to null

				if(_connectionStatus != ConnectionEvent.CLOSED)
					_conn104?.Close();

				disposedValue = true;
			}
		}

		// // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
		// ~Protocol()
		// {
		//     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
		//     Dispose(disposing: false);
		// }

		public void Dispose()
		{
			// Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}
	}
}
