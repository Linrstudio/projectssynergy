#include <pic.h>

#include "UART.h"
#include "EP.h"


#define DEVICEID 1234

void main()
{
	ANS0 =0;
	ANS1 =0;
	ANS2 =0;
	ANS3 =0;
	ANS4 =0;
	ANS5 =0;
	ANS6 =0;
	ANS7 =0;
	ANS8 =0;
	ANS9 =0;
	ANS10=0;
	ANS11=0;

	TRISC0=0;
	TRISC1=0;
	TRISC2=0;

	TRISC7=0;
	TRISB4=0;
	UARTInit();
	UARTRead();
	Init();

	while(1)
	{
		Tick();
		if(UARTReadInt16()==1337)//found header this must be 
		{
			int16 DeviceID=UARTReadInt16();
			int8 event = UARTReadInt8();
			int16 args = UARTReadInt16();

			if(DeviceID==DEVICEID)
			{
				UARTWrite();
				if(event)
				{
					if(InvokeEvent(event,args))
					{
						UARTWriteInt8(255);
					}else{
						UARTWriteInt8(0);
					}
				}
				else
				{
					Polled();
					UARTWriteInt8(255);
				}
				UARTRead();
			}
		}
	}
}

