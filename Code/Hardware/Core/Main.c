#include <pic18.h>

#include "clk_freq.h" //defines clock speed for micro in MHz, ie: 3.6864Mhz

#include	"always.h"

#include	"serial.h"
//#include	"serial.c"

#define LED1 

void Init()
{
	SWDTEN=0;
	TRISC0=0;
	ANSEL=0b00011111;
}

void main()
{
	SWDTEN=0;
	TRISC0=0;
	ANSEL=0b00011111;

	//serial_setup();
	//DTRXP=1;
	//CKTXP=1;

	//16Mhz, 1200 baud (note thatthe internal clock is 16Mhz)
	SPBRG=51;
	BRGH=1;
	BRG16=0;

	SYNC=0;						//asynchronous
	SPEN=1;						//enable serial port pins
	CREN=1;						//enable reception
	SREN=0;						//no effect
	TXIE=0;						//disable tx interrupts
	RCIE=0;						//disable rx interrupts
	TX9=0;						//8-bit transmission
	RX9=0;						//8-bit reception
	TXEN=0;						//reset transmitter
	TXEN=1;						//enable the transmitter

//CKTXP=1;

ANSEL = 0; 
ANSELH = 0; 
#if 1
	while(1)
	{
		char read=getch();
		//if(read=='r')putst("Return:");
		putch(read);
		RC0=FERR;
	}
#endif

	while(1)putst("bob");
/*
	while(1)
	{
		char read = getch();
		
		if(read=='R')
		{
			putst("Roeny");
		}
		putch(read);
	}
*/

	while(1)
	{
		putst("Roeny");
		for(int d=0;d<255;d++)for(int d2=0;d2<255;d2++);//for(int d3=0;d3<255;d3++);
		RC0=!RC0;
	}

/*
	while(1)
	{
		char b = getch();
		putch(b);
		for(int d=0;d<255;d++)for(int d2=0;d2<255;d2++);//for(int d3=0;d3<255;d3++);
		RC0=1;
		b = getch();
		putch(b);
		for(int d=0;d<255;d++)for(int d2=0;d2<255;d2++);//for(int d3=0;d3<255;d3++);
		RC0=0;
	}

	while(1)
	{
		for(int d=0;d<255;d++)for(int d2=0;d2<255;d2++);//for(int d3=0;d3<255;d3++);
		RC0=1;
		for(int d=0;d<255;d++)for(int d2=0;d2<255;d2++);//for(int d3=0;d3<255;d3++);
		RC0=0;
	}
*/
	while(1)
	{
		putst("\n\n\n");
		putst("Serial tester program for PIC18F252 by Shane Tolmie\n\n");
		putst("From: http://www.workingtex.com/htpic\n\n");
	 	putst("Starting up serial @ 19200 baud, N,8,1, no flow control ...\n\n");

		for(int d=0;d<255;d++)for(int d2=0;d2<255;d2++);//for(int d3=0;d3<255;d3++);
		RC0=1;
		for(int d=0;d<255;d++)for(int d2=0;d2<255;d2++);//for(int d3=0;d3<255;d3++);
		RC0=0;

	}
}
