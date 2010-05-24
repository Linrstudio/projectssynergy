#include <pic18.h>

#include "UART.h"
#include "Memory.h"
#include "RTC.h"
#include "Kismet.h"
#include "PLC.h"
#include "Settings.h"

#define LED1 RA4
#define LED2 RA5

//#define USETICKCOUNTER

void UARTUpdate();
#ifdef USETICKCOUNTER
int16 tickcount=0;
#endif

void main()
{
	//set clockspeed
	IRCF0=1;
	IRCF1=1;
	IRCF2=1;

	//configure debug leds
	TRISA4=0;
	TRISA5=0;

	//disable analog pins
	ANSEL = 0; 
	ANSELH = 0;

	LED1=1;
	LED2=1;

	PLCInit();
	RTCInit();
	UARTInit();
	MemoryInit();
	KismetInit();
	SettingsInit();
	PLCInit();
	
	while(1)//main uber duber loop
	{
		UARTUpdate();//Handle UART crap
		RTCUpdate ();//Handle realtime clock
		PLCUpdate ();//Handle PowerLineCommunications
#ifdef USETICKCOUNTER
		tickcount++;
		if(tickcount>=5000)//each blink indicates 10000Frames
		{
			tickcount=0;
			LED1=!LED1;
		}
#endif
	}
}

void UARTUpdate()
{
	if(UARTAvailable())
	{
		char read=UARTReadInt8();
		UARTWriteInt8('y');///yea whats up ?
		switch(read)
		{
			case 'h'://Hello!
			{				
				UARTWriteInt16(SettingsReadInt16(0));
			}break;

			case 's'://Settings stuff
			{
				int8 op = UARTReadInt8();
				switch(op)
				{
					case 'e'://Set external EEPROM Size
					{
						SettingsWriteInt16(0,UARTReadInt16());
					}break;
				}
			}break;

			case 'm'://memory stuff
			{
				int8 op = UARTReadInt8();
				switch(op)
				{
					case 'w'://write to memory
					{
						int16 addr=UARTReadInt16();
						int8 buffer[16];
						for(int i=0;i<16;i++)
						{
							buffer[i]=UARTReadInt8();
						}
						MemoryWriteEnable();
						for(int i=0;i<16;i++)
						{
							MemoryWriteInt8(addr,buffer[i]);
							addr++;
						}
						MemoryWriteDisable();
					}break;
					case 'r'://read from memory
					{
						int16 addr=UARTReadInt16();
						for(int i=0;i<16;i++)
						{
							UARTWriteInt8(MemoryReadInt8(addr));
							addr++;
						}
					}break;
				}
			}break;

			case 't'://time stuff
			{
				int8 op = UARTReadInt8();
				switch(op)
				{
					case 'r'://read time
					{
						UARTWriteInt8(RTCHour  );
						UARTWriteInt8(RTCMinute);
						UARTWriteInt8(RTCSecond);
						UARTWriteInt8(RTCDay   );
					}break;
					case 'w'://read time
					{
						RTCHour  =UARTReadInt8();
						RTCMinute=UARTReadInt8();
						RTCSecond=UARTReadInt8();
						RTCDay   =UARTReadInt8();
					}break;
				}
			}break;

			case 'k'://kismet stuff
			{
				int8 op = UARTReadInt8();
				switch(op)
				{
					case 'v'://read variables
					{
						for(int i=0;i<64;i++)
						{
							UARTWriteInt16(KismetVariables[i]);
						}
					}
					break;
					case 'e':
					{
						KismetEnabled=1;
					}
					break;
					case 'd':
					{
						KismetEnabled=0;
					}
					break;
					case 'x':
					{
						int16 deviceid =UARTReadInt16();
						int8  eventid  =UARTReadInt8();
						int16 eventargs=UARTReadInt16();
						KismetExecuteEvent(deviceid,eventid,eventargs);
					}
					break;
				}
			}break;

			case 'p'://PLC stuff
			{
				int8 op = UARTReadInt8();
				switch(op)
				{
					case 'r':
					{
						UARTWriteInt8(PLCReadInt8());
					}
					break;
					case 'w':
					{
						PLCWriteInt8(UARTReadInt8());
					}
					break;
				}
			}break;
		}
	}
}
