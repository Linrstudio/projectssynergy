#include "EP.h"

//push eight bits into a shift register

#define STR RC0
#define D RC1
#define CD RC2

#define CLK {CD=1;CD=0;}
void push(unsigned char _dat)
{
#if 0
	D=(_dat&128))?1:0;
	CLK;
	D=(_dat&64)?1:0;
	CLK;
	D=(_dat&32)?1:0;
	CLK;
	D=(_dat&16)?1:0;
	CLK;
	D=(_dat&8)?1:0;
	CLK;
	D=(_dat&4)?1:0;
	CLK;
	D=(_dat&2)?1:0;
	CLK;
	D=(_dat&1)?1:0;
	CLK;
	STR=1;
	STR=0;
#else
	D=(_dat&(1<<7))?1:0;
	CLK;
	D=(_dat&(1<<6))?1:0;
	CLK;
	D=(_dat&(1<<5))?1:0;
	CLK;
	D=(_dat&(1<<4))?1:0;
	CLK;
	D=(_dat&(1<<3))?1:0;
	CLK;
	D=(_dat&(1<<2))?1:0;
	CLK;
	D=(_dat&(1<<1))?1:0;
	CLK;
	D=(_dat&(1<<0))?1:0;
	CLK;
	STR=1;
	STR=0;
#endif
}

//Init method for the program
void Init()
{
	RB4=0;
}

//main Tick method for the program
void Tick()
{
	
}

//when polled by the main station
void Polled()
{
	RB4=!RB4;
}

//request to invoke event
//returns wether the event was successfully invoked
int8 InvokeEvent(int8 _Event,int16 _Args)
{
	switch(_Event)
	{
		case 1:
			RC2=0;
		break;
		case 2:
			RC2=1;
		break;
	}
}