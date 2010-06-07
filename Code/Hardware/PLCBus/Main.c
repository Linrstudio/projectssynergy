#include "Default.h"
#include "NLCMaster.h"

#define ICTX GP2
#define ICRX GP1
#define ICCLK GP0

void Delay()
{
	for(int i=0;i<32;i++);
}

void ICWaitForStart()
{
	while(1)
	{
		while(!ICCLK);			//HI
		while(ICCLK);			//LO
		if(ICRX)return;
	}
}

int8 ICReadByte()
{
	int8 i=128;
	int8 dat=0;
	do
	{
		while(!ICCLK);			//HI
		if(ICRX)dat|=i;
		while(ICCLK);			//LO
		i>>=1;
	}while(i!=0);
	return dat;
}

void main()
{
	GPWUF=0;
	CWUF=0;

	OPTION = 0;		//okay do not ask why i need to do this but somehow it enables GP2
	TMR0=0;			
	C1ON=0;			
	ANS0=0;			//disable analog ?
	ANS1=0;			//disable analog ?

 	TRIS=0b00000011;	//set in/out-puts

	ICTX=0;

	while(1)
	{
		//for(int i=0;i<255;i++)for(int i2=0;i2<255;i2++);
		NLCWriteStart();
		NLCWriteByte(0);
		NLCWriteByte(0);
		NLCWriteByte(0);
		NLCWriteByte(100);
		//for(int i=0;i<255;i++)for(int i2=0;i2<255;i2++);
		NLCWriteStart();
		NLCWriteByte(0);
		NLCWriteByte(0);
		NLCWriteByte(0);
		NLCWriteByte(101);
	}
#if 0
	while(1)
	{	
		ICWaitForStart();
		switch(ICReadByte())
		{
			case 2:
			{
				int8 recipienthi = ICReadByte();
				int8 recipientlo = ICReadByte();
				int8 eventid 	 = ICReadByte();
				int8 argumenthi  = ICReadByte();
				int8 argumentlo  = ICReadByte();
				ICTX=1;
				for(int8 i=0;i<3;i++)//try to send stuff three times
				{
					WriteStart();
					WriteByte(1);
					WriteByte(recipienthi);
					WriteByte(recipientlo);
					WriteByte(eventid);
					WriteByte(argumenthi);
					WriteByte(argumentlo);
					int8 t = ReadByte();
					int8 eventhi = ReadByte();
					int8 eventlo = ReadByte();
					int8 argshi  = ReadByte();
					int8 argslo  = ReadByte();
					if(t==1)
					{
						
						break;
					}
				}
				ICTX=0;
			}
			break;
		}
	}
#endif
}
