#include <pic18.h>

#include "serial.h"

#define LED1 RC0

void main()
{
	TRISC0=0;

	SWDTEN=0;

	UARTInit();

	ANSEL = 0; 
	ANSELH = 0; 

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
			
			
		}
	}
}


