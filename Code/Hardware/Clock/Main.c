#include <pic.h>

void main()
{
	GPWUF=0;
	CWUF=0;

	OPTION=PS2|PS0;

	TMR0=0;			//reset timer 0
	
	ANS0=0;			//disable analog
	ANS1=0;			//disable analog
	TRIS=0b110000;	//set in/out-puts

	GP2=1;
	while(1)
	{
		for(int d1=0;d1<125;d1++)
		{
			for(int d2=0;d2<5;d2++)
			{
				while(TMR0<25);
				TMR0-=25;
			}
		}

		GP2=!GP2;
	}
}