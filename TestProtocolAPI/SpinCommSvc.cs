using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using T.ProtocolAPI;

namespace T.ProtocolDriver
{
	public class SpinCommSvc : ProtocolBase, IDisposable
	{
		#region methods
		#region load setting methods
		public override eReturn InitDeviceChannel(ChannelCfg channelCfg)
		{
			return base.InitDeviceChannel(channelCfg);
		}

		public override eReturn InitNode(DrvNode node)
		{
			return base.InitNode(node);
		}

		public override eReturn ParseAddress(string Address, DrvItem Item)
		{
			return base.ParseAddress(Address, Item);
		}

		#endregion

		public override eReturn BuildCommand(SessionMsg msg)
		{
			return base.BuildCommand(msg);
		}
		#endregion
	}
}
