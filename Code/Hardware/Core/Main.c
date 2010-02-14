#include <pic18.h>

#include "UART.h"
#include "Memory.h"
#include "RTC.h"
#include "Kismet.h"
#include "PLC.h"

#define LED1 RA4
#define LED2 RA5

void UARTUpdate();

void main()
{
	IRCF0=1;
	IRCF1=1;
	IRCF2=1;

	TRISA4=0;
	TRISA5=0;

	ANSEL = 0; 
	ANSELH = 0;
	SWDTEN=0;

	LED1=0;
	LED2=0;


	UARTInit();
	//PLCInit();
	MemoryInit();
	RTCInit();
	KismetInit();
	
	while(1)//main uber duber loop
	{
		UARTUpdate();//Handle UART crap
		RTCUpdate ();//Handle realtime clock
		//PLCUpdate ();//Handle PowerLineCommunications
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
			case 'h':
			{				
				UARTWriteInt16(MEMORYSIZE);
			}break;

			case 'l'://Set LED
			{
				if(UARTReadBool())LED1=1;else LED1=0;
			}break;

			case 'v'://KismetVariable stuff
			{
				int8 op = UARTReadInt8();
				switch(op)
				{
					case 'r'://read memory
					{
						for(int i=0;i<255;i++)
						{
							UARTWriteInt8(KismetVariables[i]);
						}
						UARTWriteInt8(KismetVariables[255]);//since our forloop cant reach 255 print it by hard here
					}break;
				}
			}break;

			case 'm'://memory stuff
			{
				int8 op = UARTReadInt8();
				switch(op)
				{
					case 'x'://write to memory
					{
						MemorySPIWrite(0,UARTReadInt8());
					}break;
					case 'y'://read from memory
					{
						UARTWriteInt8(MemorySPIRead(0));
					}break;
					case 's'://read from memory
					{
						UARTWriteInt8(MemorySPIReadStatus());
					}break;
					case 'w'://write to memory
					{
						int16 addr=UARTReadInt16();
						int8 buffer[16];
						for(int i=0;i<16;i++)
						{
							int8 v=UARTReadInt8();
							
							buffer[i]=v;
						}
						for(int i=0;i<16;i++)
						{
							MemoryWriteInt8(addr,buffer[i]);
							addr++;
						}
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
						int16 eventargs=UARTReadInt8();
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
