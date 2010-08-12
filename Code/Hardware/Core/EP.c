#include"Default.h"
#include"UART.h"
#include"EP.h"

#include "Compiler.h"

#include "HardwareProfile.h"

void EPInit()//initialize Endpoints
{
	
}

void EPUpdate()
{
	if(EPPoll(1234))
	{
		TRISBbits.TRISB4=0;
		PORTBbits.RB4=0;
		TRISCbits.TRISC2=0;
		PORTCbits.RC2=1;
	}else{
		TRISBbits.TRISB4=0;
		PORTBbits.RB4=1;
		TRISCbits.TRISC2=0;
		PORTCbits.RC2=0;
	}
	//EPPoll(12);
}

int8 EPInvokeEvent(int16 _DeviceID,int8 _Event,int16 _Args)
{
	int8 timeout;
	int8 i;
	for(timeout=0;timeout<3;timeout++)//try three times
	{
		UARTWrite();
		UARTWriteInt16(1337);
		UARTWriteInt16(_DeviceID);
		UARTWriteInt8(_Event);
		UARTWriteInt16(_Args);
		UARTRead();
		for(i=0;i<255;i++)
			if(UARTAvailable()&&UARTReadInt8())
			{
				return 255;
			}
	}
	return 0;
}

//returns 0 if device not reachable
int8 EPPoll(int16 _DeviceID)
{
	int8 timeout;
	int8 i;
	int8 j;
	int8 k;
	//for(i=0;i<255;i++)for(j=0;j<255;j++);
	for(timeout=0;timeout<3;timeout++)//try three times
	{
		UARTWrite();
		UARTWriteInt16(1337);
		UARTWriteInt16(_DeviceID);
		UARTWriteInt8(0);
		UARTWriteInt16(0);
		UARTRead();
		for(i=0;i<255;i++)for(j=0;j<255;j++)
		{
			if(UARTAvailable()!=0)
			{
				if(UARTReadInt8()==255)
					return 255;
			}
		}
	}
	return 0;
}
