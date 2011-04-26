#include "settings.h"
#ifdef RELAY8

#include "EP.h"
#include "UART.h"

extern int8 EPBuffer[16];
extern int8 EPBufferSize;

int8 state[8];

int8 EPGetType()
{
	return 5;
}

//Init method for the program
void EPInit()
{
	//make everything a input
	TRISC=0x0;
	LATC=0x0;
	state[0]=0;
	state[1]=0;
	state[2]=0;
	state[3]=0;
	state[4]=0;
	state[5]=0;
	state[6]=0;
	state[7]=0;
}

//main Update method for the program
void EPUpdate()
{
	LATCbits.LATC5=state[0]?1:0;
	LATCbits.LATC4=state[1]?1:0;
	LATCbits.LATC3=state[2]?1:0;
	LATCbits.LATC6=state[3]?1:0;
	LATCbits.LATC7=state[4]?1:0;
	LATCbits.LATC2=state[5]?1:0;
	LATCbits.LATC1=state[6]?1:0;
	LATCbits.LATC0=state[7]?1:0;
}

//when polled by the main station
void EPPolled()
{
	
}

//request to invoke event
//returns wether the event was successfully invoked
int8 EPInvokeEvent(int8 _Event,int8* _Args)
{
	switch(_Event)
	{
	case 8:
		if(_Args[0])state[0]=255;else state[0]=0;
	break;
	case 9:
		if(_Args[0])state[1]=255;else state[1]=0;
	break;
	case 10:
		if(_Args[0])state[2]=255;else state[2]=0;
	break;
	case 11:
		if(_Args[0])state[3]=255;else state[3]=0;
	break;
	case 12:
		if(_Args[0])state[4]=255;else state[4]=0;
	break;
	case 13:
		if(_Args[0])state[5]=255;else state[5]=0;
	break;
	case 14:
		if(_Args[0])state[6]=255;else state[6]=0;
	break;
	case 15:
		if(_Args[0])state[7]=255;else state[7]=0;
	break;

	case 16:
		state[0]=!state[0];
	break;
	case 17:
		state[1]=!state[1];
	break;
	case 18:
		state[2]=!state[2];
	break;
	case 19:
		state[3]=!state[3];
	break;
	case 20:
		state[4]=!state[4];
	break;
	case 21:
		state[5]=!state[5];
	break;
	case 22:
		state[6]=!state[6];
	break;
	case 23:
		state[7]=!state[7];
	break;

	case 24:
		EPBuffer[0]=state[0];
		EPBuffer[1]=state[0];
		EPBufferSize=2;
	break;
	case 25:
		EPBuffer[0]=state[1];
		EPBuffer[1]=state[1];
		EPBufferSize=2;
	break;
	case 26:
		EPBuffer[0]=state[2];
		EPBuffer[1]=state[2];
		EPBufferSize=2;
	break;
	case 27:
		EPBuffer[0]=state[3];
		EPBuffer[1]=state[3];
		EPBufferSize=2;
	break;
	case 28:
		EPBuffer[0]=state[4];
		EPBuffer[1]=state[4];
		EPBufferSize=2;
	break;
	case 29:
		EPBuffer[0]=state[5];
		EPBuffer[1]=state[5];
		EPBufferSize=2;
	break;
	case 30:
		EPBuffer[0]=state[6];
		EPBuffer[1]=state[6];
		EPBufferSize=2;
	break;
	case 31:
		EPBuffer[0]=state[7];
		EPBuffer[1]=state[7];
		EPBufferSize=2;
	break;	
	}
	return 255;
}
#endif
//EOF
void boreme();