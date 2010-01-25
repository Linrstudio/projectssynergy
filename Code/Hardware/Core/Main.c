#include <pic18.h>

#include "serial.h"
#include "Memory.h"
#include "Kismet.h"

#define LED1 RC0

typedef unsigned short ushort;

struct Int16
{
	unsigned char Hi;
	unsigned char Lo;
};

void HandleUART()
{
	if(UARTAvailable())
	{
		char read=UARTReadInt8();
		switch(read)
		{
			case 'h':
			{				
				UARTWriteInt16(EEPROMSIZE);
			}
			break;

			case 'l'://Set LED
			{
				if(UARTReadBool())LED1=1;else LED1=0;
			}
			break;

			case 'w'://write to eeprom
			{
				ushort addr=UARTReadInt16();
				int buffer[16];
				for(int i=0;i<16;i++)
				{
					buffer[i]=UARTReadInt8();
				}
				for(int i=0;i<16;i++)
				{
					MemorySeek(i+addr);
					MemoryWrite(buffer[i]);
				}
				UARTWriteInt8('w');//send a W back to confirm we are done
			}
			break;

			case 'r':
			{
				ushort addr=UARTReadInt16();
				for(int i=0;i<16;i++)
				{
					MemorySeek(addr);
					UARTWriteInt8(MemoryRead());
					addr++;
				}
			}
			break;

			case 'e':
			{
				int16 deviceid=UARTReadInt16();
				int8  eventid =UARTReadInt8();
				int16 eventargs=UARTReadInt16();

				//UARTWriteInt16(deviceid);
				//UARTWriteInt8(eventid);
				//UARTWriteInt16(eventargs);

				KismetExecuteEvent(deviceid,eventid,eventargs);
				
				//UARTWriteInt16(UARTReadInt16());
			}
			break;

			case 'c':
			{
				for(int i=0;i<255;i++)
				{
					MemorySeek(i);
					MemoryWrite(0);
				}
				UARTWriteString("Memory Cleared");
			}
			break;
		}
	}
}

void main()
{
	TRISC0=0;

	SWDTEN=0;

	UARTInit();
	MemoryInit();

	ANSEL = 0; 
	ANSELH = 0;

	LED1=0;

	while(1)//main uber duber loop
	{
		HandleUART();
		//DoKismet();
	}
}
