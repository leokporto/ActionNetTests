using lib60870;
using lib60870.CS101;
using lib60870.CS104;
using lib60870.linklayer;
using System;
using System.Net.Sockets;

namespace MasterClient
{
	internal class Program
	{
		private static void ConnectionHandler(object parameter, ConnectionEvent connectionEvent)
		{
			switch (connectionEvent)
			{
				case ConnectionEvent.OPENED:
					Console.WriteLine("Connected");
					break;
				case ConnectionEvent.CLOSED:
					Console.WriteLine("Connection closed");
					break;
				case ConnectionEvent.STARTDT_CON_RECEIVED:
					Console.WriteLine("STARTDT CON received");
					break;
				case ConnectionEvent.STOPDT_CON_RECEIVED:
					Console.WriteLine("STOPDT CON received");
					break;
			}
		}

		private static bool asduReceivedHandler(object parameter, ASDU asdu)
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
			else if (asdu.TypeId == TypeID.M_ME_NA_1)
			{
				for (int i = 0; i < asdu.NumberOfElements; i++)
				{

					var msv = (MeasuredValueNormalized)asdu.GetElement(i);

					Console.WriteLine("  IOA: " + msv.ObjectAddress + " scaled value: " + msv.NormalizedValue);
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
			else if (asdu.TypeId == TypeID.C_DC_NA_1) {
				if (asdu.Cot == CauseOfTransmission.ACTIVATION_CON) {
					var cmd = (DoubleCommand)asdu.GetElement(0);
					Console.WriteLine("  IOA: " + cmd.ObjectAddress + " state: " + cmd.State + ". Command Success!");
				}
			}
			else
			{
				Console.WriteLine("Unknown message type!");
			}

			return true;
		}

		public static void Main(string[] args)
		{
			bool running = true;

			Console.CancelKeyPress += delegate (object sender, ConsoleCancelEventArgs e)
			{
				e.Cancel = true;
				running = false;
			};

			Console.WriteLine("Using lib60870.NET version " + LibraryCommon.GetLibraryVersionString());

			APCIParameters aPCIParameters = new APCIParameters()
			{
				T0 = 30,
				T1 = 15,
				T2 = 10,
				T3 = 20,
				W = 8,
				K = 12
			};
			ApplicationLayerParameters applicationLayerParameters = new ApplicationLayerParameters()
			{
				OA = 1
			};

			Connection con = new Connection("127.0.0.1", 2404, aPCIParameters, applicationLayerParameters);

			con.DebugOutput = true;

			con.SetASDUReceivedHandler(asduReceivedHandler, null);
			con.SetConnectionHandler(ConnectionHandler, null);

			TryConnect(aPCIParameters, ref con);
			AutoResetEvent autoResetEvent = new AutoResetEvent(false);
			bool isOn = false;
			if(con.IsRunning)
			{
				Timer timer1 = new Timer((state)=> {
					if(isOn)
						con.SendControlCommand(CauseOfTransmission.ACTIVATION, 1, new DoubleCommand(4, DoubleCommand.ON, false, 0));
					else
						con.SendControlCommand(CauseOfTransmission.ACTIVATION, 1, new DoubleCommand(4, DoubleCommand.OFF, false, 0));
					isOn = !isOn;
				},autoResetEvent, 1000, 5000);
				
					
			}

			while (running)
			{
				Thread.Sleep(100);
			}

			con.Close();
		}


		private static void TryConnect(APCIParameters aPCIParameters, ref Connection con)
		{
			bool t0Reached = false;
			DateTime lastTried = DateTime.Now;

			do
			{

				try
				{
					con.Connect();

					break;
				}
				catch (ConnectionException e)
				{
					Console.WriteLine("SocketException: {0}", e.Message);
				}
				finally
				{
					TimeSpan ellapsed = DateTime.Now - lastTried;
					t0Reached = (ellapsed.TotalSeconds >= aPCIParameters.T0);
				}
				Thread.Sleep(1000);

			} while (!t0Reached);

			if (t0Reached)
			{
				Console.WriteLine("The connection timeout (T0) was reached. No connection to server.");
			}
		}
	}
}