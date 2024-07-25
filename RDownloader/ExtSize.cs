
using System;

namespace RDownloader
{
	public static class SizeExtension
	{
	    private const long OneKB = 1024;
	    private const long OneMB = OneKB * 1024;
	    private const long OneGB = OneMB * 1024;
	    private const long OneTB = OneGB * 1024;
	
	    public static string ToFileSize(this long value, int decimalPlaces = 0)
	    {
	        return ((double)value).ToFileSize(decimalPlaces);
	    }
	
	    public static string ToFileSize(this double value, int decimalPlaces = 0)
	    {
	        var TBObj = Math.Round((double)value / OneTB, decimalPlaces);
	        var GbObj = Math.Round((double)value / OneGB, decimalPlaces);
	        var MBObj = Math.Round((double)value / OneMB, decimalPlaces);
	        var KBObj = Math.Round((double)value / OneKB, decimalPlaces);
	        string result = TBObj > 1 ? string.Format("{0} TB",TBObj)
	            : GbObj > 1 ? string.Format("{0} GB",GbObj)
	            : MBObj > 1 ? string.Format("{0} MB",MBObj)
	            : KBObj > 1 ? string.Format("{0} KB",KBObj)
	            : string.Format("{0} bytes", Math.Round((double)value, decimalPlaces));
	        return result;
	    }
	}
}
