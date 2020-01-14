package
{
	import flash.utils.getTimer;
	
	public class ACTFlashGate
	{
	    public static function GetMillisecondsFromStart(): Number
	    {
			var timer:Number = getTimer();
			return timer;
	    }
	    
	    public static function GetMillisecondsFromDate(): Number
	    {
			var dateTime:Number = new Date().time;
			return dateTime;
	    }
	}
}