#include "settings.h"

#ifdef DIGITAL_OUT8

#include "EP.h"
#include "UART.h"

extern int8 EPBuffer[16];
extern int8 EPBufferSize;

int8 EPGetType()
{
	return 4;
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
	case 1:
		if(_Args[0])LATCbits.LATC4=0;else LATCbits.LATC4=1;
	break;
	case 2:
		if(_Args[0])LATCbits.LATC5=0;else LATCbits.LATC5=1;
	break;
	case 3:
		if(_Args[0])LATCbits.LATC3=0;else LATCbits.LATC3=1;
	break;
	case 4:
		if(_Args[0])LATCbits.LATC6=0;else LATCbits.LATC6=1;
	break;
	case 5:
		if(_Args[0])LATCbits.LATC7=0;else LATCbits.LATC7=1;
	break;
	case 6:
		if(_Args[0])LATCbits.LATC0=0;else LATCbits.LATC0=1;
	break;
	case 7:
		if(_Args[0])LATCbits.LATC1=0;else LATCbits.LATC1=1;
	break;
	case 8:
		if(_Args[0])LATCbits.LATC2=0;else LATCbits.LATC2=1;
	break;

	case 9:
		LATCbits.LATC4=!LATCbits.LATC4;
	break;
	case 10:
		LATCbits.LATC5=!LATCbits.LATC5;
	break;
	case 11:
		LATCbits.LATC3=!LATCbits.LATC3;
	break;
	case 12:
		LATCbits.LATC6=!LATCbits.LATC6;
	break;
	case 13:
		LATCbits.LATC7=!LATCbits.LATC7;
	break;
	case 14:
		LATCbits.LATC0=!LATCbits.LATC0;
	break;
	case 15:
		LATCbits.LATC1=!LATCbits.LATC1;
	break;
	case 16:
		LATCbits.LATC2=!LATCbits.LATC2;
	break;
	}
	return 255;
}
#endif
//EOF
void boreme();