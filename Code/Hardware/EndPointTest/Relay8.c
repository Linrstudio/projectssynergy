#include "settings.h"
#ifdef RELAY8

#include "EP.h"
#include "UART.h"

extern int8 EPBuffer[16];
extern int8 EPBufferSize;

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
}

//main Update method for the program
void EPUpdate()
{
	//LATCbits.LATC7=!LATCbits.LATC7;
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
		if(_Args[0])LATCbits.LATC5=1;else LATCbits.LATC5=0;
	break;
	case 9:
		if(_Args[0])LATCbits.LATC4=1;else LATCbits.LATC4=0;
	break;
	case 10:
		if(_Args[0])LATCbits.LATC3=1;else LATCbits.LATC3=0;
	break;
	case 11:
		if(_Args[0])LATCbits.LATC6=1;else LATCbits.LATC6=0;
	break;
	case 12:
		if(_Args[0])LATCbits.LATC7=1;else LATCbits.LATC7=0;
	break;
	case 13:
		if(_Args[0])LATCbits.LATC2=1;else LATCbits.LATC2=0;
	break;
	case 14:
		if(_Args[0])LATCbits.LATC1=1;else LATCbits.LATC1=0;
	break;
	case 15:
		if(_Args[0])LATCbits.LATC0=1;else LATCbits.LATC0=0;
	break;

	case 16:
		LATCbits.LATC5=!LATCbits.LATC5;
	break;
	case 17:
		LATCbits.LATC4=!LATCbits.LATC4;
	break;
	case 18:
		LATCbits.LATC3=!LATCbits.LATC3;
	break;
	case 19:
		LATCbits.LATC6=!LATCbits.LATC6;
	break;
	case 20:
		LATCbits.LATC7=!LATCbits.LATC7;
	break;
	case 21:
		LATCbits.LATC2=!LATCbits.LATC2;
	break;
	case 22:
		LATCbits.LATC1=!LATCbits.LATC1;
	break;
	case 23:
		LATCbits.LATC0=!LATCbits.LATC0;
	break;

	case 24:
		EPBuffer[0]=LATCbits.LATC5;
		EPBufferSize=1;
	break;
	case 25:
		EPBuffer[0]=LATCbits.LATC4;
		EPBufferSize=1;
	break;
	case 26:
		EPBuffer[0]=LATCbits.LATC3;
		EPBufferSize=1;
	break;
	case 27:
		EPBuffer[0]=LATCbits.LATC6;
		EPBufferSize=1;
	break;
	case 28:
		EPBuffer[0]=LATCbits.LATC7;
		EPBufferSize=1;
	break;
	case 29:
		EPBuffer[0]=LATCbits.LATC2;
		EPBufferSize=1;
	break;
	case 30:
		EPBuffer[0]=LATCbits.LATC1;
		EPBufferSize=1;
	break;
	case 31:
		EPBuffer[0]=LATCbits.LATC0;
		EPBufferSize=1;
	break;	
	}
	return 255;
}
#endif
//EOF
void boreme();