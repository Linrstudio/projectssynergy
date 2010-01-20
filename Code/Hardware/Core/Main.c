#include <pic18.h>

#include "serial.h"
#include "Memory.h"

#define LED1 RC0

void main()
{
	TRISC0=0;

	SWDTEN=0;

	UARTInit();
	MemoryInit();

	ANSEL = 0; 
	ANSELH = 0; 

	UARTWriteString("UART Started");

	while(1)//main uber duber loop
	{
		char read=UARTRead();

		switch(read)
		{
			case 'l'://Set LED
			{
				if(UARTReadBool())LED1=1;else LED1=0;
			}
			break;
			
			case 't'://test
			{
				MemorySeek(0);MemoryWrite('R');
				MemorySeek(1);MemoryWrite('o');
				MemorySeek(2);MemoryWrite('e');
				MemorySeek(3);MemoryWrite('n');
				MemorySeek(4);MemoryWrite('y');
				LED1=1;
			}
			break;

			case 'w'://write to eeprom
			{
				int addr=UARTRead();
				int buffer[16];
				for(int i=0;i<16;i++)
				{
					buffer[i]=UARTRead();
				}
				for(int i=0;i<16;i++)
				{
					MemorySeek(i+addr);
					MemoryWrite(buffer[i]);
				}
			}
			break;

			case 'r':
			{
				int addr=UARTRead();
				for(int i=0;i<16;i++)
				{
					MemorySeek(i+addr);
					UARTWrite(MemoryRead());
				}
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

			case 'd'://dump EEPROM
			{
				for(int i=0;i<255;i++)
				{
					MemorySeek(i);
					UARTWrite(MemoryRead());
				}
			}
			break;
		}
	}
}
