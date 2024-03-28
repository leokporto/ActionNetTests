using lib60870.CS101;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test104Master.Extensions
{
	public static class QualityExtensions
	{
		public static byte GetOpcQuality(this QualityDescriptor quality) 
		{
			byte result = 0;

			if (quality.Invalid || quality.Substituted || quality.Blocked || quality.Overflow || quality.NonTopical)
			{
				result =  0;
			}
			else
			{
				result = 192;
			}

			return result;
		}
	}
}
