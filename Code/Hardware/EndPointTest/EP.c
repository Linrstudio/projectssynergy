#include "EP.h"

#define LED1A RC0
#define LED1B RC1

#define LED2A RC2
#define LED2B RB4

//Init method for the program
void Init()
{
	TRISC0=0;
	TRISC1=0;
	TRISC2=0;
	TRISB4=0;
	RC0=0;
	RC1=0;
	RC2=0;
	RB4=0;
}

//main Tick method for the program
void Tick()
{
	
}

//when polled by the main station
void Polled()
{
	RC7=!RC7;
}

//request to invoke event
//returns wether the event was successfully invoked
int8 InvokeEvent(int8 _Event,int16 _Args)
{
	switch(_Event)
	{
		case 1:
		LED1A=(_Args&1)?1:0;
		LED1B=(_Args&2)?1:0;
		break;
		case 2:
		LED2A=(_Args&1)?1:0;
		LED2B=(_Args&2)?1:0;
		break;
	}
}
